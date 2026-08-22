using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace BIPhone.Views;

public partial class MainPage
{
    // Model dữ liệu đại diện cho 1 Bài báo / Công việc
    public class TaskItem : INotifyPropertyChanged
    {
        public string Id { get; set; }
        public string Title { get; set; }
        public string Summary { get; set; }
        public string ImageUrl { get; set; }
        public string Status { get; set; } // "Pending", "InProgress", "Completed"

        private bool _isSelected;
        public bool IsSelected
        {
            get => _isSelected;
            set { _isSelected = value; OnPropertyChanged(); }
        }

        public bool IsPending => Status == "Pending";
        public bool IsNotCompleted => Status != "Completed";

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string propertyName = null) =>
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }

    // Danh sách dữ liệu chính & Danh sách hiển thị sau lọc
    private ObservableCollection<TaskItem> allTasks = new ObservableCollection<TaskItem>();
    private ObservableCollection<TaskItem> filteredTasks = new ObservableCollection<TaskItem>();
    private string currentTaskStatus = "Pending"; // Mặc định mở Tab "Chưa tiếp nhận"

    // Khởi tạo trong Constructor của MainPage
    private void InitTaskTab()
    {
        cvTasks.ItemsSource = filteredTasks;
        LoadTasksFromApi();
    }

    // 1. Giả lập / Gọi API lấy dữ liệu công việc
    private async void LoadTasksFromApi()
    {
        refreshTasks.IsRefreshing = true;

        try
        {
            // TODO: Thay bằng HttpClient call API thực tế của bạn
            // var result = await httpClient.GetFromJsonAsync<List<TaskItem>>("https://your-api.com/tasks");

            await Task.Delay(800); // Giả lập độ trễ mạng

            allTasks = new ObservableCollection<TaskItem>
                {
                    new TaskItem
                    {
                        Id = "1",
                        Title = "Hết thời lái xe tùy tiện khi camera AI giăng khắp phố",
                        Summary = "Khả năng ghi nhận vi phạm ngày càng rộng khiến tài xế phải bỏ tâm lý chủ quan, làm theo thói quen khi tham gia giao thông.",
                        ImageUrl = "https://picsum.photos/200/150?random=1",
                        Status = "Pending"
                    },
                    new TaskItem
                    {
                        Id = "2",
                        Title = "HLV Thái Lan: 'Ủng hộ cách Việt Nam nhập tịch cầu thủ Brazil'",
                        Summary = "HLV Anthony Hudson ủng hộ cách Việt Nam nâng cao sức mạnh bằng cầu thủ gốc Brazil, trước khi đối đầu Thái Lan ở chung kết ASEAN Cup 2026.",
                        ImageUrl = "https://picsum.photos/200/150?random=2",
                        Status = "Pending"
                    },
                    new TaskItem
                    {
                        Id = "3",
                        Title = "Kiểm tra hạ tầng mạng viễn thông khu vực Quận 1",
                        Summary = "Rà soát toàn bộ các trạm BTS và tuyến cáp quang chính phục vụ sự kiện sắp tới.",
                        ImageUrl = "https://picsum.photos/200/150?random=3",
                        Status = "InProgress"
                    }
                };

            FilterTasksByStatus(currentTaskStatus);
        }
        catch (Exception ex)
        {
            await DisplayAlert("Lỗi", $"Không thể tải danh sách công việc: {ex.Message}", "OK");
        }
        finally
        {
            refreshTasks.IsRefreshing = false;
        }
    }

    // 2. Chuyển đổi qua lại giữa 3 Tab trạng thái
    private void OnTaskStatusTabClicked(object sender, EventArgs e)
    {
        if (sender is TapGestureRecognizer tap && tap.CommandParameter is string status)
        {
            currentTaskStatus = status;

            // Reset màu Tab UI
            lblTabPending.TextColor = status == "Pending" ? Color.FromArgb("#EE4D2D") : Color.FromArgb("#666666");
            lineTabPending.Color = status == "Pending" ? Color.FromArgb("#EE4D2D") : Colors.Transparent;

            lblTabInProgress.TextColor = status == "InProgress" ? Color.FromArgb("#EE4D2D") : Color.FromArgb("#666666");
            lineTabInProgress.Color = status == "InProgress" ? Color.FromArgb("#EE4D2D") : Colors.Transparent;

            lblTabCompleted.TextColor = status == "Completed" ? Color.FromArgb("#EE4D2D") : Color.FromArgb("#666666");
            lineTabCompleted.Color = status == "Completed" ? Color.FromArgb("#EE4D2D") : Colors.Transparent;

            FilterTasksByStatus(status);
        }
    }

    private void FilterTasksByStatus(string status)
    {
        filteredTasks.Clear();
        var items = allTasks.Where(t => t.Status == status);
        foreach (var item in items)
        {
            item.IsSelected = false; // Reset trạng thái đóng/mở nút
            filteredTasks.Add(item);
        }
    }

    // 3. Khi bấm vào 1 bài báo -> Ẩn/Hiện 3 nút hành động
    private void OnTaskArticleTapped(object sender, EventArgs e)
    {
        if (sender is TapGestureRecognizer tap && tap.CommandParameter is TaskItem selectedTask)
        {
            // Đóng tất cả các item khác
            foreach (var item in filteredTasks)
            {
                if (item != selectedTask) item.IsSelected = false;
            }

            // Toggle đóng/mở nút bài đang chọn
            selectedTask.IsSelected = !selectedTask.IsSelected;
        }
    }

    // 4. Kéo xuống để Refresh danh sách
    private void OnRefreshTasks(object sender, EventArgs e)
    {
        LoadTasksFromApi();
    }

    // =====================================================
    // XỬ LÝ 3 NÚT HÀNH ĐỘNG
    // =====================================================

    // Nút 1: Tiếp nhận
    private async void OnAcceptTaskClicked(object sender, EventArgs e)
    {
        if (sender is Button btn && btn.CommandParameter is TaskItem task)
        {
            // TODO: Call API Tiếp nhận bài báo / công việc ở đây
            // await httpClient.PostAsync($"https://your-api.com/tasks/{task.Id}/accept", null);

            task.Status = "InProgress";
            FilterTasksByStatus(currentTaskStatus); // Refresh lại giao diện Tab hiện tại
            await DisplayAlert("Thành công", $"Đã tiếp nhận công việc: {task.Title}", "OK");
        }
    }

    // Nút 2: Hủy tiếp nhận (Hiển thị hộp thoại nhập lý do)
    private async void OnRejectTaskClicked(object sender, EventArgs e)
    {
        if (sender is Button btn && btn.CommandParameter is TaskItem task)
        {
            string reason = await DisplayPromptAsync(
                "Hủy tiếp nhận",
                "Vui lòng nhập lý do hủy công việc này:",
                accept: "Xác nhận",
                cancel: "Hủy bỏ",
                placeholder: "Nhập lý do...");

            if (!string.IsNullOrWhiteSpace(reason))
            {
                // TODO: Call API Hủy tiếp nhận gửi lý do lên Server
                // await httpClient.PostAsJsonAsync($"https://your-api.com/tasks/{task.Id}/reject", new { Reason = reason });

                allTasks.Remove(task);
                FilterTasksByStatus(currentTaskStatus); // Refresh danh sách sau khi hủy
                await DisplayAlert("Thông báo", "Đã hủy tiếp nhận công việc thành công.", "OK");
            }
        }
    }

    // Nút 3: Báo cáo (Để trống chờ kết nối Dynamic ContentPage sau)
    private async void OnReportTaskClicked(object sender, EventArgs e)
    {
        if (sender is Button btn && btn.CommandParameter is TaskItem task)
        {
            // Mở Dynamic ContentPage báo cáo ở đây
            await DisplayAlert("Báo cáo", $"Mở form báo cáo cho công việc ID: {task.Id}\n(Đang chờ ghép Dynamic ContentPage)", "OK");
        }
    }
}
