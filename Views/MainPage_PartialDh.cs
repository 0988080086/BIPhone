using System.ComponentModel;
using System.Data;
using System.Runtime.CompilerServices;

namespace BIPhone.Views;

public partial class MainPage
{
    private List<DonHangModel> _allDonHang = new();
    private int _currentFilterTrangThai = 0;
    private bool _isDonHangEventsInitialized = false;

    /// <summary>
    /// Hàm khởi tạo sự kiện - Đảm bảo chỉ đăng ký 1 lần duy nhất
    /// </summary>
    public void InitTabDonHangEvents()
    {
        if (_isDonHangEventsInitialized) return;
        _isDonHangEventsInitialized = true;

        BtnFilterTatCa.Clicked += (s, e) => OnFilterChipClicked(0, BtnFilterTatCa);
        BtnFilterChoLay.Clicked += (s, e) => OnFilterChipClicked(1, BtnFilterChoLay);
        BtnFilterDangGiao.Clicked += (s, e) => OnFilterChipClicked(2, BtnFilterDangGiao);
        BtnFilterDaGiao.Clicked += (s, e) => OnFilterChipClicked(3, BtnFilterDaGiao);
        BtnFilterDaHuy.Clicked += (s, e) => OnFilterChipClicked(4, BtnFilterDaHuy);

        EntrySearchDonHang.TextChanged += OnSearchDonHangTextChanged;
    }

    public async Task LoadDanhSachDonHangAsync()
    {
        try
        {
            ActDonHangLoading.IsVisible = true;
            ActDonHangLoading.IsRunning = true;


            //NghiepVuID,GhID,GhSo,CtLoaiID,NgayLapPhieu,DtID,DtMa,DtTen,DtDiaChi,DtDienThoai,BatDau,KetThuc,TinhTrang,ThanhToan,TrangThai,NgayCn,
            //SoLuong,DonGiaSauBh,SoLuong_A,SoLuong_B,SoLuong_C,ThanhToanSauBh,DuongDan,TenTep,FileSize
            //Lấy dữ liệu mới nhất
            DataTable _Tbl; string _LastMessage; DonHangModel _item;
            (_Tbl, _LastMessage) = await ClsConnService.Instance.MauiDeliveryAsync();
            if (_Tbl != null && _Tbl.Rows.Count > 0)
            {
                _allDonHang = new List<DonHangModel>();

                // Dùng foreach để lấy trọn vẹn tất cả các dòng (tránh sót dòng cuối)
                foreach (DataRow aRow in _Tbl.Rows)
                {
                    // Ghép URL đường dẫn hình ảnh an toàn
                    string duongDan = aRow["DuongDan"]?.ToString() ?? "";
                    string tenTep = aRow["TenTep"]?.ToString() ?? "";
                    string fullImageUrl = !string.IsNullOrEmpty(tenTep) ? $"{duongDan.TrimEnd('/')}/{tenTep.TrimStart('/')}" : "placeholder_image.png";

                    var item = new DonHangModel
                    {
                        MaKhachHang = aRow["DtMa"]?.ToString() ?? "",
                        TenKhachHang = aRow["DtTen"]?.ToString() ?? "",

                        // Kiểm tra linh hoạt cột tên hàng hóa
                        TenSanPhamDaiDien = _Tbl.Columns.Contains("HhTen") ? aRow["HhTen"]?.ToString() : (_Tbl.Columns.Contains("Ten") ? aRow["Ten"]?.ToString() : ""),
                        AnhSanPhamDaiDien = fullImageUrl,
                        // Convert.ToDouble an toàn cho float/double từ SQL
                        TongTien = aRow["ThanhToanSauBh"] != DBNull.Value ? Convert.ToDouble(aRow["ThanhToanSauBh"]): 0,
                        DaThu = 0,
                        TenNhanVienGiao = "UserName",
                        // Convert.ToInt32 an toàn cho decimal/tinyint từ SQL
                        TrangThaiId = aRow["TrangThai"] != DBNull.Value ? Convert.ToInt32(aRow["TrangThai"]) : 1
                    };

                    _allDonHang.Add(item);
                }
            }
            else
            {
                // Mẫu dữ liệu Mockup nếu không có Data
                if (_allDonHang == null || _allDonHang.Count == 0)
                {
                    _allDonHang = new List<DonHangModel>
                {
                    new DonHangModel
                    {
                        MaKhachHang = "KH001",
                        TenKhachHang = "Nguyễn Văn A",
                        TenSanPhamDaiDien = "iPhone 15 Pro Max 256GB Gold",
                        TongTien = 30000000,
                        DaThu = 30000000,
                        TenNhanVienGiao = "Shipper Nam",
                        TrangThaiId = 1
                    },
                    new DonHangModel
                    {
                        MaKhachHang = "KH002",
                        TenKhachHang = "Trần Thị B",
                        TenSanPhamDaiDien = "Samsung Galaxy S24 Ultra 512GB",
                        TongTien = 25000000,
                        DaThu = 10000000,
                        TenNhanVienGiao = "Shipper Tuấn",
                        TrangThaiId = 2
                    }
                };
                }
            };
            ApplyFilterAndCalculateSummary();
        }
        catch (Exception ex)
        {
            await DisplayAlert("Lỗi", "Không thể tải danh sách đơn hàng: " + ex.Message, "OK");
        }
        finally
        {
            ActDonHangLoading.IsRunning = false;
            ActDonHangLoading.IsVisible = false;
        }
    }

    private void ApplyFilterAndCalculateSummary()
    {
        string keyword = EntrySearchDonHang?.Text?.Trim().ToLower() ?? "";

        // Lọc kết hợp cả Trạng thái + Từ khóa Tìm kiếm
        var filteredList = _allDonHang.Where(x =>
            (_currentFilterTrangThai == 0 || x.TrangThaiId == _currentFilterTrangThai) &&
            (string.IsNullOrEmpty(keyword) ||
             (x.MaKhachHang?.ToLower().Contains(keyword) ?? false) ||
             (x.TenKhachHang?.ToLower().Contains(keyword) ?? false) ||
             (x.TenSanPhamDaiDien?.ToLower().Contains(keyword) ?? false))
        ).ToList();

        GridOrderContent.ItemsSource = filteredList;

        // Cập nhật Footer
        var summary = new ThongKeDonHangModel
        {
            TongSoDon = _allDonHang.Count,
            ChoLay = _allDonHang.Count(x => x.TrangThaiId == 1),
            DangGiao = _allDonHang.Count(x => x.TrangThaiId == 2),
            DaGiao = _allDonHang.Count(x => x.TrangThaiId == 3),
            DaHuy = _allDonHang.Count(x => x.TrangThaiId == 4),
            TongTienHang = filteredList.Sum(x => x.TongTien),
            TongDaThu = filteredList.Sum(x => x.DaThu)
        };

        // Gán BindingContext an toàn cho dòng Footer (Row 3 trong Grid)
        var footerGrid = GridFooterThongKe.Children.FirstOrDefault(c => Grid.GetRow((BindableObject)c) == 3) as Layout;
        if (footerGrid != null)
        {
            footerGrid.BindingContext = summary;
        }
    }

    private void OnSearchDonHangTextChanged(object sender, TextChangedEventArgs e)
    {
        ApplyFilterAndCalculateSummary();
    }

    private void OnFilterChipClicked(int statusId, Button selectedBtn)
    {
        _currentFilterTrangThai = statusId;

        Color normalBg = Color.FromArgb("#F1F5F9");
        Color normalText = Color.FromArgb("#475569");

        BtnFilterTatCa.BackgroundColor = normalBg; BtnFilterTatCa.TextColor = normalText;
        BtnFilterChoLay.BackgroundColor = normalBg; BtnFilterChoLay.TextColor = normalText;
        BtnFilterDangGiao.BackgroundColor = normalBg; BtnFilterDangGiao.TextColor = normalText;
        BtnFilterDaGiao.BackgroundColor = normalBg; BtnFilterDaGiao.TextColor = normalText;
        BtnFilterDaHuy.BackgroundColor = normalBg; BtnFilterDaHuy.TextColor = normalText;

        selectedBtn.BackgroundColor = Color.FromArgb("#1E3A8A");
        selectedBtn.TextColor = Colors.White;

        ApplyFilterAndCalculateSummary();
    }

    #region --- SỰ KIỆN NÚT BẤM CỤ THỂ ---

    private async void BtnTiepNhan_Clicked(object sender, EventArgs e)
    {
        if (sender is Button btn && btn.BindingContext is DonHangModel donHang)
        {
            bool confirm = await DisplayAlert("Xác nhận", $"Tiếp nhận đơn hàng của {donHang.TenKhachHang}?", "Đồng ý", "Hủy");
            if (!confirm) return;

            // Chuyển sang trạng thái Đang giao
            donHang.TrangThaiId = 2;
            donHang.TenTrangThai = "Đang giao";

            ApplyFilterAndCalculateSummary(); // Cập nhật lại danh sách & thống kê
        }
    }

    private async void BtnHuyTiepNhan_Clicked(object sender, EventArgs e)
    {
        if (sender is Button btn && btn.BindingContext is DonHangModel donHang)
        {
            bool confirm = await DisplayAlert("Xác nhận", $"Hủy tiếp nhận đơn hàng của {donHang.TenKhachHang}?", "Đồng ý", "Hủy");
            if (!confirm) return;

            donHang.TrangThaiId = 1;
            donHang.TenTrangThai = "Chờ lấy hàng";

            ApplyFilterAndCalculateSummary();
        }
    }

    private async void BtnSerial_Clicked(object sender, EventArgs e)
    {
        if (sender is Button btn && btn.BindingContext is DonHangModel donHang)
        {
            string serial = await DisplayPromptAsync("Số Serial/IMEI", $"Nhập Serial cho sản phẩm {donHang.TenSanPhamDaiDien}:", "Lưu", "Hủy");
            if (!string.IsNullOrWhiteSpace(serial))
            {
                await DisplayAlert("Thông báo", $"Đã lưu Serial: {serial}", "OK");
            }
        }
    }

    private async void BtnDaGiao_Clicked(object sender, EventArgs e)
    {
        if (sender is Button btn && btn.BindingContext is DonHangModel donHang)
        {
            bool confirm = await DisplayAlert("Xác nhận", $"Đánh dấu đơn hàng của {donHang.TenKhachHang} là ĐÃ GIAO?", "Đồng ý", "Hủy");
            if (!confirm) return;

            donHang.TrangThaiId = 3;
            donHang.TenTrangThai = "Đã giao";

            ApplyFilterAndCalculateSummary();
        }
    }

    private async void BtnHuyDon_Clicked(object sender, EventArgs e)
    {
        if (sender is Button btn && btn.BindingContext is DonHangModel donHang)
        {
            string lyDo = await DisplayPromptAsync("Hủy Đơn", "Nhập lý do hủy đơn hàng:", "Hủy đơn", "Quay lại");
            if (lyDo != null)
            {
                donHang.TrangThaiId = 4;
                donHang.TenTrangThai = "Đã hủy";

                ApplyFilterAndCalculateSummary();
            }
        }
    }

    #endregion
}

public class DonHangModel : INotifyPropertyChanged
{
    private int _trangThaiId;
    private string _tenTrangThai;

    public string MaKhachHang { get; set; }
    public string TenKhachHang { get; set; }
    public string AnhSanPhamDaiDien { get; set; }
    public string TenSanPhamDaiDien { get; set; }
    public string MoTaThemSanPham { get; set; }
    public bool HasMoTaThem => !string.IsNullOrEmpty(MoTaThemSanPham);

    public double TongTien { get; set; }
    public decimal DaThu { get; set; }
    public string TenNhanVienGiao { get; set; }

    // Trạng thái đơn: 1-Chờ lấy, 2-Đang giao, 3-Đã giao, 4-Đã hủy
    public int TrangThaiId
    {
        get => _trangThaiId;
        set
        {
            if (_trangThaiId != value)
            {
                _trangThaiId = value;
                OnPropertyChanged();
                // Thông báo cho UI vẽ lại các nút bấm tương ứng
                OnPropertyChanged(nameof(CanTiepNhan));
                OnPropertyChanged(nameof(CanHuyTiepNhan));
                OnPropertyChanged(nameof(CanNhapSerial));
                OnPropertyChanged(nameof(CanDanhDauDaGiao));
                OnPropertyChanged(nameof(CanHuyDon));
            }
        }
    }

    public string TenTrangThai
    {
        get => _tenTrangThai;
        set { _tenTrangThai = value; OnPropertyChanged(); }
    }

    // --- CÁC CỜ ĐIỀU KHIỂN ẨN/HIỆN NÚT BẤM ---
    public bool CanTiepNhan => TrangThaiId == 1;
    public bool CanHuyTiepNhan => TrangThaiId == 2;
    public bool CanNhapSerial => TrangThaiId == 2;
    public bool CanDanhDauDaGiao => TrangThaiId == 2;
    public bool CanHuyDon => TrangThaiId == 1 || TrangThaiId == 2;

    public string TongTienFormatted => TongTien.ToString("N0") + "đ";
    public string DaThuFormatted => DaThu.ToString("N0") + "đ";

    public event PropertyChangedEventHandler PropertyChanged;
    protected void OnPropertyChanged([CallerMemberName] string propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}
public class ThongKeDonHangModel
{
    public int TongSoDon { get; set; }
    public int ChoLay { get; set; }
    public int DangGiao { get; set; }
    public int DaGiao { get; set; }
    public int DaHuy { get; set; }
    public double TongTienHang { get; set; }
    public decimal TongDaThu { get; set; }

    // Chuỗi hiển thị dòng tổng số đơn
    public string ThongKeSoLuongFormatted =>
        $"Tổng: {TongSoDon} (Chờ lấy: {ChoLay} | Đang giao: {DangGiao} | Đã giao: {DaGiao} | Hủy: {DaHuy})";

    public string TongTienHangFormatted => TongTienHang.ToString("N0") + "đ";
    public string TongDaThuFormatted => TongDaThu.ToString("N0") + "đ";
}