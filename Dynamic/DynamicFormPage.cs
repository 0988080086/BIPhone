using Microsoft.Maui.Layouts;
namespace BIPhone.Dynamic;
public class DynamicFormPage : ContentPage
{
    private readonly DynamicFeature _feature;
    // Map quản lý các Control để bóc tách dữ liệu khi Submit
    private readonly Dictionary<string, (View Control, FieldMetadata Meta)> _fieldMap = new();

    // Khai báo Action Callback để trả kết quả về cho Màn hình gọi
    public Action<Dictionary<string, object>>? OnSubmitted;

    public DynamicFormPage(DynamicFeature feature)
    {
        _feature = feature;
        Title = _feature.Title;
        BuildUI();
    }

    private void BuildUI()
    {
        var mainStack = new VerticalStackLayout { Padding = 16, Spacing = 12 };

        var flexContainer = new FlexLayout
        {
            Wrap = FlexWrap.Wrap,
            Direction = FlexDirection.Row,
            JustifyContent = FlexJustify.SpaceBetween
        };

        // Duyệt qua từng Field để vẽ Control tương ứng
        foreach (var field in _feature.Fields)
        {
            var fieldUI = RenderField(field);
            FlexLayout.SetBasis(fieldUI, new FlexBasis((float)(field.WidthPercent / 100.0), true));
            flexContainer.Children.Add(fieldUI);
        }

        mainStack.Add(flexContainer);

        // Nút Submit
        var btnSubmit = new Button
        {
            Text = "LƯU HỒ SƠ",
            BackgroundColor = Colors.DarkBlue,
            TextColor = Colors.White,
            HeightRequest = 48,
            Margin = new Thickness(0, 20, 0, 0)
        };
        btnSubmit.Clicked += OnSubmitClicked;
        mainStack.Add(btnSubmit);

        Content = new ScrollView { Content = mainStack };
    }

    private View RenderField(FieldMetadata field)
    {
        var box = new VerticalStackLayout { Padding = 4, Spacing = 4 };

        // Label Tiêu đề
        box.Add(new Label
        {
            Text = field.FieldName + (field.IsRequired ? " (*)" : ""),
            FontAttributes = FontAttributes.Bold,
            FontSize = 13
        });

        View inputControl;

        switch (field.ControlType)
        {
            case DynamicControlType.RadioGroup: // 1. Giới tính (Nam/Nữ)
                var radioStack = new HorizontalStackLayout { Spacing = 15 };
                foreach (var opt in field.StaticOptions ?? new())
                {
                    radioStack.Add(new RadioButton { Content = opt.Display, Value = opt.Key, GroupName = field.FieldCode });
                }
                inputControl = radioStack;
                break;

            case DynamicControlType.ComboListKey: // 2. Xã / Phường (ComboList - Lưu Key Decimal)
                var picker = new Picker { Title = $"-- Chọn {field.FieldName} --" };
                // Giả lập nạp danh sách từ SQL (Key Decimal, Display Text)
                var mockData = new List<OptionItem>
                {
                    new() { Key = 10023.0m, Display = "Phường Bách Khoa" },
                    new() { Key = 10024.0m, Display = "Phường Hàng Bài" }
                };
                picker.ItemsSource = mockData;
                picker.ItemDisplayBinding = new Binding("Display");
                inputControl = picker;
                break;

            case DynamicControlType.MultiColumnLookup: // 3. Cơ quan thuế / MST (Lookup Grid)
                var gridLookup = new Grid
                {
                    ColumnDefinitions =
        {
            new ColumnDefinition { Width = GridLength.Star },
            new ColumnDefinition { Width = GridLength.Auto }
        }
                };

                var entryLookup = new Entry
                {
                    Placeholder = "Bấm kính lúp để chọn cơ quan thuế...",
                    IsReadOnly = true
                };

                var btnSearch = new Button { Text = "🔍", WidthRequest = 50 };

                // 1. Định nghĩa hàm mở Popup độc lập
                async Task OpenPopupAsync()
                {
                    var mockTaxAgencies = new List<TaxAgencyItem>
        {
            new() { MaCqtDecimal = 1001.0m, MaSoThue = "0100100101", TenCoQuanThue = "Cục Thuế TP Hà Nội", DiaChi = "187 Giảng Võ, Đống Đa, Hà Nội" },
            new() { MaCqtDecimal = 1002.0m, MaSoThue = "0100100102", TenCoQuanThue = "Chi cục Thuế Quận Cầu Giấy", DiaChi = "68 Nguyễn Phong Sắc, Cầu Giấy, Hà Nội" },
            new() { MaCqtDecimal = 1003.0m, MaSoThue = "0100100103", TenCoQuanThue = "Chi cục Thuế Quận Hai Bà Trưng", DiaChi = "161 Triệu Việt Vương, Hà Nội" },
            new() { MaCqtDecimal = 1004.0m, MaSoThue = "0100100104", TenCoQuanThue = "Chi cục Thuế Quận Đống Đa", DiaChi = "185 Đặng Tiến Đông, Đống Đa, Hà Nội" }
        };

                    var popup = new TaxAgencyLookupPopupPage(mockTaxAgencies);

                    popup.OnItemSelected = (selectedItem) =>
                    {
                        entryLookup.Text = selectedItem.TenCoQuanThue;
                        entryLookup.ClassId = selectedItem.MaCqtDecimal.ToString();
                    };

                    await Navigation.PushModalAsync(popup);
                }

                // 2. Bắt sự kiện Click của Button
                btnSearch.Clicked += async (s, e) => await OpenPopupAsync();

                // 3. Bắt sự kiện Tap của Entry (Đã hết lỗi CS0029)
                var tapGesture = new TapGestureRecognizer();
                tapGesture.Tapped += async (s, e) => await OpenPopupAsync();
                entryLookup.GestureRecognizers.Add(tapGesture);

                gridLookup.Add(entryLookup, 0, 0);
                gridLookup.Add(btnSearch, 1, 0);
                inputControl = gridLookup;
                break;

            //var gridLookup = new Grid { ColumnDefinitions = { new ColumnDefinition { Width = GridLength.Star }, new ColumnDefinition { Width = GridLength.Auto } } };
            //var entryLookup = new Entry { Placeholder = "Bấm kính lúp để chọn cơ quan thuế...", IsReadOnly = true };
            //var btnSearch = new Button { Text = "🔍" };

            //btnSearch.Clicked += async (s, e) =>
            //{
            //    // Mô phỏng mở Popup Grid chọn Cơ quan thuế
            //    await DisplayAlert("Lookup", "Mở Grid tìm kiếm nhiều cột (MST, Tên, Địa chỉ, Cán bộ)", "OK");
            //    // Giả lập người dùng chọn dòng có Key Decimal = 88392.0m
            //    entryLookup.Text = "Cục Thuế TP Hà Nội";
            //    entryLookup.ClassId = "88392.0"; // Lưu gá tạm KeyDecimal vào ClassId
            //};

            //gridLookup.Add(entryLookup, 0, 0);
            //gridLookup.Add(btnSearch, 1, 0);
            //inputControl = gridLookup;
            //break;

            case DynamicControlType.MultilineText: // 4. Ý kiến cá nhân (Co giãn chiều cao)
                inputControl = new Editor { AutoSize = EditorAutoSizeOption.TextChanges, MinimumHeightRequest = 80 };
                break;

            default: // Textbox thông thường hoặc EditableCombobox
                inputControl = new Entry { Placeholder = $"Nhập {field.FieldName}..." };
                break;
        }

        _fieldMap[field.FieldCode] = (inputControl, field);
        box.Add(inputControl);
        return box;
    }

    private async void OnSubmitClicked(object? sender, EventArgs e)
    {
        var resultData = new Dictionary<string, object>();

        // Thu thập và bóc tách dữ liệu đúng định dạng để Submit
        foreach (var (fieldCode, (control, meta)) in _fieldMap)
        {
            object? val = null;

            if (control is Entry entry)
            {
                val = entry.Text;
            }
            else if (control is HorizontalStackLayout radioStack) // RadioGroup
            {
                var checkedRadio = radioStack.Children.OfType<RadioButton>().FirstOrDefault(r => r.IsChecked);
                val = checkedRadio?.Value; // Lấy Key (1 hoặc 2)
            }
            else if (control is Picker picker) // ComboList Key
            {
                if (picker.SelectedItem is OptionItem opt)
                    val = opt.Key; // Lấy Key Decimal (VD: 10023.0)
            }
            else if (control is Grid lookupGrid && lookupGrid.Children[0] is Entry lookupEntry) // MultiColumn Lookup
            {
                if (!string.IsNullOrEmpty(lookupEntry.ClassId))
                    val = Convert.ToDecimal(lookupEntry.ClassId); // Lấy Key Decimal
            }
            else if (control is Editor editor) // Multiline
            {
                val = editor.Text;
            }

            if (val != null) resultData[fieldCode] = val;
        }

        // Báo thành công và bắn dữ liệu về qua Callback
        OnSubmitted?.Invoke(resultData);
        await Navigation.PopAsync();
    }
}

//=============================================================================
//Cách sử dụng DynamicFormPage
//=============================================================================
//private async void ButtonTest_Clicked(object sender, EventArgs e)
//{
//    OpenHoSoLaoDongPage();
//    return;
//}
//private async void OpenHoSoLaoDongPage()
//{
//    // 1. Khai báo Cấu hình Tính năng "HoSoLaoDong" (Thực tế JSON này tải từ WebService API)
//    var featureHoSoLaoDong = new DynamicFeature
//    {
//        FeatureId = "HoSoLaoDong",
//        Title = "Khai Báo Hồ Sơ Lao Động",
//        Fields = new List<FieldMetadata>
//    {
//        new() { FieldCode = "HoTen", FieldName = "Họ và Tên", ControlType = DynamicControlType.Text, WidthPercent = 100, IsRequired = true },
//        new() { FieldCode = "GioiTinh", FieldName = "Giới tính", ControlType = DynamicControlType.RadioGroup, WidthPercent = 100,
//                StaticOptions = new() { new() { Key = 1, Display = "Nam" }, new() { Key = 2, Display = "Nữ" } } },
//        new() { FieldCode = "ChucVu", FieldName = "Chức vụ", ControlType = DynamicControlType.EditableCombobox, WidthPercent = 100 },
//        new() { FieldCode = "MaXaPhuong", FieldName = "Xã / Phường", ControlType = DynamicControlType.ComboListKey, WidthPercent = 100, ValueMember = "KeyDecimal" },
//        new() { FieldCode = "MaCoQuanThue", FieldName = "Cơ quan quản lý thuế", ControlType = DynamicControlType.MultiColumnLookup, WidthPercent = 100, ValueMember = "KeyDecimal" },
//        new() { FieldCode = "YKienCaNhan", FieldName = "Ý kiến cá nhân", ControlType = DynamicControlType.MultilineText, WidthPercent = 100 }
//    }
//    };

//    // 2. Khởi tạo DynamicFormPage
//    var dynamicPage = new DynamicFormPage(featureHoSoLaoDong);

//    // 3. Đăng ký nhận Kết quả sau khi Người dùng bấm "LƯU HỒ SƠ"
//    dynamicPage.OnSubmitted = async (Dictionary<string, object> submittedData) =>
//    {
//        // Chuyển kết quả thu được thành JSON chuẩn bị đẩy lên WebService
//        string jsonSubmit = System.Text.Json.JsonSerializer.Serialize(submittedData);

//        /* Kết quả nhận được sẽ có dạng chuẩn Key-Value như sau:
//        {
//            "HoTen": "Nguyễn Văn A",
//            "GioiTinh": 1,
//            "ChucVu": "Trưởng phòng Kế toán",
//            "MaXaPhuong": 10023.0,          <-- Đã chuyển thành Key Decimal
//            "MaCoQuanThue": 88392.0,        <-- Đã chuyển thành Key Decimal
//            "YKienCaNhan": "Mong muốn đăng ký giảm trừ gia cảnh."
//        }
//        */

//        // Gọi WebService API đẩy jsonSubmit lên Server SQL
//        await DisplayAlert("KẾT QUẢ RETURN", "Dữ liệu đóng gói sẵn sàng đẩy API:\n" + jsonSubmit, "OK");
//    };

//    // 4. Mở Màn hình
//    //await Navigation.PushAsync(dynamicPage);
//    //Shell.Current.Navigation.PushAsync(dynamicPage);
//    await Navigation.PushAsync(dynamicPage);
//}
//=============================================================================