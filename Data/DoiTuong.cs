using System.Data;
using Microsoft.Data.Sqlite;

namespace BIPhone.Data;

public class DoiTuong
{
    public const string TableName = "DoiTuong";
    private SqliteConnection _connection = null!;

    // Singleton Instance
    public static DoiTuong Instance { get; } = new DoiTuong();

    // Constructor rỗng bắt buộc cho Singleton
    public DoiTuong() { }

    public DoiTuong(SqliteConnection connection)
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

    public bool Save(DoiTuongItem item)
    {
        if (item == null) return false;
        EnsureOpen();

        string sql = $@"
            INSERT INTO {TableName} (
                Rowguid, DtID, DtChaID, DtTcID, DtNhomID, DtLoaiID, DtKqhID, DtCapDo, DtHoaDon,
                ThuocTinhID, HcID, DpID, GpsLocation, CaNhanIdType, CaNhanIdNo, XungHo, PbID,
                NguonID, NvTao, NgayTao, Ma, Ten, DaiDien, DiaChi, DienThoai, Fax, Email,
                Website, Mst, MstDiaChi, MstMaNganSach, DienGiai, DienGiaiPopup, NgungGiaoDich, KieuTaiKhoan,
                LichThanhToanKieu, LichThanhToanNgayTu, LichThanhToanNgayDen, TrangThai, NgayCN
            ) VALUES (
                @Rowguid, @DtID, @DtChaID, @DtTcID, @DtNhomID, @DtLoaiID, @DtKqhID, @DtCapDo, @DtHoaDon,
                @ThuocTinhID, @HcID, @DpID, @GpsLocation, @CaNhanIdType, @CaNhanIdNo, @XungHo, @PbID,
                @NguonID, @NvTao, @NgayTao, @Ma, @Ten, @DaiDien, @DiaChi, @DienThoai, @Fax, @Email,
                @Website, @Mst, @MstDiaChi, @MstMaNganSach, @DienGiai, @DienGiaiPopup, @NgungGiaoDich, @KieuTaiKhoan,
                @LichThanhToanKieu, @LichThanhToanNgayTu, @LichThanhToanNgayDen, @TrangThai, @NgayCN
            )
            ON CONFLICT(DtID) DO UPDATE SET
                Rowguid = excluded.Rowguid,
                DtChaID = excluded.DtChaID,
                DtTcID = excluded.DtTcID,
                DtNhomID = excluded.DtNhomID,
                DtLoaiID = excluded.DtLoaiID,
                DtKqhID = excluded.DtKqhID,
                DtCapDo = excluded.DtCapDo,
                DtHoaDon = excluded.DtHoaDon,
                ThuocTinhID = excluded.ThuocTinhID,
                HcID = excluded.HcID,
                DpID = excluded.DpID,
                GpsLocation = excluded.GpsLocation,
                CaNhanIdType = excluded.CaNhanIdType,
                CaNhanIdNo = excluded.CaNhanIdNo,
                XungHo = excluded.XungHo,
                PbID = excluded.PbID,
                NguonID = excluded.NguonID,
                NvTao = excluded.NvTao,
                NgayTao = excluded.NgayTao,
                Ma = excluded.Ma,
                Ten = excluded.Ten,
                DaiDien = excluded.DaiDien,
                DiaChi = excluded.DiaChi,
                DienThoai = excluded.DienThoai,
                Fax = excluded.Fax,
                Email = excluded.Email,
                Website = excluded.Website,
                Mst = excluded.Mst,
                MstDiaChi = excluded.MstDiaChi,
                MstMaNganSach = excluded.MstMaNganSach,
                DienGiai = excluded.DienGiai,
                DienGiaiPopup = excluded.DienGiaiPopup,
                NgungGiaoDich = excluded.NgungGiaoDich,
                KieuTaiKhoan = excluded.KieuTaiKhoan,
                LichThanhToanKieu = excluded.LichThanhToanKieu,
                LichThanhToanNgayTu = excluded.LichThanhToanNgayTu,
                LichThanhToanNgayDen = excluded.LichThanhToanNgayDen,
                TrangThai = excluded.TrangThai,
                NgayCN = excluded.NgayCN;";

        using var cmd = new SqliteCommand(sql, _connection);
        AddParameters(cmd, item);

        return cmd.ExecuteNonQuery() > 0;
    }

    public DoiTuongItem? GetByDtID(decimal dtId)
    {
        EnsureOpen();
        string sql = $"SELECT * FROM {TableName} WHERE DtID = @DtID LIMIT 1;";
        using var cmd = new SqliteCommand(sql, _connection);
        cmd.Parameters.AddWithValue("@DtID", dtId);

        using var reader = cmd.ExecuteReader();
        if (reader.Read())
        {
            return MapItem(reader);
        }

        return null;
    }
    public DoiTuongItem? GetByTel(string _TelNumber)
    {
        if (string.IsNullOrWhiteSpace(_TelNumber) || _TelNumber.Trim().Length < 7) return null;
        EnsureOpen();
        // 1. Không đặt @TelNumber bên trong dấu nháy đơn '' trong câu lệnh SQL
        string sql = $"SELECT * FROM {TableName} WHERE DienThoai LIKE @TelNumber AND TrangThai IN (1,2) LIMIT 1;";

        using var cmd = new SqliteCommand(sql, _connection);
        // 2. Nối ký tự '%' trực tiếp vào giá trị tham số _TelNumber truyền vào
        cmd.Parameters.AddWithValue("@TelNumber", $"%{_TelNumber.Trim()}%");
        using var reader = cmd.ExecuteReader();
        if (reader.Read())
        {
            return MapItem(reader);
        }
        return null;
    }

    public List<DoiTuongItem> GetAll()
    {
        EnsureOpen();
        var list = new List<DoiTuongItem>();

        string sql = $"SELECT * FROM {TableName};";
        using var cmd = new SqliteCommand(sql, _connection);
        using var reader = cmd.ExecuteReader();

        while (reader.Read())
        {
            list.Add(MapItem(reader));
        }

        return list;
    }

    public bool DeleteByDtID(decimal dtId)
    {
        EnsureOpen();
        string sql = $"DELETE FROM {TableName} WHERE DtID = @DtID;";
        using var cmd = new SqliteCommand(sql, _connection);
        cmd.Parameters.AddWithValue("@DtID", dtId);

        return cmd.ExecuteNonQuery() > 0;
    }

    private static void AddParameters(SqliteCommand cmd, DoiTuongItem item)
    {
        cmd.Parameters.AddWithValue("@Rowguid", item.Rowguid.ToString());
        cmd.Parameters.AddWithValue("@DtID", item.DtID);
        cmd.Parameters.AddWithValue("@DtChaID", item.DtChaID);
        cmd.Parameters.AddWithValue("@DtTcID", item.DtTcID ?? string.Empty);
        cmd.Parameters.AddWithValue("@DtNhomID", item.DtNhomID);
        cmd.Parameters.AddWithValue("@DtLoaiID", item.DtLoaiID);
        cmd.Parameters.AddWithValue("@DtKqhID", item.DtKqhID);
        cmd.Parameters.AddWithValue("@DtCapDo", item.DtCapDo);
        cmd.Parameters.AddWithValue("@DtHoaDon", item.DtHoaDon);
        cmd.Parameters.AddWithValue("@ThuocTinhID", item.ThuocTinhID);
        cmd.Parameters.AddWithValue("@HcID", item.HcID);
        cmd.Parameters.AddWithValue("@DpID", item.DpID);
        cmd.Parameters.AddWithValue("@GpsLocation", item.GpsLocation ?? string.Empty);
        cmd.Parameters.AddWithValue("@CaNhanIdType", item.CaNhanIdType);
        cmd.Parameters.AddWithValue("@CaNhanIdNo", item.CaNhanIdNo ?? string.Empty);
        cmd.Parameters.AddWithValue("@XungHo", item.XungHo ?? string.Empty);
        cmd.Parameters.AddWithValue("@PbID", item.PbID);
        cmd.Parameters.AddWithValue("@NguonID", item.NguonID);
        cmd.Parameters.AddWithValue("@NvTao", item.NvTao);
        cmd.Parameters.AddWithValue("@NgayTao", item.NgayTao.ToString("yyyy-MM-dd HH:mm:ss"));
        cmd.Parameters.AddWithValue("@Ma", item.Ma ?? string.Empty);
        cmd.Parameters.AddWithValue("@Ten", item.Ten ?? string.Empty);
        cmd.Parameters.AddWithValue("@DaiDien", item.DaiDien ?? string.Empty);
        cmd.Parameters.AddWithValue("@DiaChi", item.DiaChi ?? string.Empty);
        cmd.Parameters.AddWithValue("@DienThoai", item.DienThoai ?? string.Empty);
        cmd.Parameters.AddWithValue("@Fax", item.Fax ?? string.Empty);
        cmd.Parameters.AddWithValue("@Email", item.Email ?? string.Empty);
        cmd.Parameters.AddWithValue("@Website", item.Website ?? string.Empty);
        cmd.Parameters.AddWithValue("@Mst", item.Mst ?? string.Empty);
        cmd.Parameters.AddWithValue("@MstDiaChi", item.MstDiaChi ?? string.Empty);
        cmd.Parameters.AddWithValue("@MstMaNganSach", item.MstMaNganSach ?? string.Empty);
        cmd.Parameters.AddWithValue("@DienGiai", item.DienGiai ?? string.Empty);
        cmd.Parameters.AddWithValue("@DienGiaiPopup", item.DienGiaiPopup ?? string.Empty);
        cmd.Parameters.AddWithValue("@NgungGiaoDich", item.NgungGiaoDich);
        cmd.Parameters.AddWithValue("@KieuTaiKhoan", item.KieuTaiKhoan);
        cmd.Parameters.AddWithValue("@LichThanhToanKieu", item.LichThanhToanKieu);
        cmd.Parameters.AddWithValue("@LichThanhToanNgayTu", item.LichThanhToanNgayTu);
        cmd.Parameters.AddWithValue("@LichThanhToanNgayDen", item.LichThanhToanNgayDen);
        cmd.Parameters.AddWithValue("@TrangThai", item.TrangThai);
        cmd.Parameters.AddWithValue("@NgayCN", item.NgayCN);
    }

    private static DoiTuongItem MapItem(SqliteDataReader reader)
    {
        return new DoiTuongItem
        {
            Rowguid = reader["Rowguid"] != DBNull.Value && Guid.TryParse(reader["Rowguid"].ToString(), out var g) ? g : Guid.Empty,
            DtID = GetDecimal(reader, "DtID"),
            DtChaID = GetDecimal(reader, "DtChaID"),
            DtTcID = GetString(reader, "DtTcID"),
            DtNhomID = GetDecimal(reader, "DtNhomID"),
            DtLoaiID = GetDecimal(reader, "DtLoaiID"),
            DtKqhID = GetDecimal(reader, "DtKqhID"),
            DtCapDo = GetDecimal(reader, "DtCapDo"),
            DtHoaDon = GetDecimal(reader, "DtHoaDon"),
            ThuocTinhID = GetDecimal(reader, "ThuocTinhID"),
            HcID = GetDecimal(reader, "HcID"),
            DpID = GetDecimal(reader, "DpID"),
            GpsLocation = GetString(reader, "GpsLocation"),
            CaNhanIdType = GetDecimal(reader, "CaNhanIdType"),
            CaNhanIdNo = GetString(reader, "CaNhanIdNo"),
            XungHo = GetString(reader, "XungHo"),
            PbID = GetDecimal(reader, "PbID"),
            NguonID = GetDecimal(reader, "NguonID"),
            NvTao = GetDecimal(reader, "NvTao"),
            NgayTao = reader["NgayTao"] != DBNull.Value && DateTime.TryParse(reader["NgayTao"].ToString(), out var dt) ? dt : new DateTime(1900, 1, 1),
            Ma = GetString(reader, "Ma"),
            Ten = GetString(reader, "Ten"),
            DaiDien = GetString(reader, "DaiDien"),
            DiaChi = GetString(reader, "DiaChi"),
            DienThoai = GetString(reader, "DienThoai"),
            Fax = GetString(reader, "Fax"),
            Email = GetString(reader, "Email"),
            Website = GetString(reader, "Website"),
            Mst = GetString(reader, "Mst"),
            MstDiaChi = GetString(reader, "MstDiaChi"),
            MstMaNganSach = GetString(reader, "MstMaNganSach"),
            DienGiai = GetString(reader, "DienGiai"),
            DienGiaiPopup = GetString(reader, "DienGiaiPopup"),
            NgungGiaoDich = GetDecimal(reader, "NgungGiaoDich"),
            KieuTaiKhoan = GetDecimal(reader, "KieuTaiKhoan"),
            LichThanhToanKieu = GetDecimal(reader, "LichThanhToanKieu"),
            LichThanhToanNgayTu = GetDecimal(reader, "LichThanhToanNgayTu"),
            LichThanhToanNgayDen = GetDecimal(reader, "LichThanhToanNgayDen"),
            TrangThai = GetDecimal(reader, "TrangThai"),
            NgayCN = GetDouble(reader, "NgayCN")
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
            Rowguid TEXT,
            DtID NUMERIC PRIMARY KEY,
            DtChaID NUMERIC,
            DtTcID TEXT,
            DtNhomID NUMERIC,
            DtLoaiID NUMERIC,
            DtKqhID NUMERIC,
            DtCapDo NUMERIC,
            DtHoaDon NUMERIC,
            ThuocTinhID NUMERIC,
            HcID NUMERIC,
            DpID NUMERIC,
            GpsLocation TEXT,
            CaNhanIdType NUMERIC,
            CaNhanIdNo TEXT,
            XungHo TEXT,
            PbID NUMERIC,
            NguonID NUMERIC,
            NvTao NUMERIC,
            NgayTao TEXT,
            Ma TEXT,
            Ten TEXT,
            DaiDien TEXT,
            DiaChi TEXT,
            DienThoai TEXT,
            Fax TEXT,
            Email TEXT,
            Website TEXT,
            Mst TEXT,
            MstDiaChi TEXT,
            MstMaNganSach TEXT,
            DienGiai TEXT,
            DienGiaiPopup TEXT,
            NgungGiaoDich NUMERIC,
            KieuTaiKhoan NUMERIC,
            LichThanhToanKieu NUMERIC,
            LichThanhToanNgayTu NUMERIC,
            LichThanhToanNgayDen NUMERIC,
            TrangThai NUMERIC,
            NgayCN REAL
        );";
    }

    /// <summary>
    /// Đếm tổng số bản ghi khách hàng có TrangThai IN (1, 2)
    /// </summary>
    /// <returns>Tổng số lượng bản ghi (long)</returns>
    public long GetTotalCount()
    {
        EnsureOpen();
        string sql = $"SELECT COUNT(1) FROM {TableName} WHERE TrangThai IN (1, 2);";

        using var cmd = new SqliteCommand(sql, _connection);

        try
        {
            var result = cmd.ExecuteScalar();
            if (result != null && result != DBNull.Value)
            {
                return Convert.ToInt64(result);
            }
        }
        catch (Exception ex)
        {
            LogWriter.WriteLine($"Lỗi GetTotalCount {TableName}: {ex.Message}");
        }
        return 0;
    }
}

public class DoiTuongItem
{   
    private static DateTime MinDate = new DateTime(1900, 1, 1);    

    public virtual Guid Rowguid { get; set; } = Guid.Empty;
    public decimal DtID { get; set; } = 0;
    public decimal DtChaID { get; set; } = 0;
    public string DtTcID { get; set; } = string.Empty;
    public decimal DtNhomID { get; set; } = 0;
    public decimal DtLoaiID { get; set; } = 0;
    public decimal DtKqhID { get; set; } = 0;
    public decimal DtCapDo { get; set; } = 0;
    public decimal DtHoaDon { get; set; } = 0;
    public decimal ThuocTinhID { get; set; } = 0;
    public decimal HcID { get; set; } = 0;
    public decimal DpID { get; set; } = 0;
    public string GpsLocation { get; set; } = string.Empty;
    public decimal CaNhanIdType { get; set; } = 0;
    public string CaNhanIdNo { get; set; } = string.Empty;
    public string XungHo { get; set; } = string.Empty;
    public decimal PbID { get; set; } = 0;
    public decimal NguonID { get; set; } = 0;
    public decimal NvTao { get; set; } = 0;
    public DateTime NgayTao { get; set; } = MinDate;
    public string Ma { get; set; } = string.Empty;
    public string Ten { get; set; } = string.Empty;
    public string DaiDien { get; set; } = string.Empty;
    public string DiaChi { get; set; } = string.Empty;
    public string DienThoai { get; set; } = string.Empty;
    public string Fax { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Website { get; set; } = string.Empty;
    public string Mst { get; set; } = string.Empty;
    public string MstDiaChi { get; set; } = string.Empty;
    public string MstMaNganSach { get; set; } = string.Empty;
    public string DienGiai { get; set; } = string.Empty;
    public string DienGiaiPopup { get; set; } = string.Empty;
    public decimal NgungGiaoDich { get; set; } = 0;

    /// <summary>Kiểu tài khoản: 1: shopping, 2: business</summary>
    public decimal KieuTaiKhoan { get; set; } = 0;

    public decimal LichThanhToanKieu { get; set; } = 0;
    public decimal LichThanhToanNgayTu { get; set; } = 0;
    public decimal LichThanhToanNgayDen { get; set; } = 0;

    public virtual decimal TrangThai { get; set; } = (decimal)TrangThaiEnum.HetHieuLuc;
    public virtual double NgayCN { get; set; } = 0.0;

    public bool FromDataRow(DataRow aRow)
    {
        if (aRow == null) return false;

        try
        {
            // 1. Rowguid
            if (aRow.Table.Columns.Contains("Rowguid") && aRow["Rowguid"] != DBNull.Value)
            {
                if (Guid.TryParse(aRow["Rowguid"].ToString(), out Guid g))
                    Rowguid = g;
            }
            // 2. DtID
            if (aRow.Table.Columns.Contains("DtID") && aRow["DtID"] != DBNull.Value)
            {
                DtID = Convert.ToDecimal(aRow["DtID"]);
            }
            // 3. DtChaID
            if (aRow.Table.Columns.Contains("DtChaID") && aRow["DtChaID"] != DBNull.Value)
            {
                DtChaID = Convert.ToDecimal(aRow["DtChaID"]);
            }
            if (aRow.Table.Columns.Contains("DtTcID") && aRow["DtTcID"] != DBNull.Value)
            {
                DtTcID = Convert.ToString(aRow["DtTcID"]);
            }
            if (aRow.Table.Columns.Contains("DtNhomID") && aRow["DtNhomID"] != DBNull.Value)
            {
                DtNhomID = Convert.ToDecimal(aRow["DtNhomID"]);
            }
            if (aRow.Table.Columns.Contains("DtKqhID") && aRow["DtKqhID"] != DBNull.Value)
            {
                DtKqhID = Convert.ToDecimal(aRow["DtKqhID"]);
            }
            if (aRow.Table.Columns.Contains("DtLoaiID") && aRow["DtLoaiID"] != DBNull.Value)
            {
                DtLoaiID = Convert.ToDecimal(aRow["DtLoaiID"]);
            }
            if (aRow.Table.Columns.Contains("DtCapDo") && aRow["DtCapDo"] != DBNull.Value)
            {
                DtCapDo = Convert.ToDecimal(aRow["DtCapDo"]);
            }
            if (aRow.Table.Columns.Contains("DtHoaDon") && aRow["DtHoaDon"] != DBNull.Value)
            {
                DtHoaDon = Convert.ToDecimal(aRow["DtHoaDon"]);
            }
            if (aRow.Table.Columns.Contains("ThuocTinhID") && aRow["ThuocTinhID"] != DBNull.Value)
            {
                ThuocTinhID = Convert.ToDecimal(aRow["ThuocTinhID"]);
            }
            if (aRow.Table.Columns.Contains("HcID") && aRow["HcID"] != DBNull.Value)
            {
                HcID = Convert.ToDecimal(aRow["HcID"]);
            }
            if (aRow.Table.Columns.Contains("DpID") && aRow["DpID"] != DBNull.Value)
            {
                DpID = Convert.ToDecimal(aRow["DpID"]);
            }
            if (aRow.Table.Columns.Contains("GpsLocation") && aRow["GpsLocation"] != DBNull.Value)
            {
                GpsLocation = Convert.ToString(aRow["GpsLocation"]);
            }
            if (aRow.Table.Columns.Contains("CaNhanIdType") && aRow["CaNhanIdType"] != DBNull.Value)
            {
                CaNhanIdType = Convert.ToDecimal(aRow["CaNhanIdType"]);
            }
            if (aRow.Table.Columns.Contains("CaNhanIdNo") && aRow["CaNhanIdNo"] != DBNull.Value)
            {
                CaNhanIdNo = Convert.ToString(aRow["CaNhanIdNo"]);
            }
            if (aRow.Table.Columns.Contains("XungHo") && aRow["XungHo"] != DBNull.Value)
            {
                XungHo = Convert.ToString(aRow["XungHo"]);
            }
            if (aRow.Table.Columns.Contains("PbID") && aRow["PbID"] != DBNull.Value)
            {
                PbID = Convert.ToDecimal(aRow["PbID"]);
            }
            if (aRow.Table.Columns.Contains("NguonID") && aRow["NguonID"] != DBNull.Value)
            {
                NguonID = Convert.ToDecimal(aRow["NguonID"]);
            }
            if (aRow.Table.Columns.Contains("NvTao") && aRow["NvTao"] != DBNull.Value)
            {
                NvTao = Convert.ToDecimal(aRow["NvTao"]);
            }
            if (aRow.Table.Columns.Contains("NgayTao") && aRow["NgayTao"] != DBNull.Value)
            {
                NgayTao = Convert.ToDateTime(aRow["NgayTao"]);
            }
            if (aRow.Table.Columns.Contains("Ma") && aRow["Ma"] != DBNull.Value)
            {
                Ma = Convert.ToString(aRow["Ma"]);
            }
            if (aRow.Table.Columns.Contains("Ten") && aRow["Ten"] != DBNull.Value)
            {
                Ten = Convert.ToString(aRow["Ten"]);
            }
            if (aRow.Table.Columns.Contains("DaiDien") && aRow["DaiDien"] != DBNull.Value)
            {
                DaiDien = Convert.ToString(aRow["DaiDien"]);
            }
            if (aRow.Table.Columns.Contains("DiaChi") && aRow["DiaChi"] != DBNull.Value)
            {
                DiaChi = Convert.ToString(aRow["DiaChi"]);
            }
            if (aRow.Table.Columns.Contains("DienThoai") && aRow["DienThoai"] != DBNull.Value)
            {
                DienThoai = Convert.ToString(aRow["DienThoai"]);
            }
            if (aRow.Table.Columns.Contains("Fax") && aRow["Fax"] != DBNull.Value)
            {
                Fax = Convert.ToString(aRow["Fax"]);
            }
            if (aRow.Table.Columns.Contains("Email") && aRow["Email"] != DBNull.Value)
            {
                Email = Convert.ToString(aRow["Email"]);
            }
            if (aRow.Table.Columns.Contains("Website") && aRow["Website"] != DBNull.Value)
            {
                Website = Convert.ToString(aRow["Website"]);
            }
            if (aRow.Table.Columns.Contains("Mst") && aRow["Mst"] != DBNull.Value)
            {
                Mst = Convert.ToString(aRow["Mst"]);
            }
            if (aRow.Table.Columns.Contains("MstDiaChi") && aRow["MstDiaChi"] != DBNull.Value)
            {
                MstDiaChi = Convert.ToString(aRow["MstDiaChi"]);
            }
            if (aRow.Table.Columns.Contains("MstMaNganSach") && aRow["MstMaNganSach"] != DBNull.Value)
            {
                MstMaNganSach = Convert.ToString(aRow["MstMaNganSach"]);
            }
            if (aRow.Table.Columns.Contains("DienGiai") && aRow["DienGiai"] != DBNull.Value)
            {
                DienGiai = Convert.ToString(aRow["DienGiai"]);
            }
            if (aRow.Table.Columns.Contains("DienGiaiPopup") && aRow["DienGiaiPopup"] != DBNull.Value)
            {
                DienGiaiPopup = Convert.ToString(aRow["DienGiaiPopup"]);
            }
            if (aRow.Table.Columns.Contains("NgungGiaoDich") && aRow["NgungGiaoDich"] != DBNull.Value)
            {
                NgungGiaoDich = Convert.ToDecimal(aRow["NgungGiaoDich"]);
            }
            if (aRow.Table.Columns.Contains("KieuTaiKhoan") && aRow["KieuTaiKhoan"] != DBNull.Value)
            {
                KieuTaiKhoan = Convert.ToDecimal(aRow["KieuTaiKhoan"]);
            }
            if (aRow.Table.Columns.Contains("LichThanhToanKieu") && aRow["LichThanhToanKieu"] != DBNull.Value)
            {
                LichThanhToanKieu = Convert.ToDecimal(aRow["LichThanhToanKieu"]);
            }
            if (aRow.Table.Columns.Contains("LichThanhToanNgayTu") && aRow["LichThanhToanNgayTu"] != DBNull.Value)
            {
                LichThanhToanNgayTu = Convert.ToDecimal(aRow["LichThanhToanNgayTu"]);
            }
            if (aRow.Table.Columns.Contains("LichThanhToanNgayDen") && aRow["LichThanhToanNgayDen"] != DBNull.Value)
            {
                LichThanhToanNgayDen = Convert.ToDecimal(aRow["LichThanhToanNgayDen"]);
            }
            if (aRow.Table.Columns.Contains("TrangThai") && aRow["TrangThai"] != DBNull.Value)
            {
                TrangThai = Convert.ToDecimal(aRow["TrangThai"]);
            }
            if (aRow.Table.Columns.Contains("NgayCN") && aRow["NgayCN"] != DBNull.Value)
            {
                NgayCN = Convert.ToDouble(aRow["NgayCN"]);
            }

            return true;
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"Lỗi FromDataRow DoiTuongItem: {ex.Message}");
            return false;
        }
    }

    // Các trường phụ trợ
    //public string DtNhomTen { get; set; } = string.Empty;
    //public string DtKqhTen { get; set; } = string.Empty;
    //public string DtLoaiTen { get; set; } = string.Empty;
    //public string HcTen { get; set; } = string.Empty;
    //public string DpTen { get; set; } = string.Empty;
    //public string PbTen { get; set; } = string.Empty;
    //public string NguonTen { get; set; } = string.Empty;
    //public string NvTaoTen { get; set; } = string.Empty;
    //public string KieuTaiKhoanTen { get; set; } = string.Empty;
    //public string LichThanhToanTen { get; set; } = string.Empty;
    //public string TrangThaiTen { get; set; } = string.Empty;
    //public string PbID_PQ { get; set; } = string.Empty;
}