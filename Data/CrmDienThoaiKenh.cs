using Microsoft.Data.Sqlite;
using System.Data;

namespace BIPhone.Data;

public class CrmDienThoaiKenh
{
    public const string TableName = "CrmDienThoaiKenh";

    private static readonly Lazy<CrmDienThoaiKenh> _instance = new(() => new CrmDienThoaiKenh());
    public static CrmDienThoaiKenh Instance => _instance.Value;

    private string? _connectionString;

    private CrmDienThoaiKenh() { }

    // Cấu hình chuỗi kết nối (Gợi nhớ giống CrmDienThoai)
    public void Configure(string databasePath)
    {
        _connectionString = databasePath;
    }

    private SqliteConnection GetConnection()
    {
        if (string.IsNullOrEmpty(_connectionString))
        {
            throw new InvalidOperationException("CrmDienThoaiKenh chưa được cấu hình ConnectionString. Hãy gọi Configure() trước.");
        }

        var connection = new SqliteConnection(_connectionString);
        if (connection.State != ConnectionState.Open)
        {
            connection.Open();
        }
        return connection;
    }

    // Khởi tạo bảng
    public void InitTable()
    {
        using var connection = GetConnection();
        using var cmd = new SqliteCommand(CreateTable(), connection);
        cmd.ExecuteNonQuery();
    }

    // Lưu bản ghi (Tự động INSERT hoặc UPDATE theo KenhID bằng SQLite UPSERT)
    public bool Save(CrmDienThoaiKenhItem item)
    {
        if (item == null) return false;

        using var connection = GetConnection();

        string sql = $@"
            INSERT INTO {TableName} (
                Rowguid, KenhID, PbID, NvID, Kenh, KenhSoMay, TinhChat, DienGiai,
                CpuID, NetworkLayer, DeviceName, DeviceChannel, DeviceRecord,
                NsdPhoneDeviceID, Android_LastLogin, Android_IPaddress, Android_TenTruyCap,
                Android_MatKhau, ChoPhep_NhanDienDt_Den, ChoPhep_NhanDienDt_HienPopup,
                ChoPhep_NhanDienDt_Di, ChoPhep_GhiAmDt, ChoPhep_GuiTinSms, ChoPhep_LuuTruGps,
                ChoPhep_TimKiemKh, ChoPhep_ThemMoiKh, ChoPhep_SuaKh, ChoPhep_TaoChungTu,
                ChoPhep_SuaChungTu, ChoPhep_XemGpsNhanVien, TrangThai, NgayCn
            ) VALUES (
                @Rowguid, @KenhID, @PbID, @NvID, @Kenh, @KenhSoMay, @TinhChat, @DienGiai,
                @CpuID, @NetworkLayer, @DeviceName, @DeviceChannel, @DeviceRecord,
                @NsdPhoneDeviceID, @Android_LastLogin, @Android_IPaddress, @Android_TenTruyCap,
                @Android_MatKhau, @ChoPhep_NhanDienDt_Den, @ChoPhep_NhanDienDt_HienPopup,
                @ChoPhep_NhanDienDt_Di, @ChoPhep_GhiAmDt, @ChoPhep_GuiTinSms, @ChoPhep_LuuTruGps,
                @ChoPhep_TimKiemKh, @ChoPhep_ThemMoiKh, @ChoPhep_SuaKh, @ChoPhep_TaoChungTu,
                @ChoPhep_SuaChungTu, @ChoPhep_XemGpsNhanVien, @TrangThai, @NgayCn
            )
            ON CONFLICT(KenhID) DO UPDATE SET
                Rowguid = excluded.Rowguid,
                PbID = excluded.PbID,
                NvID = excluded.NvID,
                Kenh = excluded.Kenh,
                KenhSoMay = excluded.KenhSoMay,
                TinhChat = excluded.TinhChat,
                DienGiai = excluded.DienGiai,
                CpuID = excluded.CpuID,
                NetworkLayer = excluded.NetworkLayer,
                DeviceName = excluded.DeviceName,
                DeviceChannel = excluded.DeviceChannel,
                DeviceRecord = excluded.DeviceRecord,
                NsdPhoneDeviceID = excluded.NsdPhoneDeviceID,
                Android_LastLogin = excluded.Android_LastLogin,
                Android_IPaddress = excluded.Android_IPaddress,
                Android_TenTruyCap = excluded.Android_TenTruyCap,
                Android_MatKhau = excluded.Android_MatKhau,
                ChoPhep_NhanDienDt_Den = excluded.ChoPhep_NhanDienDt_Den,
                ChoPhep_NhanDienDt_HienPopup = excluded.ChoPhep_NhanDienDt_HienPopup,
                ChoPhep_NhanDienDt_Di = excluded.ChoPhep_NhanDienDt_Di,
                ChoPhep_GhiAmDt = excluded.ChoPhep_GhiAmDt,
                ChoPhep_GuiTinSms = excluded.ChoPhep_GuiTinSms,
                ChoPhep_LuuTruGps = excluded.ChoPhep_LuuTruGps,
                ChoPhep_TimKiemKh = excluded.ChoPhep_TimKiemKh,
                ChoPhep_ThemMoiKh = excluded.ChoPhep_ThemMoiKh,
                ChoPhep_SuaKh = excluded.ChoPhep_SuaKh,
                ChoPhep_TaoChungTu = excluded.ChoPhep_TaoChungTu,
                ChoPhep_SuaChungTu = excluded.ChoPhep_SuaChungTu,
                ChoPhep_XemGpsNhanVien = excluded.ChoPhep_XemGpsNhanVien,
                TrangThai = excluded.TrangThai,
                NgayCn = excluded.NgayCn;";

        using var cmd = new SqliteCommand(sql, connection);
        AddParameters(cmd, item);

        return cmd.ExecuteNonQuery() > 0;
    }

    // Đọc 1 bản ghi theo KenhID
    public CrmDienThoaiKenhItem? GetByKenhID(decimal kenhId)
    {
        using var connection = GetConnection();
        string sql = $"SELECT * FROM {TableName} WHERE KenhID = @KenhID LIMIT 1;";
        using var cmd = new SqliteCommand(sql, connection);
        cmd.Parameters.AddWithValue("@KenhID", kenhId);

        using var reader = cmd.ExecuteReader();
        if (reader.Read())
        {
            return MapItem(reader);
        }

        return null;
    }

    // Đọc tất cả bản ghi
    public List<CrmDienThoaiKenhItem> GetAll()
    {
        using var connection = GetConnection();
        var list = new List<CrmDienThoaiKenhItem>();

        string sql = $"SELECT * FROM {TableName};";
        using var cmd = new SqliteCommand(sql, connection);
        using var reader = cmd.ExecuteReader();

        while (reader.Read())
        {
            list.Add(MapItem(reader));
        }

        return list;
    }

    // Xóa bản ghi theo KenhID
    public bool DeleteByKenhID(decimal kenhId)
    {
        using var connection = GetConnection();
        string sql = $"DELETE FROM {TableName} WHERE KenhID = @KenhID;";
        using var cmd = new SqliteCommand(sql, connection);
        cmd.Parameters.AddWithValue("@KenhID", kenhId);

        return cmd.ExecuteNonQuery() > 0;
    }

    private static void AddParameters(SqliteCommand cmd, CrmDienThoaiKenhItem item)
    {
        cmd.Parameters.AddWithValue("@Rowguid", item.Rowguid.ToString());
        cmd.Parameters.AddWithValue("@KenhID", item.KenhID);
        cmd.Parameters.AddWithValue("@PbID", item.PbID);
        cmd.Parameters.AddWithValue("@NvID", item.NvID);
        cmd.Parameters.AddWithValue("@Kenh", item.Kenh ?? string.Empty);
        cmd.Parameters.AddWithValue("@KenhSoMay", item.KenhSoMay ?? string.Empty);
        cmd.Parameters.AddWithValue("@TinhChat", item.TinhChat);
        cmd.Parameters.AddWithValue("@DienGiai", item.DienGiai ?? string.Empty);
        cmd.Parameters.AddWithValue("@CpuID", item.CpuID ?? string.Empty);
        cmd.Parameters.AddWithValue("@NetworkLayer", item.NetworkLayer);
        cmd.Parameters.AddWithValue("@DeviceName", item.DeviceName ?? string.Empty);
        cmd.Parameters.AddWithValue("@DeviceChannel", item.DeviceChannel ?? string.Empty);
        cmd.Parameters.AddWithValue("@DeviceRecord", item.DeviceRecord ?? string.Empty);
        cmd.Parameters.AddWithValue("@NsdPhoneDeviceID", item.NsdPhoneDeviceID ?? string.Empty);
        cmd.Parameters.AddWithValue("@Android_LastLogin", item.Android_LastLogin);
        cmd.Parameters.AddWithValue("@Android_IPaddress", item.Android_IPaddress ?? string.Empty);
        cmd.Parameters.AddWithValue("@Android_TenTruyCap", item.Android_TenTruyCap ?? string.Empty);
        cmd.Parameters.AddWithValue("@Android_MatKhau", item.Android_MatKhau ?? string.Empty);
        cmd.Parameters.AddWithValue("@ChoPhep_NhanDienDt_Den", item.ChoPhep_NhanDienDt_Den);
        cmd.Parameters.AddWithValue("@ChoPhep_NhanDienDt_HienPopup", item.ChoPhep_NhanDienDt_HienPopup);
        cmd.Parameters.AddWithValue("@ChoPhep_NhanDienDt_Di", item.ChoPhep_NhanDienDt_Di);
        cmd.Parameters.AddWithValue("@ChoPhep_GhiAmDt", item.ChoPhep_GhiAmDt);
        cmd.Parameters.AddWithValue("@ChoPhep_GuiTinSms", item.ChoPhep_GuiTinSms);
        cmd.Parameters.AddWithValue("@ChoPhep_LuuTruGps", item.ChoPhep_LuuTruGps);
        cmd.Parameters.AddWithValue("@ChoPhep_TimKiemKh", item.ChoPhep_TimKiemKh);
        cmd.Parameters.AddWithValue("@ChoPhep_ThemMoiKh", item.ChoPhep_ThemMoiKh);
        cmd.Parameters.AddWithValue("@ChoPhep_SuaKh", item.ChoPhep_SuaKh);
        cmd.Parameters.AddWithValue("@ChoPhep_TaoChungTu", item.ChoPhep_TaoChungTu);
        cmd.Parameters.AddWithValue("@ChoPhep_SuaChungTu", item.ChoPhep_SuaChungTu);
        cmd.Parameters.AddWithValue("@ChoPhep_XemGpsNhanVien", item.ChoPhep_XemGpsNhanVien);
        cmd.Parameters.AddWithValue("@TrangThai", item.TrangThai);
        cmd.Parameters.AddWithValue("@NgayCn", item.NgayCn);
    }

    private static CrmDienThoaiKenhItem MapItem(SqliteDataReader reader)
    {
        return new CrmDienThoaiKenhItem
        {
            Rowguid = reader["Rowguid"] != DBNull.Value && Guid.TryParse(reader["Rowguid"].ToString(), out var g) ? g : Guid.Empty,
            KenhID = Convert.ToDecimal(reader["KenhID"]),
            PbID = Convert.ToDecimal(reader["PbID"]),
            NvID = Convert.ToDecimal(reader["NvID"]),
            Kenh = reader["Kenh"].ToString() ?? string.Empty,
            KenhSoMay = reader["KenhSoMay"].ToString() ?? string.Empty,
            TinhChat = Convert.ToDecimal(reader["TinhChat"]),
            DienGiai = reader["DienGiai"].ToString() ?? string.Empty,
            CpuID = reader["CpuID"].ToString() ?? string.Empty,
            NetworkLayer = Convert.ToDecimal(reader["NetworkLayer"]),
            DeviceName = reader["DeviceName"].ToString() ?? string.Empty,
            DeviceChannel = reader["DeviceChannel"].ToString() ?? string.Empty,
            DeviceRecord = reader["DeviceRecord"].ToString() ?? string.Empty,
            NsdPhoneDeviceID = reader["NsdPhoneDeviceID"].ToString() ?? string.Empty,
            Android_LastLogin = Convert.ToDouble(reader["Android_LastLogin"]),
            Android_IPaddress = reader["Android_IPaddress"].ToString() ?? string.Empty,
            Android_TenTruyCap = reader["Android_TenTruyCap"].ToString() ?? string.Empty,
            Android_MatKhau = reader["Android_MatKhau"].ToString() ?? string.Empty,
            ChoPhep_NhanDienDt_Den = Convert.ToDecimal(reader["ChoPhep_NhanDienDt_Den"]),
            ChoPhep_NhanDienDt_HienPopup = Convert.ToDecimal(reader["ChoPhep_NhanDienDt_HienPopup"]),
            ChoPhep_NhanDienDt_Di = Convert.ToDecimal(reader["ChoPhep_NhanDienDt_Di"]),
            ChoPhep_GhiAmDt = Convert.ToDecimal(reader["ChoPhep_GhiAmDt"]),
            ChoPhep_GuiTinSms = Convert.ToDecimal(reader["ChoPhep_GuiTinSms"]),
            ChoPhep_LuuTruGps = Convert.ToDecimal(reader["ChoPhep_LuuTruGps"]),
            ChoPhep_TimKiemKh = Convert.ToDecimal(reader["ChoPhep_TimKiemKh"]),
            ChoPhep_ThemMoiKh = Convert.ToDecimal(reader["ChoPhep_ThemMoiKh"]),
            ChoPhep_SuaKh = Convert.ToDecimal(reader["ChoPhep_SuaKh"]),
            ChoPhep_TaoChungTu = Convert.ToDecimal(reader["ChoPhep_TaoChungTu"]),
            ChoPhep_SuaChungTu = Convert.ToDecimal(reader["ChoPhep_SuaChungTu"]),
            ChoPhep_XemGpsNhanVien = Convert.ToDecimal(reader["ChoPhep_XemGpsNhanVien"]),
            TrangThai = Convert.ToDecimal(reader["TrangThai"]),
            NgayCn = Convert.ToDouble(reader["NgayCn"])
        };
    }

    public static string CreateTable()
    {
        return $@"
        CREATE TABLE IF NOT EXISTS {TableName} (
            Rowguid TEXT,
            KenhID NUMERIC PRIMARY KEY,
            PbID NUMERIC,
            NvID NUMERIC,
            Kenh TEXT,
            KenhSoMay TEXT,
            TinhChat NUMERIC,
            DienGiai TEXT,
            CpuID TEXT,
            NetworkLayer NUMERIC,
            DeviceName TEXT,
            DeviceChannel TEXT,
            DeviceRecord TEXT,
            NsdPhoneDeviceID TEXT,
            Android_LastLogin REAL,
            Android_IPaddress TEXT,
            Android_TenTruyCap TEXT,
            Android_MatKhau TEXT,
            ChoPhep_NhanDienDt_Den NUMERIC,
            ChoPhep_NhanDienDt_HienPopup NUMERIC,
            ChoPhep_NhanDienDt_Di NUMERIC,
            ChoPhep_GhiAmDt NUMERIC,
            ChoPhep_GuiTinSms NUMERIC,
            ChoPhep_LuuTruGps NUMERIC,
            ChoPhep_TimKiemKh NUMERIC,
            ChoPhep_ThemMoiKh NUMERIC,
            ChoPhep_SuaKh NUMERIC,
            ChoPhep_TaoChungTu NUMERIC,
            ChoPhep_SuaChungTu NUMERIC,
            ChoPhep_XemGpsNhanVien NUMERIC,
            TrangThai NUMERIC,
            NgayCn REAL
        );";
    }
}
public class CrmDienThoaiKenhItem
{
    public Guid Rowguid { get; set; } = Guid.Empty;
    public decimal KenhID { get; set; } = 0;
    public decimal PbID { get; set; } = 0;
    public decimal NvID { get; set; } = 0;
    public string Kenh { get; set; } = string.Empty;
    public string KenhSoMay { get; set; } = string.Empty;
    public decimal TinhChat { get; set; } = 0;
    public string DienGiai { get; set; } = string.Empty;
    public string CpuID { get; set; } = string.Empty;
    public decimal NetworkLayer { get; set; } = 0;
    public string DeviceName { get; set; } = string.Empty;
    public string DeviceChannel { get; set; } = string.Empty;
    public string DeviceRecord { get; set; } = string.Empty;
    public string NsdPhoneDeviceID { get; set; } = string.Empty;
    public double Android_LastLogin { get; set; } = 0;
    public string Android_IPaddress { get; set; } = string.Empty;
    public string Android_TenTruyCap { get; set; } = string.Empty;
    public string Android_MatKhau { get; set; } = string.Empty;
    public decimal ChoPhep_NhanDienDt_Den { get; set; } = 0;
    public decimal ChoPhep_NhanDienDt_HienPopup { get; set; } = 0;
    public decimal ChoPhep_NhanDienDt_Di { get; set; } = 0;
    public decimal ChoPhep_GhiAmDt { get; set; } = 0;
    public decimal ChoPhep_GuiTinSms { get; set; } = 0;
    public decimal ChoPhep_LuuTruGps { get; set; } = 0;
    public decimal ChoPhep_TimKiemKh { get; set; } = 0;
    public decimal ChoPhep_ThemMoiKh { get; set; } = 0;
    public decimal ChoPhep_SuaKh { get; set; } = 0;
    public decimal ChoPhep_TaoChungTu { get; set; } = 0;
    public decimal ChoPhep_SuaChungTu { get; set; } = 0;
    public decimal ChoPhep_XemGpsNhanVien { get; set; } = 0;
    public decimal TrangThai { get; set; } = 0;
    public double NgayCn { get; set; } = 0;
}