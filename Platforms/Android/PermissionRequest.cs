using Android.App;
using Android.App.Roles;
using Android.Content;
using Android.Content.PM;
using Android.Net;
using Android.OS;
using Android.Provider;
using AndroidX.Core.App;
using AndroidX.Core.Content;

namespace BIPhone;

public static class PermissionRequest
{
    // TaskCompletionSource cho các quyền
    private static TaskCompletionSource<bool>? _phonePermissionTcs;             // READ_PHONE_STATE
    private static TaskCompletionSource<bool>? _callLogPermissionTcs;           // READ_CALL_LOG
    private static TaskCompletionSource<bool>? _contactsPermissionTcs;          // READ_CONTACTS
    private static TaskCompletionSource<bool>? _answerPhoneCallsPermissionTcs;  // ANSWER_PHONE_CALLS
    private static TaskCompletionSource<bool>? _callPhonePermissionTcs;         // CALL_PHONE
    private static TaskCompletionSource<bool>? _batteryPermissionTcs;           // BatteryOptimization
    private static TaskCompletionSource<bool>? _overlayPermissionTcs;           // Overlay
    private static TaskCompletionSource<bool>? _callScreeningPermissionTcs;     // CallScreening
    private static TaskCompletionSource<bool>? _usageStatsPermissionTcs;        // PACKAGE_USAGE_STATS
    private static TaskCompletionSource<bool>? _notificationlistenerTcs;        // NOTIFICATION_LISTENER

    // Mã định danh cho các dạng Quyền
    public const int RequestPhoneCode = 1001;               // READ_PHONE_STATE
    public const int RequestCallLogCode = 1002;              // READ_CALL_LOG
    public const int RequestContactsCode = 1003;             // READ_CONTACTS
    public const int RequestAnswerPhoneCallsCode = 1004;     // ANSWER_PHONE_CALLS
    public const int RequestCallPhoneCode = 1005;            // CALL_PHONE
    public const int RequestBatteryCode = 1006;              // Battery
    public const int RequestOverlayCode = 1007;              // Overlay    
    public const int RequestUsageStatsCode = 1008;           // PACKAGE_USAGE_STATS
    public const int RequestCallScreeningCode = 1009;        // CallScreening (Đã đổi sang public)
    public const int RequestNotificationListenerCode = 1010; // Đọc thông báo ứng dụng khác
        

    // 1. Request READ_PHONE_STATE
    public static Task<bool> RequestPhonePermissionAsync(Activity activity)
    {
        if (ContextCompat.CheckSelfPermission(activity, Android.Manifest.Permission.ReadPhoneState) == Permission.Granted)
        {
            return Task.FromResult(true);
        }

        _phonePermissionTcs = new TaskCompletionSource<bool>();
        ActivityCompat.RequestPermissions(activity, new string[] { Android.Manifest.Permission.ReadPhoneState }, RequestPhoneCode);
        return _phonePermissionTcs.Task;
    }

    // 2. Request READ_CALL_LOG
    public static Task<bool> RequestCallLogPermissionAsync(Activity activity)
    {
        if (ContextCompat.CheckSelfPermission(activity, Android.Manifest.Permission.ReadCallLog) == Permission.Granted)
        {
            return Task.FromResult(true);
        }

        _callLogPermissionTcs = new TaskCompletionSource<bool>();
        ActivityCompat.RequestPermissions(activity, new string[] { Android.Manifest.Permission.ReadCallLog }, RequestCallLogCode);
        return _callLogPermissionTcs.Task;
    }

    // 3. Request READ_CONTACTS
    public static Task<bool> RequestContactsPermissionAsync(Activity activity)
    {
        if (ContextCompat.CheckSelfPermission(activity, Android.Manifest.Permission.ReadContacts) == Permission.Granted)
        {
            return Task.FromResult(true);
        }

        _contactsPermissionTcs = new TaskCompletionSource<bool>();
        ActivityCompat.RequestPermissions(activity, new string[] { Android.Manifest.Permission.ReadContacts }, RequestContactsCode);
        return _contactsPermissionTcs.Task;
    }

    // 4. Request ANSWER_PHONE_CALLS
    public static Task<bool> RequestAnswerPhoneCallsPermissionAsync(Activity activity)
    {
        if (Build.VERSION.SdkInt < BuildVersionCodes.O)
        {
            return Task.FromResult(true);
        }

        if (ContextCompat.CheckSelfPermission(activity, Android.Manifest.Permission.AnswerPhoneCalls) == Permission.Granted)
        {
            return Task.FromResult(true);
        }

        _answerPhoneCallsPermissionTcs = new TaskCompletionSource<bool>();
        ActivityCompat.RequestPermissions(activity, new string[] { Android.Manifest.Permission.AnswerPhoneCalls }, RequestAnswerPhoneCallsCode);
        return _answerPhoneCallsPermissionTcs.Task;
    }

    // 5. Request CALL_PHONE
    public static Task<bool> RequestCallPhonePermissionAsync(Activity activity)
    {
        if (ContextCompat.CheckSelfPermission(activity, Android.Manifest.Permission.CallPhone) == Permission.Granted)
        {
            return Task.FromResult(true);
        }

        _callPhonePermissionTcs = new TaskCompletionSource<bool>();
        ActivityCompat.RequestPermissions(activity, new string[] { Android.Manifest.Permission.CallPhone }, RequestCallPhoneCode);
        return _callPhonePermissionTcs.Task;
    }

    // 6. Request BatteryOptimization
    public static Task<bool> RequestIgnoreBatteryOptimization(Activity activity)
    {
        if (Build.VERSION.SdkInt < BuildVersionCodes.M)
        {
            return Task.FromResult(true);
        }

        PowerManager? pm = activity.GetSystemService(Context.PowerService) as PowerManager;
        if (pm == null || pm.IsIgnoringBatteryOptimizations(activity.PackageName))
        {
            return Task.FromResult(true);
        }

        _batteryPermissionTcs = new TaskCompletionSource<bool>();
        Intent intent = new global::Android.Content.Intent(Settings.ActionRequestIgnoreBatteryOptimizations);
        intent.SetData(global::Android.Net.Uri.Parse("package:" + activity.PackageName));
        activity.StartActivityForResult(intent, RequestBatteryCode);

        return _batteryPermissionTcs.Task;
    }

    // 7. Request Overlay
    public static Task<bool> RequestOverlayPermissionAsync(Activity activity)
    {
        if (Settings.CanDrawOverlays(activity))
        {
            return Task.FromResult(true);
        }

        _overlayPermissionTcs = new TaskCompletionSource<bool>();
        global::Android.Content.Intent intent = new global::Android.Content.Intent(Settings.ActionManageOverlayPermission, global::Android.Net.Uri.Parse("package:" + activity.PackageName));
        activity.StartActivityForResult(intent, RequestOverlayCode);

        return _overlayPermissionTcs.Task;
    }

    // 8. Request CallScreening
    public static Task<bool> RequestCallScreeningAsync(Activity activity)
    {
        if (Build.VERSION.SdkInt < BuildVersionCodes.Q)
            return Task.FromResult(false);

        RoleManager? roleManager = activity.GetSystemService(Java.Lang.Class.FromType(typeof(RoleManager))) as RoleManager;
        if (roleManager == null)
            return Task.FromResult(false);

        if (roleManager.IsRoleHeld(RoleManager.RoleCallScreening))
        {
            return Task.FromResult(true);
        }

        _callScreeningPermissionTcs = new TaskCompletionSource<bool>();

#pragma warning disable CS0618
        activity.StartActivityForResult(roleManager.CreateRequestRoleIntent(RoleManager.RoleCallScreening), RequestCallScreeningCode);
#pragma warning restore CS0618        

        return _callScreeningPermissionTcs.Task;
    }

    // 9. Request PACKAGE_USAGE_STATS
    public static Task<bool> RequestUsageStatsPermissionAsync(Activity activity)
    {
        if (HasUsageStatsPermission(activity))
        {
            return Task.FromResult(true);
        }

        _usageStatsPermissionTcs = new TaskCompletionSource<bool>();
        Intent intent = new Intent(Settings.ActionUsageAccessSettings);
        intent.SetData(global::Android.Net.Uri.Parse("package:" + activity.PackageName));

        try
        {
            activity.StartActivityForResult(intent, RequestUsageStatsCode);
        }
        catch
        {
            intent = new Intent(Settings.ActionUsageAccessSettings);
            activity.StartActivityForResult(intent, RequestUsageStatsCode);
        }

        return _usageStatsPermissionTcs.Task;
    }
    public static bool HasUsageStatsPermission(Context context)
    {
        try
        {
            AppOpsManager? appOps = context.GetSystemService(Context.AppOpsService) as AppOpsManager;
            if (appOps == null) return false;

            AppOpsManagerMode mode;
            if (Build.VERSION.SdkInt >= BuildVersionCodes.Q)
            {
                mode = appOps.UnsafeCheckOpNoThrow(
                    AppOpsManager.OpstrGetUsageStats,
                    Android.OS.Process.MyUid(),
                    context.PackageName);
            }
            else
            {
                mode = appOps.CheckOpNoThrow(
                    AppOpsManager.OpstrGetUsageStats,
                    Android.OS.Process.MyUid(),
                    context.PackageName);
            }

            return mode == AppOpsManagerMode.Allowed;
        }
        catch
        {
            return false;
        }
    }

    // 10. Request NOTIFICATION_LISTENER
    public static Task<bool> RequestNotificationListenerAsync(Activity activity)
    {
        if (HasNotificationListenerPermission(activity))
        {
            return Task.FromResult(true);
        }

        _notificationlistenerTcs = new TaskCompletionSource<bool>();

        try
        {
            Intent intent = new Intent("android.settings.ACTION_NOTIFICATION_LISTENER_SETTINGS");
            activity.StartActivityForResult(intent, RequestNotificationListenerCode);
        }
        catch
        {
            // Trường hợp một số ROM tùy biến không mở được thẳng trang Notification Access
            Intent intent = new Intent(Settings.ActionSettings);
            activity.StartActivityForResult(intent, RequestNotificationListenerCode);
        }

        return _notificationlistenerTcs.Task;
    }
    /// <summary>
    /// Hàm helper kiểm tra xem app đã được bật quyền đọc thông báo trong Cài đặt chưa
    /// </summary>
    public static bool HasNotificationListenerPermission(Context context)
    {
        try
        {
            string packageName = context.PackageName;
            string flat = Settings.Secure.GetString(context.ContentResolver, "enabled_notification_listeners");

            return !string.IsNullOrEmpty(flat) && flat.Contains(packageName);
        }
        catch
        {
            return false;
        }
    }


    // Lắng nghe kết quả từ Dialog xin quyền hệ thống (1001 -> 1005)
    public static void OnRequestPermissionsResult(int requestCode, string[] permissions, Permission[] grantResults)
    {
        bool isGranted = grantResults.Length > 0 && grantResults[0] == Permission.Granted;

        switch (requestCode)
        {
            case RequestPhoneCode:
                _phonePermissionTcs?.TrySetResult(isGranted);
                break;
            case RequestCallLogCode:
                _callLogPermissionTcs?.TrySetResult(isGranted);
                break;
            case RequestContactsCode:
                _contactsPermissionTcs?.TrySetResult(isGranted);
                break;
            case RequestAnswerPhoneCallsCode:
                _answerPhoneCallsPermissionTcs?.TrySetResult(isGranted);
                break;
            case RequestCallPhoneCode:
                _callPhonePermissionTcs?.TrySetResult(isGranted);
                break;
        }
    }

    // Lắng nghe kết quả từ Trang Cài Đặt Hệ Thống (1006 -> 1009)
    public static void OnActivityResult(int requestCode, Result resultCode, Intent? data, Activity activity)
    {
        if (requestCode == RequestOverlayCode)
        {
            bool isGranted = Settings.CanDrawOverlays(activity);
            _overlayPermissionTcs?.TrySetResult(isGranted);
        }
        else if (requestCode == RequestBatteryCode)
        {
            PowerManager? pm = activity.GetSystemService(Context.PowerService) as PowerManager;
            bool isIgnoring = pm != null && pm.IsIgnoringBatteryOptimizations(activity.PackageName);
            _batteryPermissionTcs?.TrySetResult(isIgnoring);
        }
        else if (requestCode == RequestUsageStatsCode) // <-- ĐÃ BỔ SUNG ĐỂ TRÁNH TREO
        {
            bool isGranted = HasUsageStatsPermission(activity);
            _usageStatsPermissionTcs?.TrySetResult(isGranted);
        }
        else if (requestCode == RequestCallScreeningCode)
        {
            bool granted = false;
            if (Build.VERSION.SdkInt >= BuildVersionCodes.Q)
            {
                RoleManager? roleManager = activity.GetSystemService(Java.Lang.Class.FromType(typeof(RoleManager))) as RoleManager;
                if (roleManager != null)
                    granted = roleManager.IsRoleHeld(RoleManager.RoleCallScreening);
            }
            _callScreeningPermissionTcs?.TrySetResult(granted);
        }
        else if (requestCode == RequestNotificationListenerCode)
        {
            bool isGranted = HasNotificationListenerPermission(activity);
            _notificationlistenerTcs?.TrySetResult(isGranted);
        }
    }
}