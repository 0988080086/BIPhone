using Android.App;
using Android.App.Roles;
using Android.Content;
using Android.Content.PM;
using Android.OS;
using BIPhone.Platforms.Android;

namespace BIPhone;

[Activity(
    Theme = "@style/Maui.SplashTheme",
    MainLauncher = true,
    LaunchMode = LaunchMode.SingleTop,
    ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation | ConfigChanges.UiMode | ConfigChanges.ScreenLayout | ConfigChanges.SmallestScreenSize | ConfigChanges.Density)]
public class MainActivity : MauiAppCompatActivity, IEventsReceiver
{
    protected override void OnCreate(Bundle? savedInstanceState)
    {
        base.OnCreate(savedInstanceState);

        // 1: Lấy mã thiết bị deviceId, và gán vào biến toàn cục AppData.DeviceId
        string? _deviceId = Android.Provider.Settings.Secure.GetString(ContentResolver, Android.Provider.Settings.Secure.AndroidId);
        if (_deviceId == null)
        {
            _deviceId = "";
        }
        AppSettings.DeviceID = _deviceId;   //Mỗi nền tảng đều phải khởi tạo
        AppSettings.Platform = "Android";
        AppSettings.PhoneToken = "";
        AppSettings.AppType = (int)PhoneAppTypeEnum.KhachHang;

        // 2. Đăng ký nhận sự kiện khi MainActivity khởi tạo
        EventMessenger.Register(this);

        // 3. Kiểu nhận cuộc gọi (Khởi tạo ban đầu)
        AppSettings.IsCallScreeningEnabled = IsCallScreeningRoleGranted();
    }

    protected override void OnDestroy()
    {
        // Hủy đăng ký khi Activity bị hủy
        EventMessenger.Unregister(this);

        base.OnDestroy();        
    }

    // Nhận sự kiện từ EventMessenger phát ra từ bất kỳ đâu
    public void OnEventReceived(EventMessageItem message)
    {
        if (message?.EventCode == EventEnum.RequestPermissions)
        {
            int[] permissionCodes = message.DataIntSplit();
            if (permissionCodes == null || permissionCodes.Length == 0)
                return;

            // Bắt buộc gọi trên Main Thread để mở UI xin quyền
            MainThread.BeginInvokeOnMainThread(async () =>
            {
                await ProcessPermissionRequestsAsync(permissionCodes);                
            });
        }
        else if (message?.EventCode == EventEnum.StartForeGroundService && message.Data?.ToString() == "AgentService")
        {
            var intent = new Intent(this, typeof(AgentService));
            if (Build.VERSION.SdkInt >= BuildVersionCodes.O)
            {
                StartForegroundService(intent);
            }
            else
            {
                StartService(intent);
            }            
        }    
    }

    // =====================================================
    // XIN QUYỀN TUẦN TỰ THEO DANH SÁCH MÃ TRUYỀN VÀO
    // =====================================================
    private async Task ProcessPermissionRequestsAsync(int[] codes)
    {
        foreach (int code in codes)
        {
            switch (code)
            {
                case PermissionRequest.RequestPhoneCode: // 1001
                    await PermissionRequest.RequestPhonePermissionAsync(this);
                    break;

                case PermissionRequest.RequestCallLogCode: // 1002
                    await PermissionRequest.RequestCallLogPermissionAsync(this);
                    break;

                case PermissionRequest.RequestContactsCode: // 1003
                    await PermissionRequest.RequestContactsPermissionAsync(this);
                    break;

                case PermissionRequest.RequestAnswerPhoneCallsCode: // 1004
                    await PermissionRequest.RequestAnswerPhoneCallsPermissionAsync(this);
                    break;

                case PermissionRequest.RequestCallPhoneCode: // 1005
                    await PermissionRequest.RequestCallPhonePermissionAsync(this);
                    break;

                case PermissionRequest.RequestBatteryCode: // 1006
                    await PermissionRequest.RequestIgnoreBatteryOptimization(this);
                    break;

                case PermissionRequest.RequestOverlayCode: // 1007
                    await PermissionRequest.RequestOverlayPermissionAsync(this);
                    break;

                case PermissionRequest.RequestUsageStatsCode: // 1008
                    await PermissionRequest.RequestUsageStatsPermissionAsync(this);
                    break;

                case PermissionRequest.RequestCallScreeningCode: // 1009
                    await PermissionRequest.RequestCallScreeningAsync(this);
                    break;
                case PermissionRequest.RequestNotificationListenerCode: // 1010
                    await PermissionRequest.RequestNotificationListenerAsync(this);
                    break;
            }
        }
    }

    // =====================================================
    // CHUYỂN TÍN HIỆU KẾT QUẢ VỀ PERMISSIONREQUEST
    // =====================================================

    public override void OnRequestPermissionsResult(
        int requestCode,
        string[] permissions,
        [Android.Runtime.GeneratedEnum] Permission[] grantResults)
    {
        base.OnRequestPermissionsResult(requestCode, permissions, grantResults);

        // Đẩy thông số về lớp static PermissionRequest xử lý
        PermissionRequest.OnRequestPermissionsResult(requestCode, permissions, grantResults);
    }
    protected override void OnActivityResult(int requestCode, Result resultCode, Intent? data)
    {
        base.OnActivityResult(requestCode, resultCode, data);

        // Đẩy thông số về lớp static PermissionRequest xử lý
        PermissionRequest.OnActivityResult(requestCode, resultCode, data, this);
        
        // Cập nhật lại cấu hình CallScreening nếu vừa yêu cầu quyền CallScreening (1009)
        if (requestCode == PermissionRequest.RequestCallScreeningCode)
        {
            AppSettings.IsCallScreeningEnabled = IsCallScreeningRoleGranted();
        }
    }
    public bool IsCallScreeningRoleGranted()
    {
        if (Build.VERSION.SdkInt >= BuildVersionCodes.Q)
        {
            var roleManager = (RoleManager?)GetSystemService(Context.RoleService);
            return roleManager != null && roleManager.IsRoleHeld(RoleManager.RoleCallScreening);
        }
        return false;
    }
}