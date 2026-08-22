using Microsoft.Data.Sqlite;
using System.Data;

namespace BIPhone.Data;

public class DanhMuc
{
    public const string TableName = "DanhMuc";

    private static readonly Lazy<DanhMuc> _instance = new(() => new DanhMuc());
    public static DanhMuc Instance => _instance.Value;

    private string? _connectionString;

    private DanhMuc() { }

    // Cấu hình chuỗi kết nối
    public void Configure(string databasePath)
    {
        _connectionString = databasePath;
    }

    private SqliteConnection GetConnection()
    {
        if (string.IsNullOrEmpty(_connectionString))
        {
            throw new InvalidOperationException("DanhMuc chưa được cấu hình ConnectionString. Hãy gọi Configure() trước.");
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

    // Lưu bản ghi (Tự động INSERT hoặc UPDATE theo DmID bằng SQLite UPSERT)
    public bool Save(DanhMucItem item)
    {
        if (item == null) return false;

        using var connection = GetConnection();

        string sql = $@"
            INSERT INTO {TableName} (
                rowguid, DmLoaiID, DmID, DmChaID, DmRootID, Ma, Ten, DienGiai,
                MotaHtml, TinhChatID, TepID, DisplayOnWebsite, TrangThai, NgayCn, KichThuoc
            ) VALUES (
                @rowguid, @DmLoaiID, @DmID, @DmChaID, @DmRootID, @Ma, @Ten, @DienGiai,
                @MotaHtml, @TinhChatID, @TepID, @DisplayOnWebsite, @TrangThai, @NgayCn, @KichThuoc
            )
            ON CONFLICT(DmID) DO UPDATE SET
                rowguid = excluded.rowguid,
                DmLoaiID = excluded.DmLoaiID,
                DmChaID = excluded.DmChaID,
                DmRootID = excluded.DmRootID,
                Ma = excluded.Ma,
                Ten = excluded.Ten,
                DienGiai = excluded.DienGiai,
                MotaHtml = excluded.MotaHtml,
                TinhChatID = excluded.TinhChatID,
                TepID = excluded.TepID,
                DisplayOnWebsite = excluded.DisplayOnWebsite,
                TrangThai = excluded.TrangThai,
                NgayCn = excluded.NgayCn,
                KichThuoc = excluded.KichThuoc;";

        using var cmd = new SqliteCommand(sql, connection);
        AddParameters(cmd, item);

        return cmd.ExecuteNonQuery() > 0;
    }

    // Đọc 1 bản ghi theo DmID
    public DanhMucItem? GetByDmID(decimal dmId)
    {
        using var connection = GetConnection();
        string sql = $"SELECT * FROM {TableName} WHERE DmID = @DmID LIMIT 1;";
        using var cmd = new SqliteCommand(sql, connection);
        cmd.Parameters.AddWithValue("@DmID", dmId);

        using var reader = cmd.ExecuteReader();
        if (reader.Read())
        {
            return MapItem(reader);
        }

        return null;
    }

    // Đọc danh sách tất cả bản ghi
    public List<DanhMucItem> GetAll()
    {
        using var connection = GetConnection();
        var list = new List<DanhMucItem>();

        string sql = $"SELECT * FROM {TableName};";
        using var cmd = new SqliteCommand(sql, connection);
        using var reader = cmd.ExecuteReader();

        while (reader.Read())
        {
            list.Add(MapItem(reader));
        }

        return list;
    }

    // Xóa bản ghi theo DmID
    public bool DeleteByDmID(decimal dmId)
    {
        using var connection = GetConnection();
        string sql = $"DELETE FROM {TableName} WHERE DmID = @DmID;";
        using var cmd = new SqliteCommand(sql, connection);
        cmd.Parameters.AddWithValue("@DmID", dmId);

        return cmd.ExecuteNonQuery() > 0;
    }

    private static void AddParameters(SqliteCommand cmd, DanhMucItem item)
    {
        cmd.Parameters.AddWithValue("@rowguid", item.rowguid.ToString());
        cmd.Parameters.AddWithValue("@DmLoaiID", item.DmLoaiID);
        cmd.Parameters.AddWithValue("@DmID", item.DmID);
        cmd.Parameters.AddWithValue("@DmChaID", item.DmChaID);
        cmd.Parameters.AddWithValue("@DmRootID", item.DmRootID);
        cmd.Parameters.AddWithValue("@Ma", item.Ma ?? string.Empty);
        cmd.Parameters.AddWithValue("@Ten", item.Ten ?? string.Empty);
        cmd.Parameters.AddWithValue("@DienGiai", item.DienGiai ?? string.Empty);
        cmd.Parameters.AddWithValue("@MotaHtml", item.MotaHtml ?? string.Empty);
        cmd.Parameters.AddWithValue("@TinhChatID", item.TinhChatID ?? string.Empty);
        cmd.Parameters.AddWithValue("@TepID", item.TepID);
        cmd.Parameters.AddWithValue("@DisplayOnWebsite", item.DisplayOnWebsite);
        cmd.Parameters.AddWithValue("@TrangThai", item.TrangThai);
        cmd.Parameters.AddWithValue("@NgayCn", item.NgayCn);
        cmd.Parameters.AddWithValue("@KichThuoc", item.KichThuoc);
    }

    private static DanhMucItem MapItem(SqliteDataReader reader)
    {
        return new DanhMucItem
        {
            rowguid = reader["rowguid"] != DBNull.Value && Guid.TryParse(reader["rowguid"].ToString(), out var g) ? g : Guid.Empty,
            DmLoaiID = Convert.ToDecimal(reader["DmLoaiID"]),
            DmID = Convert.ToDecimal(reader["DmID"]),
            DmChaID = Convert.ToDecimal(reader["DmChaID"]),
            DmRootID = Convert.ToDecimal(reader["DmRootID"]),
            Ma = reader["Ma"].ToString() ?? string.Empty,
            Ten = reader["Ten"].ToString() ?? string.Empty,
            DienGiai = reader["DienGiai"].ToString() ?? string.Empty,
            MotaHtml = reader["MotaHtml"].ToString() ?? string.Empty,
            TinhChatID = reader["TinhChatID"].ToString() ?? string.Empty,
            TepID = Convert.ToDecimal(reader["TepID"]),
            DisplayOnWebsite = Convert.ToDecimal(reader["DisplayOnWebsite"]),
            TrangThai = Convert.ToDecimal(reader["TrangThai"]),
            NgayCn = Convert.ToDouble(reader["NgayCn"]),
            KichThuoc = Convert.ToDouble(reader["KichThuoc"])
        };
    }

    public static string CreateTable()
    {
        return $@"
        CREATE TABLE IF NOT EXISTS {TableName} (
            rowguid TEXT,
            DmLoaiID NUMERIC,
            DmID NUMERIC PRIMARY KEY,
            DmChaID NUMERIC,
            DmRootID NUMERIC,
            Ma TEXT,
            Ten TEXT,
            DienGiai TEXT,
            MotaHtml TEXT,
            TinhChatID TEXT,
            TepID NUMERIC,
            DisplayOnWebsite NUMERIC,
            TrangThai NUMERIC,
            NgayCn REAL,
            KichThuoc REAL
        );";
    }
}

public class DanhMucItem
{
    public Guid rowguid { get; set; } = Guid.Empty;
    public decimal DmLoaiID { get; set; } = 0m;
    public decimal DmID { get; set; } = 0m;
    public decimal DmChaID { get; set; } = 0m;
    public decimal DmRootID { get; set; } = 0m;
    public string Ma { get; set; } = string.Empty;
    public string Ten { get; set; } = string.Empty;
    public string DienGiai { get; set; } = string.Empty;
    public string MotaHtml { get; set; } = string.Empty;
    public string TinhChatID { get; set; } = string.Empty;
    public decimal TepID { get; set; } = 0m;
    public decimal DisplayOnWebsite { get; set; } = 0m;
    public decimal TrangThai { get; set; } = (decimal)TrangThaiEnum.HetHieuLuc;
    public double NgayCn { get; set; } = 0;

    /// <summary>Kích thước tệp có đường dẫn "Layout/DanhMuc/"+DmID+".cache"</summary>
    public double KichThuoc { get; set; } = 0;

    public DanhMucItem()
    {
    }
}