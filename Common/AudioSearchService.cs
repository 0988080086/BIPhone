//Gọi Task từ BroadcastReceiver

//if (state == TelephonyManager.ExtraStateIdle)
//{
//    // 1. Lấy mốc thời gian ngay tại thời điểm kết thúc cuộc gọi
//    DateTime idleNow = DateTime.Now;

//    // 2. Lấy đường dẫn cấu hình thư mục ghi âm của bạn
//    string pathRecord = AppSettings.PathRecord;

//    // 3. Kích hoạt Task chạy ngầm (fire-and-forget)
//    Task.Run(async () =>
//    {
//        await AudioSearchService.ProcessAudioSearchAsync(idleNow, pathRecord);
//    });
//}


namespace BIPhone;

public class AudioSearchService
{
    /// <summary>
    /// Task xử lý chờ 5s và quét tìm file ghi âm khớp thời gian
    /// </summary>
    /// <param name="idleTime">Mốc thời gian now() khi nhận trạng thái IDLE</param>
    /// <param name="pathRecord">Đường dẫn thư mục lưu file ghi âm từ Cấu hình</param>
    public static async Task ProcessAudioSearchAsync(string _UID, DateTime idleTime, string pathRecord)
    {
        // 1. Tự động "ngủ đông" 5 giây để máy ghi và đóng file an toàn
        await Task.Delay(5000);

        // 2. Tiến hành tìm file
        string matchedFile = FindMatchingAudioFile(idleTime, pathRecord);

        if (!string.IsNullOrEmpty(matchedFile))
        {
            // 3. Nếu tìm thấy, bắn EventBus chứa cặp (_UID, Time, FileName)
            EventMessenger.Send(null, EventEnum.RecordFile, new AudioRecordFoundEvent(_UID,idleTime, matchedFile));            
        }
    }

    private static string FindMatchingAudioFile(DateTime idleTime, string pathRecord)
    {
        if (string.IsNullOrWhiteSpace(pathRecord) || !Directory.Exists(pathRecord))
            return null;

        try
        {
            var directoryInfo = new DirectoryInfo(pathRecord);
            var searchPatterns = new[] { "*.mp3", "*.wav", "*.m4a", "*.amr", "*.3gp", "*.aac" };

            // Lấy danh sách các file âm thanh trong thư mục
            var audioFiles = searchPatterns
                .SelectMany(pattern => directoryInfo.GetFiles(pattern, SearchOption.TopDirectoryOnly))
                .ToList();

            FileInfo bestMatchFile = null;
            double minDifferenceSeconds = double.MaxValue;

            foreach (var file in audioFiles)
            {
                // Dùng LastWriteTime (Thời điểm file hoàn tất việc ghi vào bộ nhớ)
                DateTime fileTime = file.LastWriteTime;

                // Tính độ lệch thời gian tính bằng Giây
                double diffSeconds = Math.Abs((fileTime - idleTime).TotalSeconds);

                // Đúng điều kiện: Trong vòng 1 giây (<= 1.0s) và là file lệch ít nhất
                if (diffSeconds <= 1.0 && diffSeconds < minDifferenceSeconds)
                {
                    minDifferenceSeconds = diffSeconds;
                    bestMatchFile = file;
                }
            }

            return bestMatchFile?.FullName;
        }
        catch
        {
            return null;
        }
    }
}

public class AudioRecordFoundEvent
{
    public string UID { get; set; }
    public DateTime IdleTime { get; set; }
    public string FilePath { get; set; }
    public string FileName => System.IO.Path.GetFileName(FilePath);

    public AudioRecordFoundEvent(string _UID, DateTime idleTime, string filePath)
    {
        UID = _UID;
        IdleTime = idleTime;
        FilePath = filePath;
    }
}