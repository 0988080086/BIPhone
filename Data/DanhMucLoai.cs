using Microsoft.Data.Sqlite;
using System.Data;

namespace BIPhone.Data;

public class DanhMucLoai
{
    public const string TableName = "DanhMucLoai";

    private static readonly Lazy<DanhMucLoai> _instance = new(() => new DanhMucLoai());
    public static DanhMucLoai Instance => _instance.Value;

    private string? _connectionString;

    private DanhMucLoai() { }

    // Cấu hình chuỗi kết nối
    public void Configure(string databasePath)
    {
        _connectionString = databasePath;
    }

    private SqliteConnection GetConnection()
    {
        if (string.IsNullOrEmpty(_connectionString))
        {
            throw new InvalidOperationException("DanhMucLoai chưa được cấu hình ConnectionString. Hãy gọi Configure() trước.");
        }

        var connection = new SqliteConnection(_connectionString);
        if (connection.State != ConnectionState.Open)
        {
            connection.Open();
        }
        return connection;
    }

    // Khởi tạo bảng nếu chưa tồn tại
    public void InitTable()
    {
        using var connection = GetConnection();
        using var cmd = new SqliteCommand(CreateTable(), connection);
        cmd.ExecuteNonQuery();
    }

    // Lưu bản ghi (Tự động INSERT hoặc UPDATE theo DmLoaiID bằng SQLite UPSERT)
    public bool Save(DanhMucLoaiItem item)
    {
        if (item == null) return false;

        using var connection = GetConnection();

        string sql = $@"
            INSERT INTO {TableName} (
                Rowguid, DmLoaiID, KieuDuLieu, Ten, DienGiai, TieuDeForm, TieuDeLuaChon,
                TieuDeTinhChat, CoDanhMuc, CoDanhMucDeQuy, CoTinhChat, TrangThai, NgayCn
            ) VALUES (
                @Rowguid, @DmLoaiID, @KieuDuLieu, @Ten, @DienGiai, @TieuDeForm, @TieuDeLuaChon,
                @TieuDeTinhChat, @CoDanhMuc, @CoDanhMucDeQuy, @CoTinhChat, @TrangThai, @NgayCn
            )
            ON CONFLICT(DmLoaiID) DO UPDATE SET
                Rowguid = excluded.Rowguid,
                KieuDuLieu = excluded.KieuDuLieu,
                Ten = excluded.Ten,
                DienGiai = excluded.DienGiai,
                TieuDeForm = excluded.TieuDeForm,
                TieuDeLuaChon = excluded.TieuDeLuaChon,
                TieuDeTinhChat = excluded.TieuDeTinhChat,
                CoDanhMuc = excluded.CoDanhMuc,
                CoDanhMucDeQuy = excluded.CoDanhMucDeQuy,
                CoTinhChat = excluded.CoTinhChat,
                TrangThai = excluded.TrangThai,
                NgayCn = excluded.NgayCn;";

        using var cmd = new SqliteCommand(sql, connection);
        AddParameters(cmd, item);

        return cmd.ExecuteNonQuery() > 0;
    }

    // Đọc 1 bản ghi theo DmLoaiID
    public DanhMucLoaiItem? GetByDmLoaiID(decimal dmLoaiId)
    {
        using var connection = GetConnection();
        string sql = $"SELECT * FROM {TableName} WHERE DmLoaiID = @DmLoaiID LIMIT 1;";
        using var cmd = new SqliteCommand(sql, connection);
        cmd.Parameters.AddWithValue("@DmLoaiID", dmLoaiId);

        using var reader = cmd.ExecuteReader();
        if (reader.Read())
        {
            return MapItem(reader);
        }

        return null;
    }

    // Đọc danh sách tất cả bản ghi
    public List<DanhMucLoaiItem> GetAll()
    {
        using var connection = GetConnection();
        var list = new List<DanhMucLoaiItem>();

        string sql = $"SELECT * FROM {TableName};";
        using var cmd = new SqliteCommand(sql, connection);
        using var reader = cmd.ExecuteReader();

        while (reader.Read())
        {
            list.Add(MapItem(reader));
        }

        return list;
    }

    // Xóa bản ghi theo DmLoaiID
    public bool DeleteByDmLoaiID(decimal dmLoaiId)
    {
        using var connection = GetConnection();
        string sql = $"DELETE FROM {TableName} WHERE DmLoaiID = @DmLoaiID;";
        using var cmd = new SqliteCommand(sql, connection);
        cmd.Parameters.AddWithValue("@DmLoaiID", dmLoaiId);

        return cmd.ExecuteNonQuery() > 0;
    }

    private static void AddParameters(SqliteCommand cmd, DanhMucLoaiItem item)
    {
        cmd.Parameters.AddWithValue("@Rowguid", item.Rowguid.ToString());
        cmd.Parameters.AddWithValue("@DmLoaiID", item.DmLoaiID);
        cmd.Parameters.AddWithValue("@KieuDuLieu", item.KieuDuLieu);
        cmd.Parameters.AddWithValue("@Ten", item.Ten ?? string.Empty);
        cmd.Parameters.AddWithValue("@DienGiai", item.DienGiai ?? string.Empty);
        cmd.Parameters.AddWithValue("@TieuDeForm", item.TieuDeForm ?? string.Empty);
        cmd.Parameters.AddWithValue("@TieuDeLuaChon", item.TieuDeLuaChon ?? string.Empty);
        cmd.Parameters.AddWithValue("@TieuDeTinhChat", item.TieuDeTinhChat ?? string.Empty);
        cmd.Parameters.AddWithValue("@CoDanhMuc", item.CoDanhMuc);
        cmd.Parameters.AddWithValue("@CoDanhMucDeQuy", item.CoDanhMucDeQuy);
        cmd.Parameters.AddWithValue("@CoTinhChat", item.CoTinhChat);
        cmd.Parameters.AddWithValue("@TrangThai", item.TrangThai);
        cmd.Parameters.AddWithValue("@NgayCn", item.NgayCn);
    }

    private static DanhMucLoaiItem MapItem(SqliteDataReader reader)
    {
        return new DanhMucLoaiItem
        {
            Rowguid = reader["Rowguid"] != DBNull.Value && Guid.TryParse(reader["Rowguid"].ToString(), out var g) ? g : Guid.Empty,
            DmLoaiID = Convert.ToDecimal(reader["DmLoaiID"]),
            KieuDuLieu = Convert.ToDecimal(reader["KieuDuLieu"]),
            Ten = reader["Ten"].ToString() ?? string.Empty,
            DienGiai = reader["DienGiai"].ToString() ?? string.Empty,
            TieuDeForm = reader["TieuDeForm"].ToString() ?? string.Empty,
            TieuDeLuaChon = reader["TieuDeLuaChon"].ToString() ?? string.Empty,
            TieuDeTinhChat = reader["TieuDeTinhChat"].ToString() ?? string.Empty,
            CoDanhMuc = Convert.ToDecimal(reader["CoDanhMuc"]),
            CoDanhMucDeQuy = Convert.ToDecimal(reader["CoDanhMucDeQuy"]),
            CoTinhChat = Convert.ToDecimal(reader["CoTinhChat"]),
            TrangThai = Convert.ToDecimal(reader["TrangThai"]),
            NgayCn = Convert.ToDouble(reader["NgayCn"])
        };
    }

    public static string CreateTable()
    {
        return $@"
        CREATE TABLE IF NOT EXISTS {TableName} (
            Rowguid TEXT,
            DmLoaiID NUMERIC PRIMARY KEY,
            KieuDuLieu NUMERIC,
            Ten TEXT,
            DienGiai TEXT,
            TieuDeForm TEXT,
            TieuDeLuaChon TEXT,
            TieuDeTinhChat TEXT,
            CoDanhMuc NUMERIC,
            CoDanhMucDeQuy NUMERIC,
            CoTinhChat NUMERIC,
            TrangThai NUMERIC,
            NgayCn REAL
        );";
    }
}

public class DanhMucLoaiItem
{
    public Guid Rowguid { get; set; } = Guid.Empty;
    public decimal DmLoaiID { get; set; } = 0m;
    public decimal KieuDuLieu { get; set; } = 0m;
    public string Ten { get; set; } = string.Empty;
    public string DienGiai { get; set; } = string.Empty;

    /// <summary>Tên hiển thị Danh mục loại DmLoaiID này</summary>
    public string TieuDeForm { get; set; } = string.Empty;

    /// <summary>Tên hiển thị trước Combobox</summary>
    public string TieuDeLuaChon { get; set; } = string.Empty;

    /// <summary>Tên hiển thị trước Combobox tính chất</summary>
    public string TieuDeTinhChat { get; set; } = string.Empty;

    public decimal CoDanhMuc { get; set; } = 0m;
    public decimal CoDanhMucDeQuy { get; set; } = 0m;
    public decimal CoTinhChat { get; set; } = 0m;
    public decimal TrangThai { get; set; } = (decimal)TrangThaiEnum.HetHieuLuc;
    public double NgayCn { get; set; } = 0;

    public DanhMucLoaiItem()
    {
    }
}