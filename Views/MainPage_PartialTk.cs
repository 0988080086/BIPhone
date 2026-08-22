namespace BIPhone.Views;

public partial class MainPage
{
    // =====================================================
    // SỰ KIỆN GIAO DIỆN TAB TÀI KHOẢN (XAML ACCOUNT TAB EVENTS)
    // =====================================================

    // 1. Đăng nhập
    private void OnLoginClicked(object sender, EventArgs e)
    {
        btnLogin.IsEnabled = false;
        btnLogout.IsEnabled = true;
    }

    // 2. Đăng xuất
    private void OnLogoutClicked(object sender, EventArgs e)
    {
        btnLogin.IsEnabled = true;
        btnLogout.IsEnabled = false;
        AppSettings.LoginReset();
    }

    // 3. Mở Cài đặt
    private async void OnSettingsClicked(object sender, EventArgs e)
    {
        // TODO: Phát triển điều hướng sang trang Cài đặt (SettingsPage) tại đây
    }

    // 4. Đổi ảnh đại diện
    private void OnEditAvatarTapped(object sender, EventArgs e)
    {
        DisplayAlert("Thông báo", "Chọn ảnh đại diện mới", "OK");
    }

    // 5. Đổi điểm / Quà tặng
    private void OnRedeemPointsTapped(object sender, EventArgs e)
    {
        DisplayAlert("Đổi quà", "Mở danh sách quà tặng đổi điểm", "OK");
    }

    // 6. Trợ giúp / Chat hỗ trợ
    private void OnHelpCenterChatTapped(object sender, EventArgs e)
    {
        DisplayAlert("Trợ giúp", "Mở khung Chat hỗ trợ", "OK");
    }

    // 7. Gọi Hotline CSKH
    private void OnHotlineTapped(object sender, EventArgs e)
    {
        try
        {
            if (PhoneDialer.Default.IsSupported)
                PhoneDialer.Default.Open("19001234");
        }
        catch (Exception ex)
        {
            DisplayAlert("Lỗi", $"Không thể quay số: {ex.Message}", "OK");
        }
    }
}