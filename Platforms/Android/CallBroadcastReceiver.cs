using Android.App;
using Android.Content;
using Android.OS;
using Android.Telephony;

namespace BIPhone.Platforms.Android.Services;

[BroadcastReceiver(Enabled = true,Exported = true)]
[IntentFilter(new string[]{TelephonyManager.ActionPhoneStateChanged, Intent.ActionNewOutgoingCall })]
public class CallBroadcastReceiver : BroadcastReceiver
{
    // Tạo sự kiện static báo trạng thái cuộc gọi kết thúc
    public static event Action? OnCallEnded;

    private static string savedNumber = "";
    // Chỉ chống nhiễu Android
    private static string _lastState = "";
    private static DateTime _lastTime = DateTime.MinValue;
    // Nếu cùng trạng thái lặp lại quá nhanh thì bỏ
    private const int DuplicateMilliseconds = 50;    
    public override void OnReceive(Context? _context, Intent? _intent)
    {
        //Trạng thái cuộc gọi (RINGING, OFFHOOK, IDLE)
        //Chỉ báo ra: Cuộc gọi đi, và Kết thúc cuộc gọi
        //Cuộc gọi đến: Để ScreenCalling giải quyết
        if (_intent == null) return;


        //Đảm bảo AgentService luôn sống để nhận EventMessenger
        try
        {
            var serviceIntent = new Intent(_context, typeof(AgentService));
            if (Build.VERSION.SdkInt >= BuildVersionCodes.O)
            {
                _context.StartForegroundService(serviceIntent);
            }
            else
            {
                _context.StartService(serviceIntent);
            }
        }
        catch (Exception ex)
        {
            LogWriter.WriteLine("Start AgentService Error: " + ex.ToString());
        }

        //Bắt đầu với nhận sự kiện
        DateTime _now = DateTime.Now;
        try
        {
            //TÌNH HUỐNG 1: ActionNewOutgoingCall (Chỉ sảy ra với gọi đi)
            if (_intent.Action == Intent.ActionNewOutgoingCall)
            {   
                // Lấy số điện thoại đang gọi đi qua khóa Intent.ExtraPhoneNumber
                string? outgoingNumber = _intent.GetStringExtra(Intent.ExtraPhoneNumber);
                if (!string.IsNullOrEmpty(outgoingNumber))
                {
                    savedNumber = outgoingNumber;                    
                    BIPhone.Data.CrmDienThoaiItem _EventOut = new BIPhone.Data.CrmDienThoaiItem();
                    _EventOut.Ngay = _now;
                    _EventOut.BatDau = _now.ToString("HH:mm:ss");
                    _EventOut.KetThuc = _now.ToString("HH:mm:ss");
                    _EventOut.TinhTrang = (decimal)CrmTelTinhTrangEnum.CallOut;
                    _EventOut.Source = (decimal)TelSourceEnum.BroadcastReceiver;
                    _EventOut.CallID = 0;
                    _EventOut.DienThoai = outgoingNumber;
                    _EventOut.UID = AppSettings.DeviceID + ((DateTimeOffset)_now).ToUnixTimeSeconds();
                    _EventOut.Huong = (decimal)CrmHuongEnum._Out;

                    //Gọi đi thì luôn gửi, vì chỉ có BroadcastReceiver mới nhận ra sự kiện này
                    EventMessenger.Send(this, EventEnum.CrmDienThoaiItem, _EventOut);
                }
                return;
            }

            //TÌNH HUỐNG 2: ActionPhoneStateChanged (Có cả sự kiện phụ của gọi đến gọi đi)
            if (_intent.Action == TelephonyManager.ActionPhoneStateChanged)
            {
                string _state = "";
                object? obj = _intent.GetStringExtra(TelephonyManager.ExtraState);
                if (obj != null)
                {
                    _state = obj.ToString()!;
                }
                
                if (_state == TelephonyManager.ExtraStateRinging)
                {
                    //GỌI ĐẾN: Lấy số điện thoại từ ExtraIncomingNumber                    
                    string? incomingNumber = _intent.GetStringExtra(TelephonyManager.ExtraIncomingNumber);
                    if (!string.IsNullOrEmpty(incomingNumber))
                    {
                        savedNumber = incomingNumber;
                        BIPhone.Data.CrmDienThoaiItem _EventIn = new BIPhone.Data.CrmDienThoaiItem();
                        _EventIn.Ngay = _now;
                        _EventIn.BatDau = _now.ToString("HH:mm:ss");
                        _EventIn.KetThuc = _now.ToString("HH:mm:ss");
                        _EventIn.TinhTrang = (decimal)CrmTelTinhTrangEnum.CallIn;
                        _EventIn.Source = (decimal)TelSourceEnum.BroadcastReceiver;
                        _EventIn.CallID = 0;
                        _EventIn.DienThoai = incomingNumber;
                        _EventIn.UID = AppSettings.DeviceID + ((DateTimeOffset)_now).ToUnixTimeSeconds();
                        _EventIn.Huong = (decimal)CrmHuongEnum._In;
                        
                        //GỌI ĐẾN Chỉ gửi khi CallScreening không đăng ký thành công
                        if (AppSettings.IsCallScreeningEnabled == false)
                        {
                            EventMessenger.Send(this, EventEnum.CrmDienThoaiItem, _EventIn);
                        }
                    }
                }
                else if (_state == TelephonyManager.ExtraStateOffhook)
                {
                    //NGHE MÁY (Không phân biệt được là từ gọi đến hay gọi đi)
                    BIPhone.Data.CrmDienThoaiItem _Offhook = new BIPhone.Data.CrmDienThoaiItem();
                    //CallEventItem _Offhook = new CallEventItem();
                    _Offhook.Ngay = _now;
                    _Offhook.BatDau = _now.ToString("HH:mm:ss");
                    _Offhook.KetThuc = _now.ToString("HH:mm:ss");
                    _Offhook.TinhTrang = (decimal)CrmTelTinhTrangEnum.HookOff;
                    _Offhook.Source = (decimal)TelSourceEnum.BroadcastReceiver;
                    _Offhook.CallID = 0;
                    _Offhook.DienThoai = "";
                    _Offhook.UID = AppSettings.DeviceID + ((DateTimeOffset)_now).ToUnixTimeSeconds();
                    _Offhook.Huong = (decimal)CrmHuongEnum._Internal;

                    //Không phân biệt được nghe máy là ĐI hay ĐÊN, nên không gửi
                    EventMessenger.Send(this, EventEnum.CrmDienThoaiItem, _Offhook);
                }
                else if (_state == TelephonyManager.ExtraStateIdle)
                {
                    BIPhone.Data.CrmDienThoaiItem _EventIn = new BIPhone.Data.CrmDienThoaiItem();
                    //CallEventItem _EventIn = new CallEventItem();
                    _EventIn.Ngay = _now;
                    _EventIn.BatDau=_now.ToString("HH:mm:ss");
                    _EventIn.KetThuc = _now.ToString("HH:mm:ss");
                    _EventIn.TinhTrang = (decimal)CrmTelTinhTrangEnum.HookOff;
                    _EventIn.Source = (decimal)TelSourceEnum.BroadcastReceiver;
                    _EventIn.CallID = 0;
                    _EventIn.DienThoai = "";
                    _EventIn.UID = AppSettings.DeviceID + ((DateTimeOffset)_now).ToUnixTimeSeconds();
                    _EventIn.Huong = (decimal)CrmHuongEnum._Internal;

                    //Luôn gửi, vì chỉ có BroadcastReceiver mới nhận được sự kiện này
                    EventMessenger.Send(this, EventEnum.CrmDienThoaiItem, _EventIn);

                    //Gọi đóng cửa sổ Popup nếu tồn tại và có cấu hình CloseCallPopupWhenCallIDLE = True
                    // Bắn sự kiện 'Đóng Popup' ra ngoài
                    if (AppSettings.CloseCallPopupWhenCallIDLE==true) OnCallEnded?.Invoke();
                }
            }
        }
        catch (Exception ex)
        {
            LogWriter.WriteLine("BroadcastReceiver Error " + ex.ToString());
        }
    }
}