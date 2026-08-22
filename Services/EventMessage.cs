using CommunityToolkit.Mvvm.Messaging;
using System.Globalization;

namespace BIPhone.Services;
// =========================================================================
// 1. INTERFACE CHUNG: Cho MỌI Class (ContentPage, BasePage, ViewModel, Service...)
// =========================================================================
public interface IEventsReceiver
{
    // Hàm này sẽ tự động chạy khi có sự kiện
    void OnEventReceived(EventMessageItem message);
}

// =========================================================================
// 2. ENUM & GÓI DỮ LIỆU
// =========================================================================
public enum EventEnum
{
    None = 0,
    RequestPermissions=1,
    Logined=2,
    StartForeGroundService=3,
    CrmDienThoaiItem = 4,                       //Mới nhận cuộc gọi => Cần lưu và hiển thị Popup
    CrmDienThoaiItem_RefreshCallLogs = 5,       //Đã hiển thị và quay lại cập nhật vào danh sách CallLogs trên màn hình chính
    RecordFile = 6,                              //File ghi âm cuộc gọi
    OnBankTransferReceived=7
}

public class EventMessageItem
{
    public object Sender { get; set; } // Người gửi (luôn là 'this')
    public EventEnum EventCode { get; set; }
    public object? Data { get; set; }

    public EventMessageItem(object sender, EventEnum eventCode, object? data = null)
    {
        Sender = sender;
        EventCode = eventCode;
        Data = data;
    }

    /// <summary>Tách chuỗi data thành một mảng cách nhau bởi dấu ','</summary>
    public string[] DataStringSplit(char separator = ',')
    {
        if (Data == null) 
            return Array.Empty<string>();

        string strData = Data.ToString() ?? string.Empty;
        if (string.IsNullOrWhiteSpace(strData))
            return Array.Empty<string>();

        // Tự động xóa các mục rỗng và xóa khoảng trắng ở 2 đầu mỗi phần tử (Trim)
        return strData.Split(separator, StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
    }

    /// <summary>Tách chuỗi Số data thành một mảng số cách nhau bởi dấu ','</summary>
    public decimal[] DataNumberSplit(char separator = ',')
    {
        var stringArray = DataStringSplit(separator);
        var numberList = new List<decimal>();

        foreach (var item in stringArray)
        {
            // Thử ép kiểu số hỗ trợ cả dấu . và , thập phân
            if (decimal.TryParse(item, NumberStyles.Any, CultureInfo.InvariantCulture, out decimal val) ||
                decimal.TryParse(item, NumberStyles.Any, CultureInfo.CurrentCulture, out val))
            {
                numberList.Add(val);
            }
        }

        return numberList.ToArray();
    }

    /// <summary>Tách chuỗi Số data thành một mảng số cách nhau bởi dấu ','</summary>
    public int[] DataIntSplit(char separator = ',')
    {
        var stringArray = DataStringSplit(separator);
        var intList = new List<int>();

        foreach (var item in stringArray)
        {
            if (int.TryParse(item, NumberStyles.Any, CultureInfo.InvariantCulture, out int val))
            {
                intList.Add(val);
            }
        }

        return intList.ToArray();
    }
}

// =========================================================================
// 3. BỘ ĐIỀU PHỐI TẬP TRUNG (EVENT PIPELINE)
// =========================================================================
public static class EventMessenger
{
    // 1. HÀM REGISTER TỰ ĐỘNG PHÂN BIỆT UI VÀ SERVICE// =========================================================================
    // 1. HÀM SEND DUY NHẤT: Bắt buộc truyền 'this'
    // =========================================================================
    public static void Send(object sender, EventEnum eventCode, object? data = null)
    {
        var message = new EventMessageItem(sender, eventCode, data);
        Task.Run(() =>
        {
            WeakReferenceMessenger.Default.Send(message);
        });
    }
    // =========================================================================
    // 2. HÀM REGISTER TỰ ĐỘNG PHÂN BIỆT UI VÀ SERVICE
    // =========================================================================
    public static void Register(IEventsReceiver recipient)
    {
        WeakReferenceMessenger.Default.Register<EventMessageItem>(recipient, (r, message) =>
        {
            // 🔍 KIỂM TRA 1: Nơi đăng ký nhận (r) có phải là GIAO DIỆN UI không?
            bool isUiComponent = r is Element; // (Bao gồm ContentPage, BasePage, ContentView...)

            // 🔍 KIỂM TRA 2: So sánh ô nhớ giữa Nơi nhận (r) và Nơi gửi (message.Sender)
            bool isSelfSent = ReferenceEquals(r, message.Sender);

            // 🛑 QUY TẮC THÔNG MINH:
            // Nếu nơi nhận là UI VÀ tin nhắn do CHÍNH MÀN HÌNH ĐÓ phát ra -> TỰ ĐỘNG BỎ QUA (Chống lặp UI)
            if (isUiComponent && isSelfSent)
            {
                return;
            }

            // 🟢 NẾU LÀ SERVICE HOẶC MÀN HÌNH KHÁC -> Cho qua luồng xử lý
            ProcessEventPipeline(() => recipient.OnEventReceived(message), message);
        });
    }
    public static void Unregister(object recipient)
    {
        WeakReferenceMessenger.Default.Unregister<EventMessageItem>(recipient);
    }

    private static void ProcessEventPipeline(Action executionAction, EventMessageItem message)
    {
        MainThread.BeginInvokeOnMainThread(() =>
        {
            try
            {
                executionAction?.Invoke();
            }
            catch (Exception ex)
            {
                //LogWriter.WriteLine($"[EVENT ERROR] Lỗi sự kiện {message.EventCode}: {ex.Message}");
            }
        });
    }
}

//CƠ CHẾ HOẠT ĐỘNG NHƯ SAU:

//[Nơi phát lệnh] (Service / Button / Background)
//       │
//       ▼  1. Gọi Send(EventEnum, Data)
//[EventMessenger.Send]
//       │
//       ▼  2. Đóng gói thành EventMessageItem & Đưa vào Task ngầm
//[WeakReferenceMessenger.Default]
//       │  3. Tìm danh sách các class ĐÃ ĐĂNG KÝ(Subscribers)
//       │
//       ├───────────────────────┬───────────────────────┐
//       ▼                       ▼                       ▼
// [PhoneHistory][MainViewModel][Background Service]
//       │                       │                       │
//       └───────────────────────┴───────────────────────┘
//                               │
//                               ▼ 4. Chạy qua ProcessEventPipeline
//                       [MainThread.BeginInvoke]
//                               │
//                               ▼ 5. Gọi OnEventReceived(message)
//                      [Cập nhật UI / Dữ liệu]

//1. Khai báo & Đăng ký (Register)
//public class TenClassCuaBan : IEventReceiver // 1. Khai báo Interface
//{
//    public TenClassCuaBan()
//    {
//        //Gọi đúng 1 dòng lệnh này trong Constructor
//        EventMessenger.Register(this);
//    }
//}

//2. Khi Gửi tín hiệu (Send)
// Luôn luôn truyền 'this' làm tham số đầu tiên
//EventMessenger.Send(this, EventEnum.Crm_TelePhone, "0988080086");

//3. Khi Nhận tín hiệu (OnEventReceived)
//public void OnEventReceived(EventMessageItem message)
//    {
//        switch (message.EventCode)
//        {
//            case EventEnum.Crm_TelePhone:
//                // Viết trực tiếp logic xử lý dữ liệu tại đây
//                break;
//        }
//    }