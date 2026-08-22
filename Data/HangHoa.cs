using System.Data;
using Microsoft.Data.Sqlite;

namespace BIPhone.Data;

public class HangHoa
{
    public const string TableName = "HangHoa";
    private SqliteConnection _connection = null!;

    // Singleton Instance
    public static HangHoa Instance { get; } = new HangHoa();

    // Constructor rỗng bắt buộc cho Singleton
    public HangHoa() { }

    public HangHoa(SqliteConnection connection)
    {
        _connection = connection ?? throw new ArgumentNullException(nameof(connection));
    }

    // Phương thức Configure nhận đường dẫn chuỗi (Đồng bộ với các Class khác)
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

    public bool Save(HangHoaItem item)
    {
        if (item == null) return false;
        EnsureOpen();

        string sql = $@"
            INSERT INTO {TableName} (
                rowguid, HhID, HhTcID, HhNhomID, HhLoaiID, HhKieuID, ThuocTinhID, Ma, MaVach, Ten,
                DienGiai, Dvt1, Dvt2, Dvt2CongThuc, Dvt2QuyDoi, Dvt3, Dvt3CongThuc, Dvt3QuyDoi,
                DvtCongThucTieuDe, DvtCongThucGiaTri, BaoHanh, BaoHanhDvt, BaoTri, BaoTriDvt,
                HanSuDung, HanSuDungDvt, KiemDinh, KiemDinhDvt, QLTonKho, QLTheoLo, TonToiThieu,
                TonToiDa, ThueTTDB, TtdbID, ThueGtGt, VatID, TkVatTu, TkGiaVon, TkDoanhThu,
                TkHbTraLai, DisplayOnWebsite, TrangThai, NgayCn
            ) VALUES (
                @rowguid, @HhID, @HhTcID, @HhNhomID, @HhLoaiID, @HhKieuID, @ThuocTinhID, @Ma, @MaVach, @Ten,
                @DienGiai, @Dvt1, @Dvt2, @Dvt2CongThuc, @Dvt2QuyDoi, @Dvt3, @Dvt3CongThuc, @Dvt3QuyDoi,
                @DvtCongThucTieuDe, @DvtCongThucGiaTri, @BaoHanh, @BaoHanhDvt, @BaoTri, @BaoTriDvt,
                @HanSuDung, @HanSuDungDvt, @KiemDinh, @KiemDinhDvt, @QLTonKho, @QLTheoLo, @TonToiThieu,
                @TonToiDa, @ThueTTDB, @TtdbID, @ThueGtGt, @VatID, @TkVatTu, @TkGiaVon, @TkDoanhThu,
                @TkHbTraLai, @DisplayOnWebsite, @TrangThai, @NgayCn
            )
            ON CONFLICT(HhID) DO UPDATE SET
                rowguid = excluded.rowguid,
                HhTcID = excluded.HhTcID,
                HhNhomID = excluded.HhNhomID,
                HhLoaiID = excluded.HhLoaiID,
                HhKieuID = excluded.HhKieuID,
                ThuocTinhID = excluded.ThuocTinhID,
                Ma = excluded.Ma,
                MaVach = excluded.MaVach,
                Ten = excluded.Ten,
                DienGiai = excluded.DienGiai,
                Dvt1 = excluded.Dvt1,
                Dvt2 = excluded.Dvt2,
                Dvt2CongThuc = excluded.Dvt2CongThuc,
                Dvt2QuyDoi = excluded.Dvt2QuyDoi,
                Dvt3 = excluded.Dvt3,
                Dvt3CongThuc = excluded.Dvt3CongThuc,
                Dvt3QuyDoi = excluded.Dvt3QuyDoi,
                DvtCongThucTieuDe = excluded.DvtCongThucTieuDe,
                DvtCongThucGiaTri = excluded.DvtCongThucGiaTri,
                BaoHanh = excluded.BaoHanh,
                BaoHanhDvt = excluded.BaoHanhDvt,
                BaoTri = excluded.BaoTri,
                BaoTriDvt = excluded.BaoTriDvt,
                HanSuDung = excluded.HanSuDung,
                HanSuDungDvt = excluded.HanSuDungDvt,
                KiemDinh = excluded.KiemDinh,
                KiemDinhDvt = excluded.KiemDinhDvt,
                QLTonKho = excluded.QLTonKho,
                QLTheoLo = excluded.QLTheoLo,
                TonToiThieu = excluded.TonToiThieu,
                TonToiDa = excluded.TonToiDa,
                ThueTTDB = excluded.ThueTTDB,
                TtdbID = excluded.TtdbID,
                ThueGtGt = excluded.ThueGtGt,
                VatID = excluded.VatID,
                TkVatTu = excluded.TkVatTu,
                TkGiaVon = excluded.TkGiaVon,
                TkDoanhThu = excluded.TkDoanhThu,
                TkHbTraLai = excluded.TkHbTraLai,
                DisplayOnWebsite = excluded.DisplayOnWebsite,
                TrangThai = excluded.TrangThai,
                NgayCn = excluded.NgayCn;";

        using var cmd = new SqliteCommand(sql, _connection);
        AddParameters(cmd, item);

        return cmd.ExecuteNonQuery() > 0;
    }

    public HangHoaItem? GetByHhID(decimal hhId)
    {
        EnsureOpen();
        string sql = $"SELECT * FROM {TableName} WHERE HhID = @HhID LIMIT 1;";
        using var cmd = new SqliteCommand(sql, _connection);
        cmd.Parameters.AddWithValue("@HhID", hhId);

        using var reader = cmd.ExecuteReader();
        if (reader.Read())
        {
            return MapItem(reader);
        }

        return null;
    }

    public List<HangHoaItem> GetAll()
    {
        EnsureOpen();
        var list = new List<HangHoaItem>();

        string sql = $"SELECT * FROM {TableName};";
        using var cmd = new SqliteCommand(sql, _connection);
        using var reader = cmd.ExecuteReader();

        while (reader.Read())
        {
            list.Add(MapItem(reader));
        }

        return list;
    }

    public bool DeleteByHhID(decimal hhId)
    {
        EnsureOpen();
        string sql = $"DELETE FROM {TableName} WHERE HhID = @HhID;";
        using var cmd = new SqliteCommand(sql, _connection);
        cmd.Parameters.AddWithValue("@HhID", hhId);

        return cmd.ExecuteNonQuery() > 0;
    }

    private static void AddParameters(SqliteCommand cmd, HangHoaItem item)
    {
        cmd.Parameters.AddWithValue("@rowguid", item.rowguid.ToString());
        cmd.Parameters.AddWithValue("@HhID", item.HhID);
        cmd.Parameters.AddWithValue("@HhTcID", item.HhTcID);
        cmd.Parameters.AddWithValue("@HhNhomID", item.HhNhomID);
        cmd.Parameters.AddWithValue("@HhLoaiID", item.HhLoaiID);
        cmd.Parameters.AddWithValue("@HhKieuID", item.HhKieuID);
        cmd.Parameters.AddWithValue("@ThuocTinhID", item.ThuocTinhID);
        cmd.Parameters.AddWithValue("@Ma", item.Ma ?? string.Empty);
        cmd.Parameters.AddWithValue("@MaVach", item.MaVach ?? string.Empty);
        cmd.Parameters.AddWithValue("@Ten", item.Ten ?? string.Empty);
        cmd.Parameters.AddWithValue("@DienGiai", item.DienGiai ?? string.Empty);
        cmd.Parameters.AddWithValue("@Dvt1", item.Dvt1 ?? string.Empty);
        cmd.Parameters.AddWithValue("@Dvt2", item.Dvt2 ?? string.Empty);
        cmd.Parameters.AddWithValue("@Dvt2CongThuc", item.Dvt2CongThuc ?? string.Empty);
        cmd.Parameters.AddWithValue("@Dvt2QuyDoi", item.Dvt2QuyDoi);
        cmd.Parameters.AddWithValue("@Dvt3", item.Dvt3 ?? string.Empty);
        cmd.Parameters.AddWithValue("@Dvt3CongThuc", item.Dvt3CongThuc ?? string.Empty);
        cmd.Parameters.AddWithValue("@Dvt3QuyDoi", item.Dvt3QuyDoi);
        cmd.Parameters.AddWithValue("@DvtCongThucTieuDe", item.DvtCongThucTieuDe ?? string.Empty);
        cmd.Parameters.AddWithValue("@DvtCongThucGiaTri", item.DvtCongThucGiaTri ?? string.Empty);
        cmd.Parameters.AddWithValue("@BaoHanh", item.BaoHanh);
        cmd.Parameters.AddWithValue("@BaoHanhDvt", item.BaoHanhDvt);
        cmd.Parameters.AddWithValue("@BaoTri", item.BaoTri);
        cmd.Parameters.AddWithValue("@BaoTriDvt", item.BaoTriDvt);
        cmd.Parameters.AddWithValue("@HanSuDung", item.HanSuDung);
        cmd.Parameters.AddWithValue("@HanSuDungDvt", item.HanSuDungDvt);
        cmd.Parameters.AddWithValue("@KiemDinh", item.KiemDinh);
        cmd.Parameters.AddWithValue("@KiemDinhDvt", item.KiemDinhDvt);
        cmd.Parameters.AddWithValue("@QLTonKho", item.QLTonKho);
        cmd.Parameters.AddWithValue("@QLTheoLo", item.QLTheoLo);
        cmd.Parameters.AddWithValue("@TonToiThieu", item.TonToiThieu);
        cmd.Parameters.AddWithValue("@TonToiDa", item.TonToiDa);
        cmd.Parameters.AddWithValue("@ThueTTDB", item.ThueTTDB);
        cmd.Parameters.AddWithValue("@TtdbID", item.TtdbID);
        cmd.Parameters.AddWithValue("@ThueGtGt", item.ThueGtGt);
        cmd.Parameters.AddWithValue("@VatID", item.VatID);
        cmd.Parameters.AddWithValue("@TkVatTu", item.TkVatTu);
        cmd.Parameters.AddWithValue("@TkGiaVon", item.TkGiaVon);
        cmd.Parameters.AddWithValue("@TkDoanhThu", item.TkDoanhThu);
        cmd.Parameters.AddWithValue("@TkHbTraLai", item.TkHbTraLai);
        cmd.Parameters.AddWithValue("@DisplayOnWebsite", item.DisplayOnWebsite);
        cmd.Parameters.AddWithValue("@TrangThai", item.TrangThai);
        cmd.Parameters.AddWithValue("@NgayCn", item.NgayCn);
    }

    private static HangHoaItem MapItem(SqliteDataReader reader)
    {
        return new HangHoaItem
        {
            rowguid = reader["rowguid"] != DBNull.Value && Guid.TryParse(reader["rowguid"].ToString(), out var g) ? g : Guid.Empty,
            HhID = GetDecimal(reader, "HhID"),
            HhTcID = GetDecimal(reader, "HhTcID"),
            HhNhomID = GetDecimal(reader, "HhNhomID"),
            HhLoaiID = GetDecimal(reader, "HhLoaiID"),
            HhKieuID = GetDecimal(reader, "HhKieuID"),
            ThuocTinhID = GetDecimal(reader, "ThuocTinhID"),
            Ma = GetString(reader, "Ma"),
            MaVach = GetString(reader, "MaVach"),
            Ten = GetString(reader, "Ten"),
            DienGiai = GetString(reader, "DienGiai"),
            Dvt1 = GetString(reader, "Dvt1"),
            Dvt2 = GetString(reader, "Dvt2"),
            Dvt2CongThuc = GetString(reader, "Dvt2CongThuc"),
            Dvt2QuyDoi = GetDouble(reader, "Dvt2QuyDoi"),
            Dvt3 = GetString(reader, "Dvt3"),
            Dvt3CongThuc = GetString(reader, "Dvt3CongThuc"),
            Dvt3QuyDoi = GetDouble(reader, "Dvt3QuyDoi"),
            DvtCongThucTieuDe = GetString(reader, "DvtCongThucTieuDe"),
            DvtCongThucGiaTri = GetString(reader, "DvtCongThucGiaTri"),
            BaoHanh = GetDouble(reader, "BaoHanh"),
            BaoHanhDvt = GetDecimal(reader, "BaoHanhDvt"),
            BaoTri = GetDouble(reader, "BaoTri"),
            BaoTriDvt = GetDecimal(reader, "BaoTriDvt"),
            HanSuDung = GetDouble(reader, "HanSuDung"),
            HanSuDungDvt = GetDecimal(reader, "HanSuDungDvt"),
            KiemDinh = GetDouble(reader, "KiemDinh"),
            KiemDinhDvt = GetDecimal(reader, "KiemDinhDvt"),
            QLTonKho = GetDecimal(reader, "QLTonKho"),
            QLTheoLo = GetDecimal(reader, "QLTheoLo"),
            TonToiThieu = GetDouble(reader, "TonToiThieu"),
            TonToiDa = GetDouble(reader, "TonToiDa"),
            ThueTTDB = GetDouble(reader, "ThueTTDB"),
            TtdbID = GetDecimal(reader, "TtdbID"),
            ThueGtGt = GetDouble(reader, "ThueGtGt"),
            VatID = GetDecimal(reader, "VatID"),
            TkVatTu = GetDecimal(reader, "TkVatTu"),
            TkGiaVon = GetDecimal(reader, "TkGiaVon"),
            TkDoanhThu = GetDecimal(reader, "TkDoanhThu"),
            TkHbTraLai = GetDecimal(reader, "TkHbTraLai"),
            DisplayOnWebsite = GetDecimal(reader, "DisplayOnWebsite"),
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
            HhID NUMERIC PRIMARY KEY,
            HhTcID NUMERIC,
            HhNhomID NUMERIC,
            HhLoaiID NUMERIC,
            HhKieuID NUMERIC,
            ThuocTinhID NUMERIC,
            Ma TEXT,
            MaVach TEXT,
            Ten TEXT,
            DienGiai TEXT,
            Dvt1 TEXT,
            Dvt2 TEXT,
            Dvt2CongThuc TEXT,
            Dvt2QuyDoi REAL,
            Dvt3 TEXT,
            Dvt3CongThuc TEXT,
            Dvt3QuyDoi REAL,
            DvtCongThucTieuDe TEXT,
            DvtCongThucGiaTri TEXT,
            BaoHanh REAL,
            BaoHanhDvt NUMERIC,
            BaoTri REAL,
            BaoTriDvt NUMERIC,
            HanSuDung REAL,
            HanSuDungDvt NUMERIC,
            KiemDinh REAL,
            KiemDinhDvt NUMERIC,
            QLTonKho NUMERIC,
            QLTheoLo NUMERIC,
            TonToiThieu REAL,
            TonToiDa REAL,
            ThueTTDB REAL,
            TtdbID NUMERIC,
            ThueGtGt REAL,
            VatID NUMERIC,
            TkVatTu NUMERIC,
            TkGiaVon NUMERIC,
            TkDoanhThu NUMERIC,
            TkHbTraLai NUMERIC,
            DisplayOnWebsite NUMERIC,
            TrangThai NUMERIC,
            NgayCn REAL
        );";
    }
}

public class HangHoaItem
{   
    public Guid rowguid { get; set; } = Guid.Empty;
    public decimal HhID { get; set; } = 0;
    public decimal HhTcID { get; set; } = 0;
    public decimal HhNhomID { get; set; } = 0;
    public decimal HhLoaiID { get; set; } = 0;
    public decimal HhKieuID { get; set; } = 0;
    public decimal ThuocTinhID { get; set; } = 0;
    public string Ma { get; set; } = string.Empty;
    public string MaVach { get; set; } = string.Empty;
    public string Ten { get; set; } = string.Empty;
    public string DienGiai { get; set; } = string.Empty;
    public string Dvt1 { get; set; } = string.Empty;
    public string Dvt2 { get; set; } = string.Empty;
    public string Dvt2CongThuc { get; set; } = string.Empty;
    public double Dvt2QuyDoi { get; set; } = 0;
    public string Dvt3 { get; set; } = string.Empty;
    public string Dvt3CongThuc { get; set; } = string.Empty;
    public double Dvt3QuyDoi { get; set; } = 0;
    public string DvtCongThucTieuDe { get; set; } = string.Empty;
    public string DvtCongThucGiaTri { get; set; } = string.Empty;
    public double BaoHanh { get; set; } = 0;
    public decimal BaoHanhDvt { get; set; } = 0;
    public double BaoTri { get; set; } = 0;
    public decimal BaoTriDvt { get; set; } = 0;
    public double HanSuDung { get; set; } = 0;
    public decimal HanSuDungDvt { get; set; } = 0;
    public double KiemDinh { get; set; } = 0;
    public decimal KiemDinhDvt { get; set; } = 0;
    public decimal QLTonKho { get; set; } = 0;
    public decimal QLTheoLo { get; set; } = 0;
    public double TonToiThieu { get; set; } = 0;
    public double TonToiDa { get; set; } = 0;
    public double ThueTTDB { get; set; } = 0;
    public decimal TtdbID { get; set; } = 0;
    public double ThueGtGt { get; set; } = 0;
    public decimal VatID { get; set; } = 0;
    public decimal TkVatTu { get; set; } = 0;
    public decimal TkGiaVon { get; set; } = 0;
    public decimal TkDoanhThu { get; set; } = 0;
    public decimal TkHbTraLai { get; set; } = 0;
    public decimal DisplayOnWebsite { get; set; } = 0;
    public decimal TrangThai { get; set; } = 0;
    public double NgayCn { get; set; } = 0;
}