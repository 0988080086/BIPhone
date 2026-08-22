using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.OS;
using AndroidX.Core.App;
using BIPhone.Platforms.Android.Services;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;

namespace BIPhone.Platforms.Android;

[Service(Exported = false, ForegroundServiceType = ForegroundService.TypeDataSync)]
public class AgentService : Service, IEventsReceiver
{
    private const int NOTIFICATION_ID = 1000;
    private const string CHANNEL_ID = "BIPhone_Channel";

    // Biến quản lý hiển thị đè màn hình cuộc gọi
    //private OverlayManager? _overlayManager;

    // Khởi tạo AgentService
    public override void OnCreate()
    {
        base.OnCreate();

        // 1. Khởi tạo OverlayManager duy nhất 1 lần
        //_overlayManager = new OverlayManager(this);
        OverlayManager.Instance.Initialize(this);

        // 2. Đăng ký nhận sự kiện cuộc gọi kết thúc
        CallBroadcastReceiver.OnCallEnded += HandleCallEnded;

        // 3. Đăng ký nhận sự kiện khi AgentService khởi tạo
        EventMessenger.Register(this);

        // 3. Khởi tạo CallManager (Sẽ kích hoạt tại đây khi bạn hoàn thiện CallManager)
        // CallManager.Instance.Init(this);

        //4: Khởi động SmsManager để ghi log Sms

    }
    //public override async Task<StartCommandResult> OnStartCommand(Intent? intent, StartCommandFlags flags, int startId)
    public override StartCommandResult OnStartCommand(Intent? intent, StartCommandFlags flags, int startId)
    {   
        try
        {
            //1. BẮT BUỘC: Gọi StartForeground ngay trong OnStartCommand để tránh crash Android 8.0+
            CreateNotification();
        }
        catch (Exception ex)
        {
            LogWriter.WriteLine ("StartCommandResult1 " + ex.Message.ToString());
        }

        try
        {
            //2. Kiểm tra ClsConnService đọc lại cấu hình, để có tài khoản API
            ClsConnService mConnService = ClsConnService.Instance;
            mConnService.LoadByWakeup();
        }
        catch (Exception ex)  
        {
            LogWriter.WriteLine("StartCommandResult2 " + ex.Message.ToString());
        }
        // Trả lại cho OnStartCommand
        return StartCommandResult.Sticky;
    }

    public async void OnEventReceived(EventMessageItem message)
    {
        if (message == null) return;

        try
        {
            // Xử lý các EventBus message đẩy về từ CallManager hoặc các Receiver khác
            switch (message.EventCode)
            {
                case EventEnum.CrmDienThoaiItem:
                    {
                        if (message.Data is CrmDienThoaiItem item && (item.TinhTrang==(decimal)CrmTelTinhTrangEnum.CallIn || item.TinhTrang == (decimal)CrmTelTinhTrangEnum.CallOut))
                        {
                            //Bổ sung thông tin đối tượng, trước khi lưu
                            item.UpdateDoiTuong();

                            //Lưu vào nhật ký cuộc gọi
                            bool _Update = await CrmDienThoai.Instance.Save(item);
                            if (_Update) 
                            {
                                //Hiển thị lên Popup Overlay
                                OverlayManager.Instance.Show(item);
                                EventMessenger.Send(this, EventEnum.CrmDienThoaiItem_RefreshCallLogs, item);
                            }
                        }
                        break;
                    }
            }
        }
        catch (Exception ex)
        {
            LogWriter.WriteLine("Lỗi OnEventReceived: " + ex.Message);
        }
    }

    private void HandleCallEnded()
    {
        // Chạy trên Main Thread (UI Thread) để đóng Popup an toàn
        MainThread.BeginInvokeOnMainThread(() =>
        {
            //_overlayManager?.Close();
            OverlayManager.Instance.Close();
        });
    }

    public override IBinder? OnBind(Intent? intent)
    {
        return null;
    }

    public override void OnDestroy()
    {
        // Hủy đăng ký để tránh leak bộ nhớ
        CallBroadcastReceiver.OnCallEnded -= HandleCallEnded;

        // Đóng Overlay
        //_overlayManager?.Close();
        OverlayManager.Instance.Close();

        // Hủy đăng ký EventMessenger
        EventMessenger.Unregister(this);

        base.OnDestroy();
    }

    private void CreateNotification()
    {
        var manager = (NotificationManager?)GetSystemService(NotificationService);

        if (Build.VERSION.SdkInt >= BuildVersionCodes.O)
        {
            var channel = new NotificationChannel(CHANNEL_ID, "BIPhone Service", NotificationImportance.Low)
            {
                Description = "BIPhone chạy nền để đón nhận cuộc gọi, gps, sms"
            };
            manager?.CreateNotificationChannel(channel);
        }

        var notification = new NotificationCompat.Builder(this, CHANNEL_ID)
            .SetContentTitle("BIPhone")
            .SetContentText("BIPhone đang chạy nền...")
            .SetSmallIcon(Resource.Mipmap.appicon)
            .SetOngoing(true)
            .Build();

        StartForeground(NOTIFICATION_ID, notification);
    }
}