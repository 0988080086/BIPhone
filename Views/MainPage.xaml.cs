using System.Collections.ObjectModel;
using System.Data;
using System.Windows.Input;

namespace BIPhone.Views;

public partial class MainPage : ContentPage, IEventsReceiver
{
    private ClsConnService mConnService;
    private bool _OnAppearingFirstTime = true;

    private readonly Color activeColor = Color.FromArgb("#EE4D2D"); // Màu cam Shopee
    private readonly Color inactiveColor = Color.FromArgb("#888888"); // Màu xám mờ
    private MainTab _currentTab = MainTab.Dashboard;

    // 1. Khai báo Command xử lý click Menu động
    public ICommand MenuItemCommand { get; private set; }
    // 2. Danh sách ObservableCollection Binding ra UI
    public ObservableCollection<MenuItemModel> VisibleMenuItems { get; set; } = new();


    ///// <summary>Định nghĩa 6 Tab theo đúng thứ tự chuẩn</summary>
    //private enum MainTab
    //{
    //    Dashboard, // Trang chủ
    //    Product,   // Sản phẩm (Chuyển từ Home cũ sang)
    //    Call,      // Cuộc gọi
    //    Order,     // Đơn hàng
    //    Serial,    // Serial hàng hoá
    //    Account    // Tài khoản
    //}

    /// <summary>Khởi động Mainpage</summary>
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
        //txtSearch.Keyboard = Keyboard.Text;

        // 4. Khởi tạo Menu động (Gọi trước ShowTab)
        MenuItemCommand = new Command<string>(OnExecuteMenuCommand);

        // 5. Ẩn tab Sản phẩm (false) và Công việc (false)
        ConfigureVisibleTabs(
            showDashboard: true,
            showProduct: true,
            showCall: true,
            showOrder: true,
            showTask: true,
            showAccount: true
            );

        // 6. Mặc định mở TAB 0 - Trang chủ (Dashboard)
        ShowTab(MainTab.Call);
        SetActiveTab(MainTab.Call);
    }
    
    /// <summary>Sự kiện hiển thị lần đầu => Đọc và kết nối lại API</summary>
    protected override async void OnAppearing()
    {
        base.OnAppearing();
        // 1. PHẦN GIAO DIỆN:
        bool _Logined = ClsConnService.Instance.MauiLogined;
        if (_Logined == false)
        {
            btnLogin.IsEnabled = true; btnLogin.BackgroundColor = Color.FromArgb("#EE4D2D");
            btnLogout.IsEnabled = false; btnLogout.BackgroundColor = Color.FromArgb("#888888");
        }
        else
        {
            btnLogin.IsEnabled = false; btnLogin.BackgroundColor = Color.FromArgb("#888888");
            btnLogout.IsEnabled = true; btnLogout.BackgroundColor = Color.FromArgb("#EE4D2D");
        }


        // 2. PHẦN ĐĂNG NHẬP: Chỉ thực hiện một lần với OnAppearing
        if (_OnAppearingFirstTime == false)
            return;
        _OnAppearingFirstTime = false;

        // Khởi tạo kết nối
        mConnService = ClsConnService.Instance;
        ClsConnService.Instance.LoadByAppStartup();
        
        bool _OK = false; string _LastMessage = "";
        if (AppSettings.LoginRememberAccount == true && AppSettings.LoginRememberAutoLogin == true && !string.IsNullOrEmpty(mConnService.RedirectCode) && !string.IsNullOrEmpty(mConnService.RedirectUserName) && !string.IsNullOrEmpty(mConnService.RedirectUserName) && !string.IsNullOrEmpty(mConnService.UserName) && !string.IsNullOrEmpty(mConnService.UserPass))
        {
            (_OK, _LastMessage) = await mConnService.RedirectAvailableGetAsync();
            if (_OK == true)
            {
                (_OK, _LastMessage) = await mConnService.MauiRedirectAsync();
                if (_OK == true)
                {
                    (_OK, _LastMessage) = await mConnService.MauiLoginAsync();
                }
            }
        }

        // Khởi động dữ liệu và giao diện: Loại không cần kết nối
        OnAppearing_NoNeedLogin();

        //Kiểm tra đăng nhập
        if (_OK == false || string.IsNullOrEmpty(mConnService.SecurityCode))
        {
            //Nếu chưa đăng nhập
            await Navigation.PushAsync(new LoginPage());
            return;
        }
        else
        {
            //Nếu đã đăng nhập: Báo đã đăng nhập thành công
            EventMessenger.Send(this, EventEnum.Logined, true);

            //Gọi các công đoạn cần đăng nhập mới cần chạy
            OnAppearing_NeedLogin();
        }        
    }
    private async void OnAppearing_NoNeedLogin()
    {
        // Đọc dữ liệu cuộc gọi
        await LoadDataCallsFromSQLite();
        //Hiển thị tổng số khách hàng
        await LoadTotalKhachHangCountAsync();

        await LoadHangHoaStartup();
        await LoadTotalHhCount();
        // Tải dữ liệu báo cáo ban đầu cho Dashboard (Nằm bên Partial Dashboard - MainPage_PartialDb.cs)
        await LoadDashboardData();
    }
    private async void OnAppearing_NeedLogin()
    {
        if (ClsConnService.Instance.MauiLogined== false) { return;}
        lblEventStatus.Text = "Đồng bộ dữ liệu khi khởi động ...";
        bool _OK = false; string _LastMessage = "";

        //Báo Cấp quyền cho Platforms
        string _RequestPermisionStr = AppSettings.RequestPermisionList;
        if (!string.IsNullOrEmpty(_RequestPermisionStr))
        {
            EventMessenger.Send(this, EventEnum.RequestPermissions, _RequestPermisionStr);
            EventMessenger.Send(this, EventEnum.StartForeGroundService, "AgentService");
        }
        //Đồng bộ AdminPhongBan : Hàm này tự nó lưu vào Csdl, nên không cần Scan và lưu ở ngoài
        lblEventStatus.Text = "Đồng bộ PhongBan ...";
        (_OK, _LastMessage) = await ClsConnService.Instance.MauiAdminPhongBanDbAsync();
        if (_OK == false)
        {
            await DisplayAlert("MauiAdminPhongBanDbAsync", "Đồng bộ Phong ban thất bại", "Đồng ý");
        }
        //Đồng bộ DanhMuc : Hàm này tự nó lưu vào Csdl, nên không cần Scan và lưu ở ngoài
        lblEventStatus.Text = "Đồng bộ DanhMuc ...";
        (_OK, _LastMessage) = await ClsConnService.Instance.MauiDanhMucDbAsync();
        if (_OK == false)
        {
            await DisplayAlert("MauiDanhMucDbAsync", "Đồng bộ DanhMuc thất bại", "Đồng ý");
        }
        //Đồng bộ DanhMucTinhChat : Hàm này tự nó lưu vào Csdl, nên không cần Scan và lưu ở ngoài
        lblEventStatus.Text = "Đồng bộ DanhMucTinhChat ...";
        (_OK, _LastMessage) = await ClsConnService.Instance.MauiDanhMucTinhChatDbAsync();
        if (_OK == false)
        {
            await DisplayAlert("MauiDanhMucTinhChatDbAsync", "Đồng bộ Danh mục tính chất thất bại", "Đồng ý");
        }

        DataTable _tbl; int mPage = 0;

        //Đồng bộ DoiTuong : Hàm này phải scan ở ngoài, do có nhiều page
        try
        {
            lblEventStatus.Text = "Đồng bộ DoiTuong ...";
            bool continueSync = true; mPage = 0;
            while (continueSync)
            {
                mPage++;
                lblEventStatus.Text = $"Đồng bộ DoiTuong trang {mPage}...";
                (_tbl, _LastMessage) = await mConnService.MauiDoiTuongDbAsync();
                if (_tbl != null && _tbl.Rows.Count > 0)
                {   
                    var listItems = new List<DoiTuongItem>();
                    foreach (DataRow row in _tbl.Rows)
                    {
                        var dtItem = new DoiTuongItem();
                        if (dtItem.FromDataRow(row))
                        {
                            listItems.Add(dtItem);
                        }
                    }
                    if (listItems != null && listItems.Count > 0) { DoiTuong.Instance.SaveRange(listItems); }
                }
                else
                {
                    continueSync = false;
                }
            }
        }
        catch { }
        //Đồng bộ HangHoa : Hàm này phải scan ở ngoài, do có nhiều page
        
        try
        {
            lblEventStatus.Text = "Đồng bộ HangHoa ...";
            bool continueSync = true; mPage = 0;
            while (continueSync)
            {
                mPage++;
                lblEventStatus.Text = $"Đồng bộ HangHoa trang {mPage}...";
                (_tbl, _LastMessage) = await mConnService.MauiHangHoaDbAsync();
                if (_tbl != null && _tbl.Rows.Count > 0)
                {
                    var listItems = new List<HangHoaItem>();
                    foreach (DataRow row in _tbl.Rows)
                    {
                        var hhItem = new HangHoaItem();
                        if (hhItem.FromDataRow(row))
                        {
                            listItems.Add(hhItem);
                        }
                    }
                    if (listItems != null && listItems.Count > 0) { HangHoa.Instance.SaveRange(listItems); }
                }
                else
                {
                    continueSync = false;
                }
            }
        }
        catch { }        

        //Vì đồng bộ dữ liệu sau login, nên cần khởi động lại một số lệnh trong OnAppearing_NoNeedLogin (Do có dữ liệu mới hơn)
        OnAppearing_NoNeedLogin();
    }

    #region Dynamic Menu

    // MASTER LIST chứa tất cả các Menu động phân loại theo MainTab
    private readonly List<MenuItemModel> _allMenuItems = new()
    {
        // TAB: DASHBOARD (Trang chủ)
        new MenuItemModel { Id = "NapThe", Title = "Nạp thẻ", Icon = "💳", TabCategory = MainTab.Dashboard },
        new MenuItemModel { Id = "Voucher", Title = "Voucher", Icon = "🎟️", TabCategory = MainTab.Dashboard },
        new MenuItemModel { Id = "ThanhToan", Title = "Thanh toán", Icon = "💰", TabCategory = MainTab.Dashboard },

        // TAB: CALL (Cuộc gọi)
        new MenuItemModel { Id = "LogView", Title = "Xem Log", Icon = "👁️", TabCategory = MainTab.Call },
        new MenuItemModel { Id = "LogClear", Title = "Xoá Log", Icon = "🧹", TabCategory = MainTab.Call },
        new MenuItemModel { Id = "GpsConfig", Title = "Cấu hình GPS", Icon = "📍", TabCategory = MainTab.Call },

        // TAB: PRODUCT (Sản phẩm)
        new MenuItemModel { Id = "TheGioi", Title = "Thế giới", Icon = "🌎", TabCategory = MainTab.Product },
        new MenuItemModel { Id = "GioHang", Title = "Giỏ hàng", Icon = "🛒", TabCategory = MainTab.Product }
    };

    /// <summary>Hàm gọi khi chuyển TAB (Chuyển tab Trang chủ, Cuộc gọi, Sản phẩm...)</summary>
    public void SwitchTabMenu(MainTab tab)
    {
        VisibleMenuItems.Clear();
        var filteredItems = _allMenuItems.Where(m => m.TabCategory == tab);

        foreach (var item in filteredItems)
        {
            VisibleMenuItems.Add(item);
        }
    }
    /// <summary>Bấm vào từng Icon Menu</summary>
    private void OnMenuItemTapped(object sender, TappedEventArgs e)
    {
        if (e.Parameter is int menuId)
        {
            switch (menuId)
            {
                case (int)BIPhone.MainTab.Dashboard:                    
                    break;
                case (int)BIPhone.MainTab.Call:                    
                    break;
                case (int)BIPhone.MainTab.Product:                    
                    break;
            }
        }
    }
    /// <summary>DYNAMIC Menu</summary>
    private async void OnMenuItemTapped(object sender, EventArgs e)
    {
        if (sender is VisualElement element)
        {
            // Hiệu ứng bấm nút (Scale nhẹ)
            await element.ScaleTo(0.9, 80, Easing.CubicOut);
            await element.ScaleTo(1.0, 80, Easing.CubicIn);
        }

        if (e is TappedEventArgs tappedArgs)
        {
            string actionKey = tappedArgs.Parameter?.ToString() ?? "";

            switch (actionKey)
            {
                case "NapThe":
                    // Xử lý chuyển trang/gọi hàm cho Nạp Thẻ
                    await DisplayAlert("Thông báo", "Bạn vừa chọn Nạp Thẻ", "OK");
                    break;

                case "Voucher":
                    await DisplayAlert("Thông báo", "Bạn vừa chọn Voucher", "OK");
                    break;

                case "TheGioi":
                    await DisplayAlert("Thông báo", "Bạn vừa chọn Thế Giới", "OK");
                    break;

                case "ThanhToan":
                    await DisplayAlert("Thông báo", "Bạn vừa chọn Thanh Toán", "OK");
                    break;

                case "LogClear":
                    LogWriter.Clear();
                    break;

                case "LogView":
                    string _LogFile = LogWriter.GetFileName();
                    if (!File.Exists(_LogFile))
                        return;
                    string _Text = LogWriter.ReadAll();
                    var LogControls = new UIControlList
                    {
                        FeatureId = "CrmDienThoai_NoiDung",
                        Title = "Thông tin cuộc gọi",
                        Fields = new List<UIControlItem>
                        {
                            new()
                            {
                                FieldCode = "NoiDung",
                                FieldName = "Nội dung",
                                ControlType = UIControlTypeEnum.TextMultiline, // Nhiều dòng
                                WidthPercent = 100,
                                IsRequired = true,
                                DefaultValue = _Text
                            }
                        }
                    };
                    var dynamicPage = new DynamicFormPage(LogControls);
                    await Navigation.PushAsync(dynamicPage);
                    break;
            }
        }
    }

    /// <summary>
    /// Hàm thực thi duy nhất khi click vào Menu Icon bất kỳ
    /// </summary>
    private async void OnExecuteMenuCommand(string actionKey)
    {
        if (string.IsNullOrEmpty(actionKey)) return;

        switch (actionKey)
        {
            case "NapThe":
                await DisplayAlert("Thông báo", "Bạn vừa chọn Nạp Thẻ", "OK");
                break;

            case "Voucher":
                await DisplayAlert("Thông báo", "Bạn vừa chọn Voucher", "OK");
                break;

            case "LogClear":
                LogWriter.Clear();
                await DisplayAlert("Thông báo", "Đã xóa log thành công", "OK");
                break;

            case "LogView":
                string _LogFile = LogWriter.GetFileName();
                if (!File.Exists(_LogFile)) return;
                string _Text = LogWriter.ReadAll();

                var LogControls = new UIControlList
                {
                    FeatureId = "CrmDienThoai_NoiDung",
                    Title = "Thông tin cuộc gọi",
                    Fields = new List<UIControlItem>
                    {
                        new()
                        {
                            FieldCode = "NoiDung",
                            FieldName = "Nội dung",
                            ControlType = UIControlTypeEnum.TextMultiline,
                            WidthPercent = 100,
                            IsRequired = true,
                            DefaultValue = _Text
                        }
                    }
                };
                var dynamicPage = new DynamicFormPage(LogControls);
                await Navigation.PushAsync(dynamicPage);
                break;
        }
    }

    #endregion

    #region EVENT RECEIVER

    /// <summary>EVENT RECEIVER EventMessageItem</summary>
    public void OnEventReceived(EventMessageItem message)
    {
        if (message == null) 
        { 
            return; 
        }

        switch (message.EventCode)
        {
            case EventEnum.ShowStatus:
                if (message.Data is string _Text)
                    lblEventStatus.Text = _Text;
                break;
            case EventEnum.CrmDienThoaiItem_RefreshCallLogs:
                {
                    if (message.Data is CrmDienThoaiItem item)
                    {
                        // Hàm này được gọi sang partial file MainPage_PartialCg.cs
                        UpdateCallListRealtime(item);
                    }
                    break;
                };
            case EventEnum.Logined:
                {
                    OnAppearing_NeedLogin();
                    break;
                }
            // 2. Nhận kết quả GPS trả về -> Đổ dữ liệu lại DynamicFormPage
            case EventEnum.GpsLocationRequestAnswer:
                if (message.Data is string payload && !string.IsNullOrEmpty(payload))
                {
                    var parts = payload.Split('|');
                    string targetForm = parts[0];
                    string gpsValue = parts.Length > 1 ? parts[1] : string.Empty;

                    if (targetForm == "FrmDoiTuongSM" && _currentActiveDynamicPage != null)
                    {
                        // Đảm bảo cập nhật UI trên MainThread của .NET MAUI
                        MainThread.BeginInvokeOnMainThread(() =>
                        {
                            // Kiểm tra field có tồn tại trên trang hiện tại không
                            if (_currentActiveDynamicPage.HasField("GpsLocation"))
                            {
                                _currentActiveDynamicPage.SetFieldValue("GpsLocation", gpsValue);
                            }
                            else
                            {
                                // Xử lý khi trang không có trường GPS (nếu cần)
                                System.Diagnostics.Debug.WriteLine($"Không tìm thấy trường {"GpsLocation"} trên Form.");
                            }
                        });
                    }
                }
                break;
        }
    }    
    /// <summary>Unregister EventMessenger khi OnWindowDestroying</summary>
    private void OnWindowDestroying(object? sender, EventArgs e)
    {
        // Giải phóng / Unregister khi Window bị đóng hoàn toàn
        EventMessenger.Unregister(this);

        if (Window != null)
            Window.Destroying -= OnWindowDestroying;
    }

    #endregion

    #region TAB MANAGEMENT

    private void ShowTab(MainTab tab)
    {
        _currentTab = tab;

        // Ẩn toàn bộ nội dung các View Content
        SvDashboard.IsVisible = false;
        GridSanPham.IsVisible = false;
        GridCall.IsVisible = false;
        GridGiaoHang.IsVisible = false;
        GridSerial.IsVisible = false;
        AccountContent.IsVisible = false;
        // Hiển thị TAB tương ứng được chọn
        switch (tab)
        {
            case MainTab.Dashboard:
                SvDashboard.IsVisible = true;
                LoadDashboardData(); // Gọi nạp lại dữ liệu Dashboard nếu cần
                break;

            case MainTab.Product:
                GridSanPham.IsVisible = true;
                break;

            case MainTab.Call:
                GridCall.IsVisible = true;
                break;

            case MainTab.Order:
                GridGiaoHang.IsVisible = true;
                break;

            case MainTab.Serial:
                GridSerial.IsVisible = true;
                break;

            case MainTab.Account:
                AccountContent.IsVisible = true;
                bool _Logined = ClsConnService.Instance.MauiLogined;
                if (_Logined == false)
                {
                    btnLogin.IsEnabled = true; btnLogin.BackgroundColor = Color.FromArgb("#EE4D2D");
                    btnLogout.IsEnabled = false; btnLogout.BackgroundColor = Color.FromArgb("#888888");
                }
                else
                {
                    btnLogin.IsEnabled = false; btnLogin.BackgroundColor = Color.FromArgb("#888888");
                    btnLogout.IsEnabled = true; btnLogout.BackgroundColor = Color.FromArgb("#EE4D2D");
                }
                break;
        }

        // Tự động lọc và cập nhật Menu động khi chuyển sang Tab mới
        SwitchTabMenu(tab);
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
            case MainTab.Serial:
                SelectFooterTab(lblIconTask, lblTextTask);
                break;
            case MainTab.Account:
                SelectFooterTab(lblIconAccount, lblTextAccount);
                break;
        }
    }
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
    private void ResetTabStyle(Label iconLabel, Label textLabel)
    {
        if (iconLabel != null) iconLabel.TextColor = inactiveColor;
        if (textLabel != null)
        {
            textLabel.TextColor = inactiveColor;
            textLabel.FontAttributes = FontAttributes.None;
        }
    }
    /// <summary>Hàm tùy chỉnh ẩn/hiển thị Tab theo cấu hình phân quyền</summary>
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

    #endregion

    #region TAB CLICK

    /// <summary>FOOTER TAB Dashboard</summary>
    private void OnTabDashboardClicked(object sender, EventArgs e)
    {
        SelectFooterTab(lblIconDashboard, lblTextDashboard);
        ShowTab(MainTab.Dashboard);
    }
    /// <summary>FOOTER TAB Product</summary>
    private void OnTabProductClicked(object sender, EventArgs e)
    {
        SelectFooterTab(lblIconProduct, lblTextProduct);
        ShowTab(MainTab.Product);

        LoadHangHoaStartup();
    }
    /// <summary>FOOTER TAB Call</summary>
    private void OnTabCallClicked(object sender, EventArgs e)
    {
        SelectFooterTab(lblIconCall, lblTextCall);
        ShowTab(MainTab.Call);
    }
    /// <summary>FOOTER TAB Order</summary>
    private async void OnTabOrderClicked(object sender, EventArgs e)
    {
        // 1. Chuyển UI tab ngay lập tức để ứng dụng phản hồi nhanh
        SelectFooterTab(lblIconOrder, lblTextOrder);
        ShowTab(MainTab.Order);

        // 2. Gọi tải dữ liệu bất đồng bộ sau khi UI đã đổi tab xong
        //await LoadDanhSachGiaoHangAsync();
        await InitTabGiaoHangEvents();
    }
    /// <summary>FOOTER TAB Serial</summary>
    private async void OnTabSerialClicked(object sender, EventArgs e)
    {
        SelectFooterTab(lblIconTask, lblTextTask);
        ShowTab(MainTab.Serial);

        // 2. Gọi tải dữ liệu bất đồng bộ sau khi UI đã đổi tab xong
        await InitTabSerialEvents();
    }
    /// <summary>FOOTER TAB Account</summary>
    private void OnTabAccountClicked(object sender, EventArgs e)
    {
        SelectFooterTab(lblIconAccount, lblTextAccount);
        ShowTab(MainTab.Account);
    }

    #endregion

}