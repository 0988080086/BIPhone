using System.Collections.ObjectModel;

namespace BIPhone.Views;

public partial class MainPage
{
    // =====================================================
    // KHAI BÁO BIẾN & THUỘC TÍNH PHỤ TRÁCH ĐƠN HÀNG
    // =====================================================

    // Collection quản lý danh sách đơn hàng trên UI (sẵn sàng cho bạn phát triển sau)
    // public ObservableCollection<OrderModel> OrderList { get; set; } = new ObservableCollection<OrderModel>();

    // =====================================================
    // SỰ KIỆN GIAO DIỆN TAB ĐƠN HÀNG (XAML ORDER TAB EVENTS)
    // =====================================================

    /// <summary>
    /// Nút lọc danh sách đơn hàng theo trạng thái
    /// </summary>
    private void OnOrderFilterClicked(object sender, EventArgs e)
    {
        if (sender is Button button)
        {
            string filterStatus = button.Text;
            System.Diagnostics.Debug.WriteLine("Lọc đơn hàng theo trạng thái: " + filterStatus);

            // TODO: Phát triển logic lọc dữ liệu đơn hàng tại đây
        }
    }

    /// <summary>
    /// Sự kiện khi chọn một dòng trong danh sách đơn hàng
    /// </summary>
    private void OnOrderSelected(object sender, SelectionChangedEventArgs e)
    {
        var selectedOrder = e.CurrentSelection.FirstOrDefault();
        if (selectedOrder == null) return;

        System.Diagnostics.Debug.WriteLine("Đã chọn đơn hàng: " + selectedOrder);

        // Bỏ chọn dòng highlight
        if (sender is CollectionView cv)
        {
            cv.SelectedItem = null;
        }

        // TODO: Phát triển logic xem chi tiết đơn hàng tại đây
    }
}