using Android.App;
using Android.Content;
using Android.OS;
using Android.Service.Notification;

namespace BIPhone.Platforms.Android;
[Service(
        Label = "BIPhone Notification Listener",
        Permission = "android.permission.BIND_NOTIFICATION_LISTENER_SERVICE",
        Exported = true)]
[IntentFilter(new[] { "android.service.notification.NotificationListenerService" })]
public class NotificationListener : NotificationListenerService
{
    public override void OnNotificationPosted(StatusBarNotification sbn)
    {
        base.OnNotificationPosted(sbn);

        if (sbn == null) return;
        string packageName = sbn.PackageName ?? "";

        // 1. Kiểm tra xem PackageName có nằm trong danh sách Ngân hàng hay không
        if (!BankPackages.IsSupported(packageName))
        {
            return; // Bỏ qua nếu không phải app ngân hàng trong danh sách
        }

        // 2. Trích xuất nội dung thô của thông báo
        Bundle? extras = sbn.Notification?.Extras;
        if (extras == null) return;

        string title = extras.GetString(Notification.ExtraTitle, "") ?? "";
        string text = extras.GetString(Notification.ExtraText, "") ?? "";
        // Bỏ qua nếu thông báo rỗng
        if (string.IsNullOrWhiteSpace(title) && string.IsNullOrWhiteSpace(text)) return;

        // 3. LỌC NỘI DUNG (CỰC KỲ QUAN TRỌNG). Chỉ xử lý nếu text chứa các từ khóa giao dịch
        if (!IsTransactionNotification(title, text)) return;

        // 4. Đóng gói dữ liệu thô gửi về MAUI phân tích
        var notiData = new BankNotificationData
        {
            PackageName = packageName,
            Title = title,
            Content = text,
            ReceivedTime = DateTime.Now
        };

        // 5. Gọi trực tiếp EventMessenger (Không cần MainThread)
        EventMessenger.Send(this, EventEnum.OnBankTransferReceived, notiData);
    }

    private bool IsTransactionNotification(string title, string text)
    {
        // Ví dụ: Chỉ bắt thông báo có chữ "tài khoản", "số dư", "biến động"
        string content = (title + " " + text).ToLower();
        return content.Contains("tai khoan") ||
               content.Contains("so du") ||
               content.Contains("bien dong");
    }
}
public class BankNotificationData
{
    public string PackageName { get; set; }
    public string Title { get; set; }
    public string Content { get; set; }
    public DateTime ReceivedTime { get; set; }
}

public static class BankPackages
{
    // Tập hợp danh sách các Package Name ngân hàng cần lắng nghe
    public static readonly HashSet<string> SupportedBanks = new(StringComparer.OrdinalIgnoreCase)
    {
        "com.mbmobile",                 // MB Bank
        "com.VCB",                      // Vietcombank
        "vn.com.techcombank.bb.app",   // Techcombank
        "com.vnpay.bidv",               // BIDV
        "com.vietinbank.ipay",          // VietinBank
        "com.vpb.neo",                 // VPBank
        "com.acb.mobiwork",            // ACB
        "com.tpb.mb.gprsauto"          // TPBank
    };

    /// <summary>
    /// Kiểm tra xem Package Name có nằm trong danh sách theo dõi hay không
    /// </summary>
    public static bool IsSupported(string packageName)
    {
        return !string.IsNullOrEmpty(packageName) && SupportedBanks.Contains(packageName);
    }
}