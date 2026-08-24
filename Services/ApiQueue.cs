//LƯU TÂM KHI SỬ DỤNG ConcurrentQueue
//Lấy item đầu tiên: _queue.TryPeek(out item)
//Lấy item đầu tiên: _queue.TryDequeue(out item) - Lấy xong xoá luôn item đầu tiên
//Xoá item đầu tiên: _queue.TryDequeue(out _)

using System;
using System.Collections.Concurrent;
using System.Threading.Tasks;

namespace BIPhone.Services;

public class ApiQueue
{
    // SINGLETON
    private static readonly ApiQueue _instance = new ApiQueue();
    public static ApiQueue Instance
    {
        get { return _instance; }
    }
    // HÀNG ĐỢI
    private readonly ConcurrentQueue<EventMessageItem> _queue = new ConcurrentQueue<EventMessageItem>();
    // CONSTRUCTOR
    private ApiQueue()
    {
        // Khởi động Worker
        Task.Run(Worker);
    }

    public void Add(EventMessageItem item)
    {
        if (item == null)
            return;

        _queue.Enqueue(item);

        LogWriter.WriteLine("ApiQueue.Add: " + item.EventCode.ToString());
    }
    private async Task Worker()
    {
        LogWriter.WriteLine( "ApiQueue Worker START");

        while (true)
        {
            try
            {
                EventMessageItem item;

                // Có việc thì lấy ra xử lý ngay
                if (_queue.TryPeek(out item))
                {
                    LogWriter.WriteLine("ApiQueue Process: " + item.EventCode.ToString());
                    await Process(item);
                }
                else
                {
                    // Không có việc → ngủ 5 giây
                    await Task.Delay(5000);
                }
            }
            catch (Exception ex)
            {
                LogWriter.WriteLine( "ApiQueue Worker ERROR: " + ex.Message);                
            }
            //Tạm thời vẫn nhủ thêm 5 giây cho tất cả các trường hợp, sau này bỏ sau
            //await Task.Delay(5000);
        }
    }
    private async Task Process(EventMessageItem item)
    {
        try
        {
            switch (item.EventCode)
            {
                case EventEnum.CrmDienThoaiItem:
                    //Lấy Item đầu tiên trong hàng đợi
                    if (item.Data is CrmDienThoaiItem _TelItem && _TelItem.Synced != 1)
                    {
                        //POST lên API
                        bool _OK; string _LastMessage;
                        (_OK,_LastMessage) = await ClsConnService.Instance.MauiDienThoaiSetAsync(_TelItem);
                        if (_OK)
                        {
                            //POST thành công thì thiết lập Synced = 1 và lưu vào SQLite
                            _TelItem.Synced = 1;
                            _OK = await CrmDienThoai.Instance.Save(_TelItem);
                            //Xoá hàng đợi đầu tiên vừa lấy
                            _queue.TryDequeue(out _);
                        }                        
                    }
                    break;

                //case EventEnum.CrmSmsItem:
                //    // Xử lý SMS
                //    break;

                default:
                    // Không thuộc các Case ở trên
                    break;
            }
            await Task.CompletedTask;
        }
        catch (Exception ex)
        {
            LogWriter.WriteLine( "ApiQueue.Process ERROR: " + ex.Message);
        }
    }
}
