
using BIPhone.Dynamic;
namespace BIPhone.Dynamic;
public class TaxAgencyLookupPopupPage : ContentPage
{
    private readonly List<TaxAgencyItem> _allItems;
    private CollectionView _collectionView = null!;

    // Action Callback trả lại đối tượng được chọn về cho DynamicFormPage
    public Action<TaxAgencyItem>? OnItemSelected;

    public TaxAgencyLookupPopupPage(List<TaxAgencyItem> sourceData)
    {
        _allItems = sourceData;
        Title = "Chọn Cơ Quan Quản Lý Thuế";
        BuildUI();
    }

    private void BuildUI()
    {
        // 1. Ô tìm kiếm tự động lọc theo MST, Tên hoặc Địa chỉ
        var searchBar = new SearchBar
        {
            Placeholder = "Gõ tìm theo MST, Tên, Địa chỉ...",
            Margin = new Thickness(0, 0, 0, 10)
        };
        searchBar.TextChanged += OnSearchTextChanged;

        // 2. CollectionView hiển thị dạng danh sách đa cột
        _collectionView = new CollectionView
        {
            SelectionMode = SelectionMode.Single,
            ItemsSource = _allItems,
            ItemTemplate = new DataTemplate(() =>
            {
                // Dùng Grid để chia cột cho từng dòng dữ liệu
                var grid = new Grid
                {
                    Padding = 10,
                    RowDefinitions = { new RowDefinition { Height = GridLength.Auto } },
                    ColumnDefinitions =
                    {
                        new ColumnDefinition { Width = new GridLength(110) }, // Cột 1: Mã số thuế
                        new ColumnDefinition { Width = GridLength.Star }      // Cột 2: Tên & Địa chỉ
                    }
                };

                // Hiển thị Mã số thuế
                var lblMst = new Label
                {
                    FontAttributes = FontAttributes.Bold,
                    TextColor = Colors.DarkBlue,
                    VerticalOptions = LayoutOptions.Center
                };
                lblMst.SetBinding(Label.TextProperty, nameof(TaxAgencyItem.MaSoThue));

                // Hiển thị Tên cơ quan thuế + Địa chỉ
                var detailStack = new VerticalStackLayout { Spacing = 2 };

                var lblTen = new Label
                {
                    FontAttributes = FontAttributes.Bold,
                    FontSize = 14,
                    TextColor = Colors.Black
                };
                lblTen.SetBinding(Label.TextProperty, nameof(TaxAgencyItem.TenCoQuanThue));

                var lblDiaChi = new Label
                {
                    FontSize = 12,
                    TextColor = Colors.Gray
                };
                lblDiaChi.SetBinding(Label.TextProperty, nameof(TaxAgencyItem.DiaChi));

                detailStack.Add(lblTen);
                detailStack.Add(lblDiaChi);

                grid.Add(lblMst, 0, 0);
                grid.Add(detailStack, 1, 0);

                return grid;
            })
        };

        // Bắt sự kiện chọn dòng trên Grid
        _collectionView.SelectionChanged += OnSelectionChanged;

        // 3. Nút Đóng Popup
        var btnClose = new Button
        {
            Text = "ĐÓNG CỬA SỔ",
            BackgroundColor = Colors.Gray,
            TextColor = Colors.White,
            HeightRequest = 44,
            Margin = new Thickness(0, 10, 0, 0)
        };
        btnClose.Clicked += async (s, e) => await Navigation.PopModalAsync();

        // Đóng gói giao diện
        Content = new Grid
        {
            Padding = 12,
            RowDefinitions =
            {
                new RowDefinition { Height = GridLength.Auto }, // Dòng 0: SearchBar
                new RowDefinition { Height = GridLength.Star }, // Dòng 1: CollectionView
                new RowDefinition { Height = GridLength.Auto }  // Dòng 2: Button Close
            },
                    Children =
            {
                searchBar,
                _collectionView,
                btnClose
            }
        };

    }

    // Xử lý Lọc đa cột khi người dùng gõ vào SearchBar
    private void OnSearchTextChanged(object? sender, TextChangedEventArgs e)
    {
        string keyword = e.NewTextValue?.ToLower().Trim() ?? "";

        if (string.IsNullOrWhiteSpace(keyword))
        {
            _collectionView.ItemsSource = _allItems;
        }
        else
        {
            // Lọc đồng thời trên MST, Tên Cơ Quan Thuế và Địa Chỉ
            _collectionView.ItemsSource = _allItems.Where(x =>
                x.MaSoThue.ToLower().Contains(keyword) ||
                x.TenCoQuanThue.ToLower().Contains(keyword) ||
                x.DiaChi.ToLower().Contains(keyword)
            ).ToList();
        }
    }

    // Khi chạm chọn 1 dòng -> Bắn sự kiện và Đóng Modal Popup
    private async void OnSelectionChanged(object? sender, SelectionChangedEventArgs e)
    {
        if (e.CurrentSelection.FirstOrDefault() is TaxAgencyItem selected)
        {
            OnItemSelected?.Invoke(selected);
            await Navigation.PopModalAsync(); // Đóng Popup
        }
    }
}