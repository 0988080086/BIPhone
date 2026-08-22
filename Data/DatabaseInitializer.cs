using Microsoft.Data.Sqlite;

namespace BIPhone.Data;

public static class DatabaseInitializer
{
    public const string DatabaseFilename = "AppSQLite.db3";

    public static string DatabasePath =>
        $"Data Source={Path.Combine(FileSystem.AppDataDirectory, DatabaseFilename)}";

    public static void EnsureDatabaseAndTableCreated()
    {
        // 1. Tự động cấu hình ConnectionString cho Singleton CrmDienThoai
        CrmDienThoai.Instance.Configure(DatabasePath);
        CrmDienThoaiKenh.Instance.Configure(DatabasePath);
        DanhMuc.Instance.Configure(DatabasePath);
        DanhMucLoai.Instance.Configure(DatabasePath);
        DoiTuong.Instance.Configure(DatabasePath);
        HangHoa.Instance.Configure(DatabasePath);

        // 2. Kiểm tra/Tạo thư mục CSDL
        var builder = new SqliteConnectionStringBuilder(DatabasePath);
        string dbFilePath = builder.DataSource;
        string? directory = Path.GetDirectoryName(dbFilePath);

        if (!string.IsNullOrEmpty(directory) && !Directory.Exists(directory))
        {
            Directory.CreateDirectory(directory);
        }

        // 3. Sử dụng Singleton Instance để khởi tạo bảng CrmDienThoai
        CrmDienThoai.Instance.InitTable();
        CrmDienThoaiKenh.Instance.InitTable();
        DanhMuc.Instance.InitTable();
        DanhMucLoai.Instance.InitTable();
        DoiTuong.Instance.InitTable();
        HangHoa.Instance.InitTable();
    }

    //public static bool CheckTableExists(SqliteConnection connection, string tableName)
    //{
    //    string sql = "SELECT COUNT(*) FROM sqlite_master WHERE type = 'table' AND name = @tableName;";

    //    using var cmd = new SqliteCommand(sql, connection);
    //    cmd.Parameters.AddWithValue("@tableName", tableName);

    //    long count = Convert.ToInt64(cmd.ExecuteScalar() ?? 0);
    //    return count > 0;
    //}

    //public static int SQLExecute(SqliteConnection connection, string SQLStr)
    //{
    //    if (connection == null || string.IsNullOrEmpty(SQLStr)) { return -1; }

    //    try
    //    {
    //        using var cmd = new SqliteCommand(SQLStr, connection);
    //        return cmd.ExecuteNonQuery();
    //    }
    //    catch (Exception ex)
    //    {
    //        LogWriter.WriteLine("SQLExecute error: " + ex.Message);
    //        return -1;
    //    }
    //}
}