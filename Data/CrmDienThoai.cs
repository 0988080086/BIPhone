using Microsoft.Data.Sqlite;
using System.Globalization;

namespace BIPhone.Data;

public sealed class CrmDienThoai
{
    public static readonly string TableName = "CrmDienThoai";
    private static readonly Lazy<CrmDienThoai> _instance = new Lazy<CrmDienThoai>(() => new CrmDienThoai());
    private string _connectionString = string.Empty;
    private readonly object _lockObj = new object();

    private CrmDienThoai() { }

    public static CrmDienThoai Instance => _instance.Value;

    public void Configure(string connectionString)
    {
        lock (_lockObj)
        {
            _connectionString = connectionString;
        }
    }

    #region Helper Connection Executions
    private T ExecuteWithConnection<T>(Func<SqliteConnection, T> action)
    {
        if (string.IsNullOrEmpty(_connectionString))
        {
            throw new InvalidOperationException("Chưa cấu hình connectionString. Hãy gọi CrmDienThoai.Instance.Configure(connectionString) trước.");
        }

        using var connection = new SqliteConnection(_connectionString);
        connection.Open();
        return action(connection);
    }

    private void ExecuteWithConnection(Action<SqliteConnection> action)
    {
        ExecuteWithConnection<bool>(conn =>
        {
            action(conn);
            return true;
        });
    }

    private async Task<T> ExecuteWithConnectionAsync<T>(Func<SqliteConnection, Task<T>> action)
    {
        if (string.IsNullOrEmpty(_connectionString))
        {
            throw new InvalidOperationException("Chưa cấu hình connectionString. Hãy gọi CrmDienThoai.Instance.Configure(connectionString) trước.");
        }

        using var connection = new SqliteConnection(_connectionString);
        await connection.OpenAsync();
        return await action(connection);
    }
    #endregion

    public void InitTable()
    {
        ExecuteWithConnection(connection =>
        {
            using var cmd = new SqliteCommand(CreateTable(), connection);
            cmd.ExecuteNonQuery();
        });
    }

    public async Task<bool> Save(CrmDienThoaiItem item)
    {
        if (item == null) return false;

        return ExecuteWithConnection(connection =>
        {
            string sql = $@"
                INSERT INTO {TableName} (
                    UID, rowguid, SvrID, CpuID, PbID, NhanVienID, TelID, CallID, KenhID,
                    Kenh, KenhSoMay, Huong, DienThoai, Ngay, BatDau, NhomHoiDam, TepGhiAm,
                    DtID, DtMa, DtTen, DtDiaChi, DtDienThoai, SanPhamID, NoiDung, Loi,
                    LoiThongBao, KetThuc, TinhTrang, Source, TrangThai, NgayCn, Synced
                ) VALUES (
                    @UID, @rowguid, @SvrID, @CpuID, @PbID, @NhanVienID, @TelID, @CallID, @KenhID,
                    @Kenh, @KenhSoMay, @Huong, @DienThoai, @Ngay, @BatDau, @NhomHoiDam, @TepGhiAm,
                    @DtID, @DtMa, @DtTen, @DtDiaChi, @DtDienThoai, @SanPhamID, @NoiDung, @Loi,
                    @LoiThongBao, @KetThuc, @TinhTrang, @Source, @TrangThai, @NgayCn, @Synced
                )
                ON CONFLICT(UID) DO UPDATE SET
                    rowguid = excluded.rowguid,
                    SvrID = excluded.SvrID,
                    CpuID = excluded.CpuID,
                    PbID = excluded.PbID,
                    NhanVienID = excluded.NhanVienID,
                    TelID = excluded.TelID,
                    CallID = excluded.CallID,
                    KenhID = excluded.KenhID,
                    Kenh = excluded.Kenh,
                    KenhSoMay = excluded.KenhSoMay,
                    Huong = excluded.Huong,
                    DienThoai = excluded.DienThoai,
                    Ngay = excluded.Ngay,
                    BatDau = excluded.BatDau,
                    NhomHoiDam = excluded.NhomHoiDam,
                    TepGhiAm = excluded.TepGhiAm,
                    DtID = excluded.DtID,
                    DtMa = excluded.DtMa,
                    DtTen = excluded.DtTen,
                    DtDiaChi = excluded.DtDiaChi,
                    DtDienThoai = excluded.DtDienThoai,                   
                    SanPhamID = excluded.SanPhamID,
                    NoiDung = excluded.NoiDung,
                    Loi = excluded.Loi,
                    LoiThongBao = excluded.LoiThongBao,
                    KetThuc = excluded.KetThuc,
                    TinhTrang = excluded.TinhTrang,
                    Source = excluded.Source,
                    TrangThai = excluded.TrangThai,
                    NgayCn = excluded.NgayCn,
                    Synced = excluded.Synced; ";

            using var cmd = new SqliteCommand(sql, connection);
            AddParameters(cmd, item);

            return cmd.ExecuteNonQuery() > 0;
        });
    }

    public async Task<CrmDienThoaiItem?> GetByUIDAsync(string uid)
    {
        return await ExecuteWithConnectionAsync(async connection =>
        {
            string sql = $"SELECT * FROM {TableName} WHERE UID = @UID LIMIT 1;";
            using var cmd = new SqliteCommand(sql, connection);
            cmd.Parameters.AddWithValue("@UID", uid ?? string.Empty);

            using var reader = await cmd.ExecuteReaderAsync();
            if (await reader.ReadAsync())
            {
                return MapItem(reader);
            }

            return null;
        });
    }

    public List<CrmDienThoaiItem> GetAll()
    {
        return ExecuteWithConnection(connection =>
        {
            var list = new List<CrmDienThoaiItem>();
            string sql = $"SELECT * FROM {TableName};";
            using var cmd = new SqliteCommand(sql, connection);
            using var reader = cmd.ExecuteReader();

            while (reader.Read())
            {
                list.Add(MapItem(reader));
            }

            return list;
        });
    }

    public bool DeleteByUID(string uid)
    {
        return ExecuteWithConnection(connection =>
        {
            string sql = $"DELETE FROM {TableName} WHERE UID = @UID;";
            using var cmd = new SqliteCommand(sql, connection);
            cmd.Parameters.AddWithValue("@UID", uid ?? string.Empty);

            return cmd.ExecuteNonQuery() > 0;
        });
    }

    private static void AddParameters(SqliteCommand cmd, CrmDienThoaiItem item)
    {
        cmd.Parameters.AddWithValue("@UID", item.UID ?? string.Empty);
        cmd.Parameters.AddWithValue("@rowguid", item.rowguid.ToString());
        cmd.Parameters.AddWithValue("@SvrID", item.SvrID);
        cmd.Parameters.AddWithValue("@CpuID", item.CpuID ?? string.Empty);
        cmd.Parameters.AddWithValue("@PbID", item.PbID);
        cmd.Parameters.AddWithValue("@NhanVienID", item.NhanVienID);
        cmd.Parameters.AddWithValue("@TelID", item.TelID);
        cmd.Parameters.AddWithValue("@CallID", item.CallID);
        cmd.Parameters.AddWithValue("@KenhID", item.KenhID);
        cmd.Parameters.AddWithValue("@Kenh", item.Kenh ?? string.Empty);
        cmd.Parameters.AddWithValue("@KenhSoMay", item.KenhSoMay ?? string.Empty);
        cmd.Parameters.AddWithValue("@Huong", item.Huong);
        cmd.Parameters.AddWithValue("@DienThoai", item.DienThoai ?? string.Empty);
        cmd.Parameters.AddWithValue("@Ngay", item.Ngay.ToString("o"));
        cmd.Parameters.AddWithValue("@BatDau", item.BatDau ?? "00:00:00");
        cmd.Parameters.AddWithValue("@NhomHoiDam", item.NhomHoiDam ?? string.Empty);
        cmd.Parameters.AddWithValue("@TepGhiAm", item.TepGhiAm ?? string.Empty);
        cmd.Parameters.AddWithValue("@DtID", item.DtID);
        cmd.Parameters.AddWithValue("@DtMa", item.DtMa ?? string.Empty);
        cmd.Parameters.AddWithValue("@DtTen", item.DtTen ?? string.Empty);
        cmd.Parameters.AddWithValue("@DtDiaChi", item.DtDiaChi ?? string.Empty);
        cmd.Parameters.AddWithValue("@DtDienThoai", item.DtDienThoai ?? string.Empty);
        cmd.Parameters.AddWithValue("@SanPhamID", item.SanPhamID);
        cmd.Parameters.AddWithValue("@NoiDung", item.NoiDung ?? string.Empty);
        cmd.Parameters.AddWithValue("@Loi", item.Loi);
        cmd.Parameters.AddWithValue("@LoiThongBao", item.LoiThongBao ?? string.Empty);
        cmd.Parameters.AddWithValue("@KetThuc", item.KetThuc ?? "00:00:00");
        cmd.Parameters.AddWithValue("@TinhTrang", item.TinhTrang);
        cmd.Parameters.AddWithValue("@Source", item.Source);
        cmd.Parameters.AddWithValue("@TrangThai", item.TrangThai);
        cmd.Parameters.AddWithValue("@NgayCn", item.NgayCn);
        cmd.Parameters.AddWithValue("@Synced", item.Synced);
    }

    private static CrmDienThoaiItem MapItem(SqliteDataReader reader)
    {
        return new CrmDienThoaiItem
        {
            UID = reader["UID"]?.ToString() ?? string.Empty,
            rowguid = reader["rowguid"] != DBNull.Value && Guid.TryParse(reader["rowguid"].ToString(), out var g) ? g : Guid.Empty,
            SvrID = GetDecimal(reader, "SvrID"),
            CpuID = reader["CpuID"]?.ToString() ?? string.Empty,
            PbID = GetDecimal(reader, "PbID"),
            NhanVienID = GetDecimal(reader, "NhanVienID"),
            TelID = GetDecimal(reader, "TelID"),
            CallID = GetDecimal(reader, "CallID"),
            KenhID = GetDecimal(reader, "KenhID"),
            Kenh = reader["Kenh"]?.ToString() ?? string.Empty,
            KenhSoMay = reader["KenhSoMay"]?.ToString() ?? string.Empty,
            Huong = GetDecimal(reader, "Huong"),
            DienThoai = reader["DienThoai"]?.ToString() ?? string.Empty,
            Ngay = reader["Ngay"] != DBNull.Value && DateTime.TryParse(reader["Ngay"].ToString(), out var dt) ? dt : DateTime.MinValue,
            BatDau = reader["BatDau"]?.ToString() ?? "00:00:00",
            NhomHoiDam = reader["NhomHoiDam"]?.ToString() ?? string.Empty,
            TepGhiAm = reader["TepGhiAm"]?.ToString() ?? string.Empty,
            DtID = GetDecimal(reader, "DtID"),
            DtMa = reader["DtMa"]?.ToString() ?? string.Empty,
            DtTen = reader["DtTen"]?.ToString() ?? string.Empty,
            DtDiaChi = reader["DtDiaChi"]?.ToString() ?? string.Empty,
            DtDienThoai = reader["DtDienThoai"]?.ToString() ?? string.Empty,
            SanPhamID = GetDecimal(reader, "SanPhamID"),
            NoiDung = reader["NoiDung"]?.ToString() ?? string.Empty,
            Loi = GetDecimal(reader, "Loi"),
            LoiThongBao = reader["LoiThongBao"]?.ToString() ?? string.Empty,
            KetThuc = reader["KetThuc"]?.ToString() ?? "00:00:00",
            TinhTrang = GetDecimal(reader, "TinhTrang"),
            Source = GetDecimal(reader, "Source"),
            TrangThai = GetDecimal(reader, "TrangThai"),
            NgayCn = reader["NgayCn"] != DBNull.Value ? Convert.ToDouble(reader["NgayCn"]) : 0.0,
            Synced = GetDecimal(reader, "Synced"),
        };
    }

    private static decimal GetDecimal(SqliteDataReader reader, string columnName)
    {
        if (reader[columnName] == DBNull.Value) return 0m;
        return Convert.ToDecimal(reader[columnName]);
    }

    public static string CreateTable()
    {
        return $@"
        CREATE TABLE IF NOT EXISTS {TableName} (
            UID TEXT UNIQUE,
            rowguid TEXT,
            SvrID NUMERIC,
            CpuID TEXT,
            PbID NUMERIC,
            NhanVienID NUMERIC,
            TelID NUMERIC,
            CallID NUMERIC,
            KenhID NUMERIC,
            Kenh TEXT,
            KenhSoMay TEXT,
            Huong NUMERIC,
            DienThoai TEXT,
            Ngay TEXT,
            BatDau TEXT,
            NhomHoiDam TEXT,
            TepGhiAm TEXT,
            DtID NUMERIC,
            DtMa TEXT,
            DtTen TEXT,
            DtDiaChi TEXT,
            DtDienThoai TEXT,
            SanPhamID NUMERIC,
            NoiDung TEXT,
            Loi NUMERIC,
            LoiThongBao TEXT,
            KetThuc TEXT,
            TinhTrang NUMERIC,
            Source NUMERIC,
            TrangThai NUMERIC,
            NgayCn REAL,
            Synced NUMERIC
        );";
    }

    public List<CrmDienThoaiItem> GetTop100Desc()
    {
        return ExecuteWithConnection(connection =>
        {
            var list = new List<CrmDienThoaiItem>();
            string sql = $@"
                SELECT * FROM {TableName} 
                ORDER BY UID DESC 
                LIMIT 100;";

            using var cmd = new SqliteCommand(sql, connection);
            using var reader = cmd.ExecuteReader();

            while (reader.Read())
            {
                list.Add(MapItem(reader));
            }

            return list;
        });
    }
}

public class CrmDienThoaiItem
{
    public Guid rowguid { get; set; } = Guid.Empty;
    public decimal SvrID { get; set; } = 0;
    public string CpuID { get; set; } = string.Empty;
    public decimal PbID { get; set; } = 0;
    public decimal NhanVienID { get; set; } = 0;
    public decimal TelID { get; set; } = 0;
    public string UID { get; set; } = string.Empty;
    public decimal CallID { get; set; } = 0;
    public decimal KenhID { get; set; } = 0;
    public string Kenh { get; set; } = string.Empty;
    public string KenhSoMay { get; set; } = string.Empty;
    public decimal Huong { get; set; } = 0;
    public string DienThoai { get; set; } = string.Empty;
    public DateTime Ngay { get; set; } = DateTime.MinValue;
    public string BatDau { get; set; } = "00:00:00";
    public string NhomHoiDam { get; set; } = string.Empty;
    public string TepGhiAm { get; set; } = string.Empty;
    public decimal DtID { get; set; } = 0m;
    public string DtMa { get; set; } = string.Empty;
    public string DtTen { get; set; } = string.Empty;
    public string DtDiaChi { get; set; } = string.Empty;
    public string DtDienThoai { get; set; } = string.Empty;
    public decimal SanPhamID { get; set; } = 0m;
    public string NoiDung { get; set; } = string.Empty;
    public decimal Loi { get; set; } = 0m;
    public string LoiThongBao { get; set; } = string.Empty;
    public string KetThuc { get; set; } = "00:00:00";
    public decimal TinhTrang { get; set; } = (decimal)CrmTelTinhTrangEnum.Unknown;
    public decimal Source { get; set; } = (decimal)TelSourceEnum.Unknown;
    public decimal TrangThai { get; set; } = 0m;
    public double NgayCn { get; set; } = 0.0;
    public decimal Synced { get; set; } = 0m;

    //public bool IsSynced => Synced > 0;
    public bool IsSynced => true;

    public CrmDienThoaiItem()
    {
        rowguid = Guid.Empty;
        SvrID = AppSettings.ServerID;
        Ngay = DateTime.Now;
        BatDau = "";
        KetThuc = "";
        DienThoai = "";
        TinhTrang = (decimal)CrmTelTinhTrangEnum.Unknown;
        Source = (decimal)TelSourceEnum.Unknown;
        UID = "";
        CallID = 0;
        Huong = (decimal)CrmHuongEnum._Internal;
        Synced = 0;
    }

    public void UpdateFrom(CrmDienThoaiItem source)
    {
        if (source == null) return;
        rowguid = source.rowguid;
        SvrID = source.SvrID;
        CpuID = source.CpuID;
        PbID = source.PbID;
        NhanVienID = source.NhanVienID;
        TelID = source.TelID;
        CallID = source.CallID;
        KenhID = source.KenhID;
        Kenh = source.Kenh;
        KenhSoMay = source.KenhSoMay;
        Huong = source.Huong;
        DienThoai = source.DienThoai;
        Ngay = source.Ngay;
        BatDau = source.BatDau;
        NhomHoiDam = source.NhomHoiDam;
        TepGhiAm = source.TepGhiAm;
        DtID = source.DtID;
        DtMa = source.DtMa;
        DtTen = source.DtTen;
        DtDiaChi = source.DtDiaChi;
        DtDienThoai = source.DtDienThoai;
        SanPhamID = source.SanPhamID;
        NoiDung = source.NoiDung;
        Loi = source.Loi;
        LoiThongBao = source.LoiThongBao;
        KetThuc = source.KetThuc;
        TinhTrang = source.TinhTrang;
        Source = source.Source;
        TrangThai = source.TrangThai;
        NgayCn = source.NgayCn;
        Synced = source.Synced;
    }

    public void UpdateDoiTuong()
    {
        if (DtID <= 0 && string.IsNullOrEmpty(DienThoai)) { return; }

        DoiTuongItem _DtItem = null;
        if (DtID > 0)
        {
            _DtItem = DoiTuong.Instance.GetByDtID(DtID);
        }
        else if (!string.IsNullOrEmpty(DienThoai) && DienThoai.Length > 6)
        {
            _DtItem = DoiTuong.Instance.GetByTel(DienThoai);
        }

        if (_DtItem != null)
        {
            if (DtID != _DtItem.DtID)
                DtID = _DtItem.DtID;
            if (!string.IsNullOrEmpty(_DtItem.Ma))
                DtMa = _DtItem.Ma;
            if (!string.IsNullOrEmpty(_DtItem.Ten))
                DtTen = _DtItem.Ten;
            if (!string.IsNullOrEmpty(_DtItem.DiaChi))
                DtDiaChi = _DtItem.DiaChi;
            if (!string.IsNullOrEmpty(_DtItem.DienThoai))
                DtDienThoai = _DtItem.DienThoai;
        }
    }

    public CrmDienThoaiItem Clone()
    {
        return (CrmDienThoaiItem)MemberwiseClone();
    }
}