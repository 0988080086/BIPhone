
using Microsoft.Data.Sqlite;

namespace BIPhone.Data;

public class HangHoaHinhAnh
{
    public const string TableName = "HangHoaHinhAnh";
    private readonly SqliteConnection _connection;

    public HangHoaHinhAnh(SqliteConnection connection)
    {
        _connection = connection ?? throw new ArgumentNullException(nameof(connection));
    }

    private void EnsureOpen()
    {
        if (_connection.State != System.Data.ConnectionState.Open)
        {
            _connection.Open();
        }
    }

    // Khởi tạo bảng nếu chưa tồn tại
    public void InitTable()
    {
        EnsureOpen();
        using var cmd = new SqliteCommand(CreateTable(), _connection);
        cmd.ExecuteNonQuery();
    }

    // Lưu bản ghi (Tự động INSERT hoặc UPDATE theo TepID)
    public bool Save(HangHoaHinhAnhItem item)
    {
        if (item == null) return false;
        EnsureOpen();

        // 1. Kiểm tra xem TepID đã tồn tại chưa
        string checkSql = $"SELECT EXISTS(SELECT 1 FROM {TableName} WHERE TepID = @TepID LIMIT 1);";
        using var checkCmd = new SqliteCommand(checkSql, _connection);
        checkCmd.Parameters.AddWithValue("@TepID", item.TepID);

        bool exists = Convert.ToInt64(checkCmd.ExecuteScalar()) == 1;

        string sql;
        if (exists)
        {
            // 2a. Nếu đã tồn tại -> Thực hiện UPDATE
            sql = $@"UPDATE {TableName} 
                    SET Rowguid = @Rowguid, HhID = @HhID, TinhChatID = @TinhChatID, TieuDe = @TieuDe,
                        Nam = @Nam, Stt = @Stt, MauSac = @MauSac, KichCo = @KichCo,
                        TenBang = @TenBang, ValueID = @ValueID, DuongDan = @DuongDan, TenTep = @TenTep,
                        FileSize = @FileSize, TrangThai = @TrangThai, NgayCN = @NgayCN
                    WHERE TepID = @TepID;";
        }
        else
        {
            // 2b. Nếu chưa tồn tại -> Thực hiện INSERT
            sql = $@"INSERT INTO {TableName} (
                        Rowguid, HhID, TinhChatID, TieuDe, TepID, Nam, Stt, MauSac,
                        KichCo, TenBang, ValueID, DuongDan, TenTep, FileSize, TrangThai, NgayCN
                    ) VALUES (
                        @Rowguid, @HhID, @TinhChatID, @TieuDe, @TepID, @Nam, @Stt, @MauSac,
                        @KichCo, @TenBang, @ValueID, @DuongDan, @TenTep, @FileSize, @TrangThai, @NgayCN
                    );";
        }

        using var cmd = new SqliteCommand(sql, _connection);
        AddParameters(cmd, item);

        return cmd.ExecuteNonQuery() > 0;
    }

    // Đọc 1 bản ghi theo TepID
    public HangHoaHinhAnhItem? GetByTepID(decimal tepId)
    {
        EnsureOpen();
        string sql = $"SELECT * FROM {TableName} WHERE TepID = @TepID LIMIT 1;";
        using var cmd = new SqliteCommand(sql, _connection);
        cmd.Parameters.AddWithValue("@TepID", tepId);

        using var reader = cmd.ExecuteReader();
        if (reader.Read())
        {
            return MapItem(reader);
        }

        return null;
    }

    // Đọc danh sách tất cả hình ảnh thuộc 1 hàng hóa (HhID)
    public List<HangHoaHinhAnhItem> GetByHhID(decimal hhId)
    {
        EnsureOpen();
        var list = new List<HangHoaHinhAnhItem>();

        string sql = $"SELECT * FROM {TableName} WHERE HhID = @HhID ORDER BY Stt ASC;";
        using var cmd = new SqliteCommand(sql, _connection);
        cmd.Parameters.AddWithValue("@HhID", hhId);

        using var reader = cmd.ExecuteReader();
        while (reader.Read())
        {
            list.Add(MapItem(reader));
        }

        return list;
    }

    // Đọc tất cả bản ghi
    public List<HangHoaHinhAnhItem> GetAll()
    {
        EnsureOpen();
        var list = new List<HangHoaHinhAnhItem>();

        string sql = $"SELECT * FROM {TableName};";
        using var cmd = new SqliteCommand(sql, _connection);
        using var reader = cmd.ExecuteReader();

        while (reader.Read())
        {
            list.Add(MapItem(reader));
        }

        return list;
    }

    // Xóa bản ghi theo TepID
    public bool DeleteByTepID(decimal tepId)
    {
        EnsureOpen();
        string sql = $"DELETE FROM {TableName} WHERE TepID = @TepID;";
        using var cmd = new SqliteCommand(sql, _connection);
        cmd.Parameters.AddWithValue("@TepID", tepId);

        return cmd.ExecuteNonQuery() > 0;
    }

    private static void AddParameters(SqliteCommand cmd, HangHoaHinhAnhItem item)
    {
        cmd.Parameters.AddWithValue("@Rowguid", item.Rowguid.ToString());
        cmd.Parameters.AddWithValue("@HhID", item.HhID);
        cmd.Parameters.AddWithValue("@TinhChatID", item.TinhChatID);
        cmd.Parameters.AddWithValue("@TieuDe", item.TieuDe ?? string.Empty);
        cmd.Parameters.AddWithValue("@TepID", item.TepID);
        cmd.Parameters.AddWithValue("@Nam", item.Nam);
        cmd.Parameters.AddWithValue("@Stt", item.Stt);
        cmd.Parameters.AddWithValue("@MauSac", item.MauSac ?? string.Empty);
        cmd.Parameters.AddWithValue("@KichCo", item.KichCo ?? string.Empty);
        cmd.Parameters.AddWithValue("@TenBang", item.TenBang ?? string.Empty);
        cmd.Parameters.AddWithValue("@ValueID", item.ValueID);
        cmd.Parameters.AddWithValue("@DuongDan", item.DuongDan ?? string.Empty);
        cmd.Parameters.AddWithValue("@TenTep", item.TenTep ?? string.Empty);
        cmd.Parameters.AddWithValue("@FileSize", item.FileSize);
        cmd.Parameters.AddWithValue("@TrangThai", item.TrangThai);
        cmd.Parameters.AddWithValue("@NgayCN", item.NgayCN);
    }

    private static HangHoaHinhAnhItem MapItem(SqliteDataReader reader)
    {
        return new HangHoaHinhAnhItem
        {
            Rowguid = reader["Rowguid"] != DBNull.Value && Guid.TryParse(reader["Rowguid"].ToString(), out var g) ? g : Guid.Empty,
            HhID = Convert.ToDecimal(reader["HhID"]),
            TinhChatID = Convert.ToDecimal(reader["TinhChatID"]),
            TieuDe = reader["TieuDe"].ToString() ?? string.Empty,
            TepID = Convert.ToDecimal(reader["TepID"]),
            Nam = Convert.ToDecimal(reader["Nam"]),
            Stt = Convert.ToDecimal(reader["Stt"]),
            MauSac = reader["MauSac"].ToString() ?? string.Empty,
            KichCo = reader["KichCo"].ToString() ?? string.Empty,
            TenBang = reader["TenBang"].ToString() ?? string.Empty,
            ValueID = Convert.ToDecimal(reader["ValueID"]),
            DuongDan = reader["DuongDan"].ToString() ?? string.Empty,
            TenTep = reader["TenTep"].ToString() ?? string.Empty,
            FileSize = Convert.ToDouble(reader["FileSize"]),
            TrangThai = Convert.ToDecimal(reader["TrangThai"]),
            NgayCN = Convert.ToDouble(reader["NgayCN"])
        };
    }

    public static string CreateTable()
    {
        return $@"
        CREATE TABLE IF NOT EXISTS {TableName} (
            Rowguid TEXT,
            HhID NUMERIC,
            TinhChatID NUMERIC,
            TieuDe TEXT,
            TepID NUMERIC,
            Nam NUMERIC,
            Stt NUMERIC,
            MauSac TEXT,
            KichCo TEXT,
            TenBang TEXT,
            ValueID NUMERIC,
            DuongDan TEXT,
            TenTep TEXT,
            FileSize REAL,
            TrangThai NUMERIC,
            NgayCN REAL
        );";
    }
}

public class HangHoaHinhAnhItem
{
    public Guid Rowguid { get; set; } = Guid.Empty;
    public decimal HhID { get; set; } = 0;
    public decimal TinhChatID { get; set; } = 0;
    public string TieuDe { get; set; } = string.Empty;
    public decimal TepID { get; set; } = 0;
    public decimal Nam { get; set; } = 0;
    public decimal Stt { get; set; } = 0;
    public string MauSac { get; set; } = string.Empty;
    public string KichCo { get; set; } = string.Empty;
    public string TenBang { get; set; } = string.Empty;
    public decimal ValueID { get; set; } = 0;
    public string DuongDan { get; set; } = string.Empty;
    public string TenTep { get; set; } = string.Empty;
    public double FileSize { get; set; } = 0;
    public decimal TrangThai { get; set; } = 0;
    public double NgayCN { get; set; } = 0;
}