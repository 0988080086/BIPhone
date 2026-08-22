using Microsoft.Maui.Storage;

namespace BIPhone;

public static class AppSettings
{
    #region Private Core Methods (Lưu/Đọc Storage nội bộ)
    /// <summary>Hàm private dùng chung để lưu mọi kiểu dữ liệu vào Storage</summary>
    private static void Set<T>(string key, T value)
    {
        if (value == null)
        {
            Preferences.Default.Remove(key);
            return;
        }

        switch (value)
        {
            case string s:
                Preferences.Default.Set(key, s);
                break;
            case bool b:
                Preferences.Default.Set(key, b);
                break;
            case int i: // Bổ sung int
                Preferences.Default.Set(key, i);
                break;
            case long l:
                Preferences.Default.Set(key, l);
                break;
            case double d:
                Preferences.Default.Set(key, d);
                break;
            case DateTime dt:
                Preferences.Default.Set(key, dt);
                break;
            default:
                // Với kiểu object phức tạp, chuyển sang JSON rồi lưu
                string json = System.Text.Json.JsonSerializer.Serialize(value);
                Preferences.Default.Set(key, json);
                break;
        }
    }

    private static T Get<T>(string key, T defaultValue = default!)
    {
        if (!Preferences.Default.ContainsKey(key))
            return defaultValue;

        try
        {
            Type type = typeof(T);

            if (type == typeof(string))
                return (T)(object)Preferences.Default.Get(key, (string)(object)defaultValue!);

            if (type == typeof(bool))
                return (T)(object)Preferences.Default.Get(key, (bool)(object)defaultValue!);

            if (type == typeof(int)) // Bổ sung int
                return (T)(object)Preferences.Default.Get(key, (int)(object)defaultValue!);

            if (type == typeof(long))
                return (T)(object)Preferences.Default.Get(key, (long)(object)defaultValue!);

            if (type == typeof(double))
                return (T)(object)Preferences.Default.Get(key, (double)(object)defaultValue!);

            if (type == typeof(DateTime))
                return (T)(object)Preferences.Default.Get(key, (DateTime)(object)defaultValue!);

            // Deserialize JSON nếu là Object
            string json = Preferences.Default.Get(key, string.Empty);
            if (string.IsNullOrEmpty(json)) return defaultValue;

            return System.Text.Json.JsonSerializer.Deserialize<T>(json) ?? defaultValue;
        }
        catch
        {
            // Tránh crash ứng dụng nếu dữ liệu cũ lưu sai định dạng kiểu
            return defaultValue;
        }
    }

    /// <summary>Xóa key</summary>
    private static void Remove(string key)
    {
        Preferences.Default.Remove(key);
    }
    #endregion

    /// <summary>
    /// Xóa sạch mọi Cấu hình / Data lưu trong App (Ví dụ khi Đăng xuất)
    /// </summary>
    public static void ClearAllSettings()
    {
        Preferences.Default.Clear();
    }
    public static void LoginReset()
    {
        Remove(nameof(LoginRememberAccount));
        Remove(nameof(LoginRememberAutoLogin));
    }
    public static long ServerID
    {
        get => Get<long>(nameof(ServerID), 1L);
        set => Set(nameof(ServerID), value);
    }
    public static string ServerCpuID
    {
        get => Get(nameof(ServerCpuID), "");
        set => Set(nameof(ServerCpuID), value);
    }
    public static string PathData
    {
        get => Get(nameof(PathData), "");
        set => Set(nameof(PathData), value);
    }
    public static string PathRoot
    {
        get => Get(nameof(PathRoot), "");
        set => Set(nameof(PathRoot), value);
    }
    public static string PathHost
    {
        get => Get(nameof(PathHost), "");
        set => Set(nameof(PathHost), value);
    }

    public static bool IsCallScreeningEnabled
    {
        get => Get(nameof(IsCallScreeningEnabled), false);
        set => Set(nameof(IsCallScreeningEnabled), value);
    }

    public static bool CloseCallPopupWhenCallIDLE
    {
        get => Get(nameof(CloseCallPopupWhenCallIDLE), false);
        set => Set(nameof(CloseCallPopupWhenCallIDLE), value);
    }

    public static long LastCallID
    {
        get => Get<long>(nameof(LastCallID), long.MinValue);
        set => Set(nameof(LastCallID), value);
    }

    public static long TopMarginPercent
    {
        get => Get<long>(nameof(TopMarginPercent), 25L);
        set => Set(nameof(TopMarginPercent), value);
    }
    public static long ButtonMarginPecent
    {
        get => Get<long>(nameof(ButtonMarginPecent), 5L);
        set => Set(nameof(ButtonMarginPecent), value);
    }

    public static string DeviceID
    {
        get => Get(nameof(DeviceID), "");
        set => Set(nameof(DeviceID), value);
    }

    public static string DeviceRecord
    {
        get => Get(nameof(DeviceRecord), "");
        set => Set(nameof(DeviceRecord), value);
    }

    public static string Platform
    {
        get => Get(nameof(Platform), "");
        set => Set(nameof(Platform), value);
    }
    public static string PhoneToken
    {
        get => Get(nameof(PhoneToken), "");
        set => Set(nameof(PhoneToken), value);
    }

    public static long AppType
    {
        get => Get<long>(nameof(AppType), 1L);
        set => Set(nameof(AppType), value);
    }

    public static long DoiTuongSync_DtID
    {
        get => Get<long>(nameof(DoiTuongSync_DtID), 0L);
        set => Set(nameof(DoiTuongSync_DtID), value);
    }
    public static double DoiTuongSync_NgayCn
    {
        get => Get(nameof(DoiTuongSync_NgayCn), 0.0);
        set => Set(nameof(DoiTuongSync_NgayCn), value);
    }

    public static long ChoPhep_NhanDienDt_Den
    {
        get => Get<long>(nameof(ChoPhep_NhanDienDt_Den), 0L);
        set => Set(nameof(ChoPhep_NhanDienDt_Den), value);
    }
    public static long ChoPhep_NhanDienDt_HienPopup
    {
        get => Get<long>(nameof(ChoPhep_NhanDienDt_HienPopup), 0L);
        set => Set(nameof(ChoPhep_NhanDienDt_HienPopup), value);
    }
    public static long ChoPhep_NhanDienDt_Di
    {
        get => Get<long>(nameof(ChoPhep_NhanDienDt_Di), 0L);
        set => Set(nameof(ChoPhep_NhanDienDt_Di), value);
    }
    public static long ChoPhep_GhiAmDt
    {
        get => Get<long>(nameof(ChoPhep_GhiAmDt), 0L);
        set => Set(nameof(ChoPhep_GhiAmDt), value);
    }
    public static long ChoPhep_GuiTinSms
    {
        get => Get<long>(nameof(ChoPhep_GuiTinSms), 0L);
        set => Set(nameof(ChoPhep_GuiTinSms), value);
    }
    public static long ChoPhep_LuuTruGps
    {
        get => Get<long>(nameof(ChoPhep_LuuTruGps), 0L);
        set => Set(nameof(ChoPhep_LuuTruGps), value);
    }
    public static long ChoPhep_TimKiemKh
    {
        get => Get<long>(nameof(ChoPhep_TimKiemKh), 0L);
        set => Set(nameof(ChoPhep_TimKiemKh), value);
    }
    public static long ChoPhep_ThemMoiKh
    {
        get => Get<long>(nameof(ChoPhep_ThemMoiKh), 0L);
        set => Set(nameof(ChoPhep_ThemMoiKh), value);
    }
    public static long ChoPhep_SuaKh
    {
        get => Get<long>(nameof(ChoPhep_SuaKh), 0L);
        set => Set(nameof(ChoPhep_SuaKh), value);
    }
    public static long ChoPhep_TaoChungTu
    {
        get => Get<long>(nameof(ChoPhep_TaoChungTu), 0L);
        set => Set(nameof(ChoPhep_TaoChungTu), value);
    }
    public static long ChoPhep_SuaChungTu
    {
        get => Get<long>(nameof(ChoPhep_SuaChungTu), 0L);
        set => Set(nameof(ChoPhep_SuaChungTu), value);
    }
    public static long ChoPhep_XemGpsNhanVien
    {
        get => Get<long>(nameof(ChoPhep_XemGpsNhanVien), 0L);
        set => Set(nameof(ChoPhep_XemGpsNhanVien), value);
    }
    public static bool LoginRememberAccount
    {
        get => Get(nameof(LoginRememberAccount), false);
        set => Set(nameof(LoginRememberAccount), value);
    }
    public static bool LoginRememberAutoLogin
    {
        get => Get(nameof(LoginRememberAutoLogin), false);
        set => Set(nameof(LoginRememberAutoLogin), value);
    }
    public static long ChoPhep_DocThongBao
    {
        get => Get<long>(nameof(ChoPhep_DocThongBao), 1L);
        set => Set(nameof(ChoPhep_DocThongBao), value);
    }
    public static string RedirectUrl
    {
        get => Get(nameof(RedirectUrl), "");
        set => Set(nameof(RedirectUrl), value);
    }
    public static string RedirectUrl2
    {
        get => Get(nameof(RedirectUrl2), "");
        set => Set(nameof(RedirectUrl2), value);
    }
    public static int SoapTimeOut
    {
        get => Get(nameof(SoapTimeOut), 30);
        set => Set(nameof(SoapTimeOut), value);
    }

    public static string RedirectCode
    {
        get => Get(nameof(RedirectCode), "");
        set => Set(nameof(RedirectCode), value);
    }
    public static string RedirectUserName
    {
        get => Get(nameof(RedirectUserName), "admin");
        set => Set(nameof(RedirectUserName), value);
    }
    public static string RedirectUserPass
    {
        get => Get(nameof(RedirectUserPass), "12345");
        set => Set(nameof(RedirectUserPass), value);
    }
    public static string RedirectUrlAvailable
    {
        get => Get(nameof(RedirectUrlAvailable), "");
        set => Set(nameof(RedirectUrlAvailable), value);
    }
    public static string RedirectUrlAvailable2
    {
        get => Get(nameof(RedirectUrlAvailable2), "");
        set => Set(nameof(RedirectUrlAvailable2), value);
    }
    public static string UrlTrueService
    {
        get => Get(nameof(UrlTrueService), "");
        set => Set(nameof(UrlTrueService), value);
    }

    public static string LoginUserName
    {
        get => Get(nameof(LoginUserName), "");
        set => Set(nameof(LoginUserName), value);
    }
    public static string LoginUserPass
    {
        get => Get(nameof(LoginUserPass), "");
        set => Set(nameof(LoginUserPass), value);
    }
    public static string SecurityCode
    {
        get => Get(nameof(SecurityCode), "");
        set => Set(nameof(SecurityCode), value);
    }

    public static string RequestPermisionList
    {
        get
        {
            if (AppType != (long)PhoneAppTypeEnum.NhanVien)
                return "";

            var permissions = new List<string>();

            if (ChoPhep_NhanDienDt_Di == 1L || ChoPhep_NhanDienDt_Den == 1L)
            {
                permissions.AddRange(new[] { "1001", "1002", "1003", "1004", "1005", "1009", "10006" });
            }
            if (ChoPhep_NhanDienDt_HienPopup == 1L)
            {
                permissions.AddRange(new[] { "1007", "1008" });
            }
            if (ChoPhep_DocThongBao == 1L)
            {
                permissions.Add("1010");
            }
            return string.Join(",", permissions);
        }
    }
}