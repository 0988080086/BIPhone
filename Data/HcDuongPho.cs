using System.Data;
using Microsoft.Data.Sqlite;

namespace BIPhone.Data;

public class HcDuongPho
{
    public const string TableName = "HcDuongPho";
    private SqliteConnection _connection = null!;

    // Singleton Instance
    public static HcDuongPho Instance { get; } = new HcDuongPho();

    // Constructor rỗng phục vụ Singleton
    public HcDuongPho() { }

    // Constructor hỗ trợ truyền trực tiếp connection nếu cần
    public HcDuongPho(SqliteConnection connection)
    {
        _connection = connection ?? throw new ArgumentNullException(nameof(connection));
    }

    // Cấu hình ConnectionString đồng bộ
    public void Configure(string databasePath)
    {
        var builder = new SqliteConnectionStringBuilder(databasePath);
        _connection = new SqliteConnection(builder.ConnectionString);
    }

    private void EnsureOpen()
    {
        if (_connection.State != ConnectionState.Open)
        {
            _connection.Open();
        }
    }

    public void InitTable()
    {
        EnsureOpen();
        using var cmd = new SqliteCommand(CreateTable(), _connection);
        cmd.ExecuteNonQuery();
    }

    // Lưu bản ghi (Tối ưu bằng cú pháp ON CONFLICT của SQLite)
    public bool Save(HcDuongPhoItem item)
    {
        if (item == null) return false;
        EnsureOpen();

        string sql = $@"
            INSERT INTO {TableName} (
                rowguid, DpID, HcID, Ten, DienGiai, TrangThai, NgayCn
            ) VALUES (
                @rowguid, @DpID, @HcID, @Ten, @DienGiai, @TrangThai, @NgayCn
            )
            ON CONFLICT(DpID) DO UPDATE SET
                rowguid = excluded.rowguid,
                HcID = excluded.HcID,
                Ten = excluded.Ten,
                DienGiai = excluded.DienGiai,
                TrangThai = excluded.TrangThai,
                NgayCn = excluded.NgayCn;";

        using var cmd = new SqliteCommand(sql, _connection);
        AddParameters(cmd, item);

        return cmd.ExecuteNonQuery() > 0;
    }

    public HcDuongPhoItem? GetByDpID(decimal dpId)
    {
        EnsureOpen();
        string sql = $"SELECT * FROM {TableName} WHERE DpID = @DpID LIMIT 1;";
        using var cmd = new SqliteCommand(sql, _connection);
        cmd.Parameters.AddWithValue("@DpID", dpId);

        using var reader = cmd.ExecuteReader();
        if (reader.Read())
        {
            return MapItem(reader);
        }

        return null;
    }

    public List<HcDuongPhoItem> GetAll()
    {
        EnsureOpen();
        var list = new List<HcDuongPhoItem>();

        string sql = $"SELECT * FROM {TableName};";
        using var cmd = new SqliteCommand(sql, _connection);
        using var reader = cmd.ExecuteReader();

        while (reader.Read())
        {
            list.Add(MapItem(reader));
        }

        return list;
    }

    public bool DeleteByDpID(decimal dpId)
    {
        EnsureOpen();
        string sql = $"DELETE FROM {TableName} WHERE DpID = @DpID;";
        using var cmd = new SqliteCommand(sql, _connection);
        cmd.Parameters.AddWithValue("@DpID", dpId);

        return cmd.ExecuteNonQuery() > 0;
    }

    private static void AddParameters(SqliteCommand cmd, HcDuongPhoItem item)
    {
        cmd.Parameters.AddWithValue("@rowguid", item.rowguid.ToString());
        cmd.Parameters.AddWithValue("@DpID", item.DpID);
        cmd.Parameters.AddWithValue("@HcID", item.HcID ?? string.Empty);
        cmd.Parameters.AddWithValue("@Ten", item.Ten ?? string.Empty);
        cmd.Parameters.AddWithValue("@DienGiai", item.DienGiai ?? string.Empty);
        cmd.Parameters.AddWithValue("@TrangThai", item.TrangThai);
        cmd.Parameters.AddWithValue("@NgayCn", item.NgayCn);
    }

    private static HcDuongPhoItem MapItem(SqliteDataReader reader)
    {
        return new HcDuongPhoItem
        {
            rowguid = reader["rowguid"] != DBNull.Value && Guid.TryParse(reader["rowguid"].ToString(), out var g) ? g : Guid.Empty,
            DpID = GetDecimal(reader, "DpID"),
            HcID = GetString(reader, "HcID"),
            Ten = GetString(reader, "Ten"),
            DienGiai = GetString(reader, "DienGiai"),
            TrangThai = GetDecimal(reader, "TrangThai"),
            NgayCn = GetDouble(reader, "NgayCn")
        };
    }

    private static string GetString(SqliteDataReader reader, string columnName)
    {
        var val = reader[columnName];
        return val != DBNull.Value ? val.ToString() ?? string.Empty : string.Empty;
    }

    private static decimal GetDecimal(SqliteDataReader reader, string columnName)
    {
        var val = reader[columnName];
        return val != DBNull.Value ? Convert.ToDecimal(val) : 0m;
    }

    private static double GetDouble(SqliteDataReader reader, string columnName)
    {
        var val = reader[columnName];
        return val != DBNull.Value ? Convert.ToDouble(val) : 0.0;
    }

    public static string CreateTable()
    {
        return $@"
        CREATE TABLE IF NOT EXISTS {TableName} (
            rowguid TEXT,
            DpID NUMERIC PRIMARY KEY,
            HcID TEXT,
            Ten TEXT,
            DienGiai TEXT,
            TrangThai NUMERIC,
            NgayCn REAL
        );";
    }
}

public class HcDuongPhoItem
{
    public Guid rowguid { get; set; }
    public decimal DpID { get; set; }
    public string HcID { get; set; }
    public string Ten { get; set; }
    public string DienGiai { get; set; }
    public decimal TrangThai { get; set; }
    public double NgayCn { get; set; }

    public HcDuongPhoItem()
    {
        rowguid = Guid.Empty;
        DpID = 0m;
        HcID = string.Empty;
        Ten = string.Empty;
        DienGiai = string.Empty;
        TrangThai = 0m;
        NgayCn = 0;
    }
}