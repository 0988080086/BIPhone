using System.Collections.ObjectModel;
using System.Data;

namespace BIPhone.Views;

public partial class MainPage
{
    // =====================================================
    // KHAI BÁO BIẾN & THUỘC TÍNH PHỤ TRÁCH CUỘC GỌI
    // =====================================================
    private static bool _DtSyncning = false;

    // Collection quản lý danh sách cuộc gọi trên UI
    public ObservableCollection<CrmDienThoaiItem> CallList { get; set; } = new ObservableCollection<CrmDienThoaiItem>();

    // =====================================================
    // XỬ LÝ DỮ LIỆU & REALTIME CUỘC GỌI
    // =====================================================

    /// <summary>
    /// Đọc dữ liệu Top 100 cuộc gọi gần nhất từ SQLite
    /// </summary>
    private void LoadDataCallsFromSQLite()
    {
        try
        {
            var items = CrmDienThoai.Instance.GetTop100Desc();

            MainThread.BeginInvokeOnMainThread(() =>
            {
                CallList.Clear();
                if (items != null)
                {
                    foreach (var item in items)
                    {
                        CallList.Add(item);
                    }
                }
            });
        }
        catch (Exception ex)
        {
            LogWriter.WriteLine($"Lỗi LoadDataCallsFromSQLite: {ex.Message}");
        }
    }

    /// <summary>
    /// Hàm chèn/sửa dữ liệu cuộc gọi trực tiếp vào UI siêu tốc (Realtime)
    /// </summary>
    private void UpdateCallListRealtime(CrmDienThoaiItem _Item)
    {
        if (_Item == null || string.IsNullOrWhiteSpace(_Item.UID)) return;

        MainThread.BeginInvokeOnMainThread(async () =>
        {
            var existingItem = CallList.FirstOrDefault(x => x.UID == _Item.UID);

            if (existingItem != null)
            {
                int index = CallList.IndexOf(existingItem);
                if (index != -1)
                {
                    CallList.RemoveAt(index);
                    CallList.Insert(index, _Item);
                }
            }
            else
            {
                int insertIndex = 0;
                while (insertIndex < CallList.Count)
                {
                    string currentUid = CallList[insertIndex].UID ?? string.Empty;
                    if (string.Compare(_Item.UID, currentUid, StringComparison.Ordinal) > 0)
                    {
                        break;
                    }
                    insertIndex++;
                }

                CallList.Insert(insertIndex, _Item);

                if (insertIndex == 0)
                {
                    await Task.Delay(100);

                    if (CallList.Count > 0)
                    {
                        cvCalls.ScrollTo(0, position: ScrollToPosition.Start, animate: true);
                    }
                }
            }
        });
    }

    /// <summary>
    /// Lấy tổng số lượng khách hàng từ SQLite
    /// </summary>
    private void LoadTotalKhachHangCount()
    {
        try
        {
            long count = DoiTuong.Instance.GetTotalCount();
            lblTongKhachHang.Text = $"Tổng KH: {count:N0}";
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"Lỗi LoadTotalKhachHangCount: {ex.Message}");
            lblTongKhachHang.Text = "Tổng KH: 0";
        }
    }

    // =====================================================
    // SỰ KIỆN GIAO DIỆN TAB CUỘC GỌI (XAML CALL TAB EVENTS)
    // =====================================================

    // 1. Vuốt xuống để Refresh danh sách cuộc gọi
    private void OnRefreshingCalls(object sender, EventArgs e)
    {
        refViewCalls.IsRefreshing = true;

        LoadDataCallsFromSQLite();
        LoadTotalKhachHangCount();

        refViewCalls.IsRefreshing = false;

        MainThread.BeginInvokeOnMainThread(async () =>
        {
            await Task.Delay(50);
            cvCalls.ScrollTo(0, position: ScrollToPosition.Start, animate: true);
        });
    }

    // 2. Đồng bộ khách hàng từ API về SQLite
    private async void OnDongBoKhachHangClicked(object sender, EventArgs e)
    {
        if (_DtSyncning) return;
        _DtSyncning = true;
        mConnService = ClsConnService.Instance;
        int mCount = 0; DataTable mTblKh = null; int mPage = 0;
        string _LastMessage = "";
        try
        {
            bool continueSync = true;
            while (continueSync)
            {
                mPage++;
                lblTongKhachHang.Text = $"Đang tải trang {mPage}...";                
                (mTblKh,_LastMessage) = await mConnService.MauiCustomerAsync();
                if (mTblKh != null && mTblKh.Rows.Count > 0)
                {
                    mCount += mTblKh.Rows.Count;
                    var listItems = new List<DoiTuongItem>();
                    foreach (DataRow row in mTblKh.Rows)
                    {
                        var dtItem = new DoiTuongItem();
                        if (dtItem.FromDataRow(row))
                        {
                            listItems.Add(dtItem);
                        }
                    }

                    foreach (var item in listItems)
                    {
                        DoiTuong.Instance.Save(item);
                    }
                }
                else
                {
                    continueSync = false;
                }
            }
            lblTongKhachHang.Text = $"Tải hoàn tất! ({mCount} khách hàng)";
        }
        catch (Exception ex)
        {
            LogWriter.WriteLine($"Lỗi đồng bộ khách hàng: {ex.Message}");
            lblTongKhachHang.Text = "Đồng bộ thất bại!";
        }
        finally
        {
            _DtSyncning = false;
            LoadTotalKhachHangCount();
        }
    }

    // 3. Nút "Gọi lại"
    private async void OnGoiLaiClicked(object sender, EventArgs e)
    {
        if (sender is Button btn && btn.CommandParameter is CrmDienThoaiItem item)
        {
            string phone = item.DienThoai?.Trim();

            if (string.IsNullOrEmpty(phone))
            {
                await DisplayAlert("Thông báo", "Số điện thoại rỗng!", "OK");
                return;
            }

            try
            {
                PhoneDialer.Default.Open(phone);
            }
            catch (FeatureNotSupportedException)
            {
                await DisplayAlert("Lỗi", "Thiết bị không hỗ trợ tính năng quay số gọi điện!", "OK");
            }
            catch (Exception ex)
            {
                await DisplayAlert("Lỗi", $"Không thể mở trình gọi điện: {ex.Message}", "OK");
            }
        }
    }

    // 4. Nút "Ghi chú"
    private async void OnGhiChuClicked(object sender, EventArgs e)
    {
        if (sender is Button btn && btn.CommandParameter is CrmDienThoaiItem item)
        {
            string note = await DisplayPromptAsync("Ghi chú", $"Nhập ghi chú cho UID {item.UID}:", initialValue: item.NoiDung);
            if (note != null)
            {
                item.NoiDung = note;
                await CrmDienThoai.Instance.Save(item);
            }
        }
    }

    // 5. Nút "Bán hàng"
    private async void OnBanHangClicked(object sender, EventArgs e)
    {
        if (sender is Button btn && btn.CommandParameter is CrmDienThoaiItem item)
        {
            await DisplayAlert("Bán hàng", $"Mở màn hình Bán hàng cho KH: {item.DtTen} ({item.DtMa})", "OK");
        }
    }

    // 6. Nút "Nghe lại"
    private async void OnNgheLaiClicked(object sender, EventArgs e)
    {
        if (sender is Button btn && btn.CommandParameter is CrmDienThoaiItem item)
        {
            await DisplayAlert("Nghe lại", $"Phát file ghi âm: {item.TepGhiAm}", "OK");
        }
    }

    // 7. Nút "Chia sẻ" thông tin khách hàng
    private async void OnChiaSeClicked(object sender, EventArgs e)
    {
        if (sender is Button btn && btn.CommandParameter is CrmDienThoaiItem item)
        {
            try
            {
                if (!string.IsNullOrEmpty(item.TepGhiAm) && System.IO.File.Exists(item.TepGhiAm))
                {
                    await Share.Default.RequestAsync(new ShareFileRequest
                    {
                        Title = $"Ghi âm cuộc gọi - {item.DienThoai}",
                        File = new ShareFile(item.TepGhiAm)
                    });
                }
                else
                {
                    string shareText = $"[Thông tin cuộc gọi]\n" +
                                       $"Số ĐT: {item.DienThoai}\n" +
                                       $"Thời gian: {item.BatDau}\n" +
                                       $"Khách hàng: {item.DtTen}\n" +
                                       $"Nội dung: {item.NoiDung}";

                    await Share.Default.RequestAsync(new ShareTextRequest
                    {
                        Title = "Chia sẻ cuộc gọi",
                        Text = shareText
                    });
                }
            }
            catch (Exception ex)
            {
                await DisplayAlert("Lỗi", $"Không thể chia sẻ: {ex.Message}", "OK");
            }
        }
    }
}