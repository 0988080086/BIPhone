using System.Data;
using Microsoft.Data.Sqlite;

namespace BIPhone.Data;

public class HcDiaChi
{
    public const string TableName = "HcDiaChi";
    private SqliteConnection _connection = null!;

    // Singleton Instance
    public static HcDiaChi Instance { get; } = new HcDiaChi();

    // Constructor rỗng cho Singleton
    public HcDiaChi() { }

    public HcDiaChi(SqliteConnection connection)
    {
        _connection = connection ?? throw new ArgumentNullException(nameof(connection));
    }

    // Cấu hình ConnectionString đồng bộ với các lớp khác
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

    // Lưu bản ghi (UPSERT 1 câu lệnh duy nhất bằng ON CONFLICT)
    public bool Save(HcDiaChiItem item)
    {
        if (item == null) return false;
        EnsureOpen();

        string sql = $@"
            INSERT INTO {TableName} (
                rowguid, HcID, HcChaID, TinhChat, NuocID, BangID,
                TtID, QhID, XpID, XomID, Ma, Ten, DienGiai,
                TrangThai, NgayCn
            ) VALUES (
                @rowguid, @HcID, @HcChaID, @TinhChat, @NuocID, @BangID,
                @TtID, @QhID, @XpID, @XomID, @Ma, @Ten, @DienGiai,
                @TrangThai, @NgayCn
            )
            ON CONFLICT(HcID) DO UPDATE SET
                rowguid = excluded.rowguid,
                HcChaID = excluded.HcChaID,
                TinhChat = excluded.TinhChat,
                NuocID = excluded.NuocID,
                BangID = excluded.BangID,
                TtID = excluded.TtID,
                QhID = excluded.QhID,
                XpID = excluded.XpID,
                XomID = excluded.XomID,
                Ma = excluded.Ma,
                Ten = excluded.Ten,
                DienGiai = excluded.DienGiai,
                TrangThai = excluded.TrangThai,
                NgayCn = excluded.NgayCn;";

        using var cmd = new SqliteCommand(sql, _connection);
        AddParameters(cmd, item);

        return cmd.ExecuteNonQuery() > 0;
    }

    public HcDiaChiItem? GetByHcID(decimal hcId)
    {
        EnsureOpen();
        string sql = $"SELECT * FROM {TableName} WHERE HcID = @HcID LIMIT 1;";
        using var cmd = new SqliteCommand(sql, _connection);
        cmd.Parameters.AddWithValue("@HcID", hcId);

        using var reader = cmd.ExecuteReader();
        if (reader.Read())
        {
            return MapItem(reader);
        }

        return null;
    }

    public List<HcDiaChiItem> GetAll()
    {
        EnsureOpen();
        var list = new List<HcDiaChiItem>();

        string sql = $"SELECT * FROM {TableName};";
        using var cmd = new SqliteCommand(sql, _connection);
        using var reader = cmd.ExecuteReader();

        while (reader.Read())
        {
            list.Add(MapItem(reader));
        }

        return list;
    }

    public bool DeleteByHcID(decimal hcId)
    {
        EnsureOpen();
        string sql = $"DELETE FROM {TableName} WHERE HcID = @HcID;";
        using var cmd = new SqliteCommand(sql, _connection);
        cmd.Parameters.AddWithValue("@HcID", hcId);

        return cmd.ExecuteNonQuery() > 0;
    }

    private static void AddParameters(SqliteCommand cmd, HcDiaChiItem item)
    {
        cmd.Parameters.AddWithValue("@rowguid", item.rowguid.ToString());
        cmd.Parameters.AddWithValue("@HcID", item.HcID);
        cmd.Parameters.AddWithValue("@HcChaID", item.HcChaID);
        cmd.Parameters.AddWithValue("@TinhChat", (int)item.TinhChat);
        cmd.Parameters.AddWithValue("@NuocID", item.NuocID);
        cmd.Parameters.AddWithValue("@BangID", item.BangID);
        cmd.Parameters.AddWithValue("@TtID", item.TtID);
        cmd.Parameters.AddWithValue("@QhID", item.QhID);
        cmd.Parameters.AddWithValue("@XpID", item.XpID);
        cmd.Parameters.AddWithValue("@XomID", item.XomID);
        cmd.Parameters.AddWithValue("@Ma", item.Ma ?? string.Empty);
        cmd.Parameters.AddWithValue("@Ten", item.Ten ?? string.Empty);
        cmd.Parameters.AddWithValue("@DienGiai", item.DienGiai ?? string.Empty);
        cmd.Parameters.AddWithValue("@TrangThai", item.TrangThai);
        cmd.Parameters.AddWithValue("@NgayCn", item.NgayCn);
    }

    private static HcDiaChiItem MapItem(SqliteDataReader reader)
    {
        return new HcDiaChiItem
        {
            rowguid = reader["rowguid"] != DBNull.Value && Guid.TryParse(reader["rowguid"].ToString(), out var g) ? g : Guid.Empty,
            HcID = GetDecimal(reader, "HcID"),
            HcChaID = GetDecimal(reader, "HcChaID"),
            TinhChat = (HanhChinhTinhChatEnum)GetInt32(reader, "TinhChat"),
            NuocID = GetDecimal(reader, "NuocID"),
            BangID = GetDecimal(reader, "BangID"),
            TtID = GetDecimal(reader, "TtID"),
            QhID = GetDecimal(reader, "QhID"),
            XpID = GetDecimal(reader, "XpID"),
            XomID = GetDecimal(reader, "XomID"),
            Ma = GetString(reader, "Ma"),
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

    private static int GetInt32(SqliteDataReader reader, string columnName)
    {
        var val = reader[columnName];
        return val != DBNull.Value ? Convert.ToInt32(val) : 0;
    }

    public static string CreateTable()
    {
        return $@"
        CREATE TABLE IF NOT EXISTS {TableName} (
            rowguid TEXT,
            HcID NUMERIC PRIMARY KEY,
            HcChaID NUMERIC,
            TinhChat INTEGER,
            NuocID NUMERIC,
            BangID NUMERIC,
            TtID NUMERIC,
            QhID NUMERIC,
            XpID NUMERIC,
            XomID NUMERIC,
            Ma TEXT,
            Ten TEXT,
            DienGiai TEXT,
            TrangThai NUMERIC,
            NgayCn REAL
        );";
    }
}

public class HcDiaChiItem
{
    public const string TableName = "HcDiaChi";

    public Guid rowguid { get; set; } = Guid.Empty;
    public decimal HcID { get; set; } = 0m;
    public decimal HcChaID { get; set; } = 0m;
    public HanhChinhTinhChatEnum TinhChat { get; set; } = 0;
    public decimal NuocID { get; set; } = 0m;
    public decimal BangID { get; set; } = 0m;
    public decimal TtID { get; set; } = 0m;
    public decimal QhID { get; set; } = 0m;
    public decimal XpID { get; set; } = 0m;
    public decimal XomID { get; set; } = 0m;
    public string Ma { get; set; } = string.Empty;
    public string Ten { get; set; } = string.Empty;
    public string DienGiai { get; set; } = string.Empty;
    public decimal TrangThai { get; set; } = 0m;
    public double NgayCn { get; set; } = 0;

    public HcDiaChiItem()
    {
    }
}