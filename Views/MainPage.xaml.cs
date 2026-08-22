using Microsoft.Maui.Controls;
using System.Collections.ObjectModel;
using System.Data;

namespace BIPhone.Views;

public partial class MainPage : ContentPage, IEventsReceiver
{
    private ClsConnService mConnService;
    private bool _OnAppearingFirstTime = true;

    private readonly Color activeColor = Color.FromArgb("#EE4D2D"); // Màu cam Shopee
    private readonly Color inactiveColor = Color.FromArgb("#888888"); // Màu xám mờ

    // 1. Cập nhật Enum 6 Tab theo đúng thứ tự chuẩn
    private enum MainTab
    {
        Dashboard, // Trang chủ
        Product,   // Sản phẩm (Chuyển từ Home cũ sang)
        Call,      // Cuộc gọi
        Order,     // Đơn hàng
        Task,      // Công việc
        Account    // Tài khoản
    }

    private MainTab _currentTab = MainTab.Dashboard;

    public MainPage()
    {
        InitializeComponent();

        // Gán BindingContext chính chủ
        this.BindingContext = this;

        // Binding danh sách cuộc gọi cho CollectionView trong Tab Cuộc gọi
        cvCalls.ItemsSource = CallList;

        // 2. Đăng ký nhận sự kiện
        EventMessenger.Register(this);

        // 3. Thiết lập bàn phím cho text tìm kiếm
        txtSearch.Keyboard = Keyboard.Text;

        // 3. Ẩn tab Sản phẩm (false) và Công việc (false)
        ConfigureVisibleTabs(
            showDashboard: false,
            showProduct: false,
            showCall: true,
            showOrder: true,
            showTask: true,
            showAccount: true
            );

        // 4. Mặc định mở TAB 0 - Trang chủ (Dashboard)
        ShowTab(MainTab.Call);
        SetActiveTab(MainTab.Call);
    }

    /// <summary>
    /// Hàm tùy chỉnh ẩn/hiển thị Tab theo cấu hình phân quyền
    /// </summary>
    public void ConfigureVisibleTabs(bool showDashboard, bool showProduct, bool showCall, bool showOrder, bool showTask, bool showAccount)
    {
        // 1. Cập nhật trạng thái IsVisible cho các View nút bấm Footer
        tabDashboard.IsVisible = showDashboard;
        tabProduct.IsVisible = showProduct;
        tabCall.IsVisible = showCall;
        tabOrder.IsVisible = showOrder;
        tabTask.IsVisible = showTask;
        tabAccount.IsVisible = showAccount;

        // 2. Tính lại danh sách các Tab đang active để sắp xếp lại vị trí Grid.Column
        var visibleTabs = new List<Grid>();
        if (showDashboard) visibleTabs.Add(tabDashboard);
        if (showProduct) visibleTabs.Add(tabProduct);
        if (showCall) visibleTabs.Add(tabCall);
        if (showOrder) visibleTabs.Add(tabOrder);
        if (showTask) visibleTabs.Add(tabTask);
        if (showAccount) visibleTabs.Add(tabAccount);

        // 3. Tái cấu trúc lại ColumnDefinitions của Footer Grid
        gridFooter.ColumnDefinitions.Clear();
        for (int i = 0; i < visibleTabs.Count; i++)
        {
            // Thêm cột co giãn đều nhau (Star)
            gridFooter.ColumnDefinitions.Add(new ColumnDefinition { Width = GridLength.Star });

            // Gán lại Grid.Column liên tục từ 0 -> N-1 cho từng nút hiển thị
            Grid.SetColumn(visibleTabs[i], i);
        }
    }

    /// <summary>Sự kiện hiển thị lần đầu => Đọc và kết nối lại API</summary>
    protected override async void OnAppearing()
    {
        base.OnAppearing();

        // Chỉ thực hiện một lần với OnAppearing
        if (_OnAppearingFirstTime == false)
            return;
        _OnAppearingFirstTime = false;

        // Khởi tạo kết nối
        mConnService = ClsConnService.Instance;
        await ClsConnService.Instance.LoadByAppStartup();

        bool _OK = false;
        if (AppSettings.LoginRememberAccount == true && AppSettings.LoginRememberAutoLogin == true && !string.IsNullOrEmpty(mConnService.RedirectCode) && !string.IsNullOrEmpty(mConnService.RedirectUserName) && !string.IsNullOrEmpty(mConnService.RedirectUserName) && !string.IsNullOrEmpty(mConnService.UserName) && !string.IsNullOrEmpty(mConnService.UserPass))
        {
            _OK = await mConnService.RedirectAvailableGetAsync();
            if (_OK == true)
            {
                _OK = await mConnService.MauiRedirectAsync();
                if (_OK == true)
                {
                    _OK = await mConnService.MauiLoginAsync();
                }
            }
        }

        if (_OK == false || string.IsNullOrEmpty(mConnService.SecurityCode))
        {
            await Navigation.PushAsync(new LoginPage());
            return;
        }
        else
        {
            string _RequestPermisionStr = AppSettings.RequestPermisionList;
            if (!string.IsNullOrEmpty(_RequestPermisionStr))
            {
                EventMessenger.Send(this, EventEnum.RequestPermissions, _RequestPermisionStr);
                EventMessenger.Send(this, EventEnum.StartForeGroundService, "AgentService");
            }
        }

        // Đọc dữ liệu cuộc gọi & tổng số KH lần đầu (Nằm bên Partial Call - MainPage_PartialCg.cs)
        LoadDataCallsFromSQLite();
        LoadTotalKhachHangCount();

        // Tải dữ liệu báo cáo ban đầu cho Dashboard (Nằm bên Partial Dashboard - MainPage_PartialDb.cs)
        LoadDashboardData();
    }

    // EVENT RECEIVER EventMessageItem
    public void OnEventReceived(EventMessageItem message)
    {
        if (message == null) return;

        switch (message.EventCode)
        {
            case EventEnum.CrmDienThoaiItem_RefreshCallLogs:
                {
                    if (message.Data is CrmDienThoaiItem item)
                    {
                        // Hàm này được gọi sang partial file MainPage_PartialCg.cs
                        UpdateCallListRealtime(item);
                    }
                    break;
                }
        }
    }

    protected override void OnDisappearing()
    {
        EventMessenger.Unregister(this);
        base.OnDisappearing();
    }

    // =====================================================
    // TAB MANAGEMENT
    // =====================================================
    private void ShowTab(MainTab tab)
    {
        _currentTab = tab;

        // Ẩn toàn bộ nội dung các View Content
        DashboardContent.IsVisible = false;
        ProductContent.IsVisible = false;
        CallContent.IsVisible = false;
        OrderContent.IsVisible = false;
        TaskContent.IsVisible = false;
        AccountContent.IsVisible = false;

        // Hiển thị TAB tương ứng được chọn
        switch (tab)
        {
            case MainTab.Dashboard:
                DashboardContent.IsVisible = true;
                LoadDashboardData(); // Gọi nạp lại dữ liệu Dashboard nếu cần
                break;

            case MainTab.Product:
                ProductContent.IsVisible = true;
                break;

            case MainTab.Call:
                CallContent.IsVisible = true;
                break;

            case MainTab.Order:
                OrderContent.IsVisible = true;
                break;

            case MainTab.Task:
                TaskContent.IsVisible = true;
                break;

            case MainTab.Account:
                AccountContent.IsVisible = true;
                break;
        }
    }

    // =====================================================
    // FOOTER TAB EVENTS
    // =====================================================
    private void OnTabDashboardClicked(object sender, EventArgs e)
    {
        SelectFooterTab(0, lblIconDashboard, lblTextDashboard);
        ShowTab(MainTab.Dashboard);
    }

    private void OnTabProductClicked(object sender, EventArgs e)
    {
        SelectFooterTab(1, lblIconProduct, lblTextProduct);
        ShowTab(MainTab.Product);
    }

    private void OnTabCallClicked(object sender, EventArgs e)
    {
        SelectFooterTab(2, lblIconCall, lblTextCall);
        ShowTab(MainTab.Call);
    }

    private void OnTabOrderClicked(object sender, EventArgs e)
    {
        SelectFooterTab(3, lblIconOrder, lblTextOrder);
        ShowTab(MainTab.Order);
    }

    private void OnTabTaskClicked(object sender, EventArgs e)
    {
        SelectFooterTab(4, lblIconTask, lblTextTask);
        ShowTab(MainTab.Task);
    }

    private void OnTabAccountClicked(object sender, EventArgs e)
    {
        SelectFooterTab(5, lblIconAccount, lblTextAccount);
        ShowTab(MainTab.Account);
    }

    private void SetActiveTab(MainTab tab)
    {
        switch (tab)
        {
            case MainTab.Dashboard:
                SelectFooterTab(lblIconDashboard, lblTextDashboard);
                break;
            case MainTab.Product:
                SelectFooterTab(lblIconProduct, lblTextProduct);
                break;
            case MainTab.Call:
                SelectFooterTab(lblIconCall, lblTextCall);
                break;
            case MainTab.Order:
                SelectFooterTab(lblIconOrder, lblTextOrder);
                break;
            case MainTab.Task:
                SelectFooterTab(lblIconTask, lblTextTask);
                break;
            case MainTab.Account:
                SelectFooterTab(lblIconAccount, lblTextAccount);
                break;
        }
    }

    // Bỏ tham số tabIndex không cần thiết ở hàm này
    private void SelectFooterTab(Label activeIcon, Label activeText)
    {
        ResetTabStyle(lblIconDashboard, lblTextDashboard);
        ResetTabStyle(lblIconProduct, lblTextProduct);
        ResetTabStyle(lblIconCall, lblTextCall);
        ResetTabStyle(lblIconOrder, lblTextOrder);
        ResetTabStyle(lblIconTask, lblTextTask);
        ResetTabStyle(lblIconAccount, lblTextAccount);

        if (activeIcon != null) activeIcon.TextColor = activeColor;
        if (activeText != null)
        {
            activeText.TextColor = activeColor;
            activeText.FontAttributes = FontAttributes.Bold;
        }
    }

    private void SelectFooterTab(int tabIndex, Label activeIcon, Label activeText)
    {
        ResetTabStyle(lblIconDashboard, lblTextDashboard);
        ResetTabStyle(lblIconProduct, lblTextProduct);
        ResetTabStyle(lblIconCall, lblTextCall);
        ResetTabStyle(lblIconOrder, lblTextOrder);
        ResetTabStyle(lblIconTask, lblTextTask);
        ResetTabStyle(lblIconAccount, lblTextAccount);

        if (activeIcon != null) activeIcon.TextColor = activeColor;
        if (activeText != null)
        {
            activeText.TextColor = activeColor;
            activeText.FontAttributes = FontAttributes.Bold;
        }
    }

    private void ResetTabStyle(Label iconLabel, Label textLabel)
    {
        if (iconLabel != null) iconLabel.TextColor = inactiveColor;
        if (textLabel != null)
        {
            textLabel.TextColor = inactiveColor;
            textLabel.FontAttributes = FontAttributes.None;
        }
    }

    // =====================================================
    // SEARCH
    // =====================================================
    private void OnSearchTextChanged(object sender, TextChangedEventArgs e)
    {
        string keyword = e.NewTextValue ?? "";
        System.Diagnostics.Debug.WriteLine("Search: " + keyword);
    }

    // =====================================================
    // MICRO / CART / MESSAGE / MENU
    // =====================================================
    private async void OnMicroClicked(object sender, EventArgs e)
    {
        await DisplayAlert("Micro", "Sau này gọi chức năng Voice → Text / Translate.", "Đóng");
    }

    private async void OnCartClicked(object sender, EventArgs e)
    {
        await DisplayAlert("Giỏ hàng", "Chức năng giỏ hàng.", "Đóng");
    }

    private async void OnMessageClicked(object sender, EventArgs e)
    {
        await DisplayAlert("Tin nhắn", "Chức năng Chat.", "Đóng");
    }

    private async void OnMenuClicked(object sender, EventArgs e)
    {
        if (sender is Button button)
        {
            string menuName = button.Text;
            await DisplayAlert("Menu", "Đã chọn: " + menuName, "Đóng");
        }
    }       
}