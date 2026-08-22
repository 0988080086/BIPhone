namespace BIPhone.Views;

public partial class MainPage
{
    // =====================================================
    // TRANG SẢN PHẨM / CATOLOGUE (PRODUCT TAB LOGIC & EVENTS)
    // =====================================================

    /// <summary>
    /// Đọc tổng số sản phẩm
    /// </summary>
    private void LoadTotalHangHoaCount()
    {
        // TODO: Logic đọc dữ liệu danh mục / sản phẩm tại đây
    }

    /// <summary>
    /// Sự kiện bấm vào thẻ sản phẩm/thẻ danh mục
    /// </summary>
    private void OnItemCardTapped(object sender, EventArgs e)
    {
        if (sender is Element element)
        {
            var actionGrid = element.FindByName<Grid>("gridActionButtons");
            if (actionGrid != null)
            {
                actionGrid.IsVisible = !actionGrid.IsVisible;
            }
        }
    }
}