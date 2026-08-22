using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.App.Usage;
using Java.Lang;

namespace BIPhone.Platforms.Android;
public enum ScreenStateEnum
{
    Unknown = 0,
    Locked = 1,
    Home = 2,
    Application = 3
}

public class AndroidScreenState
{
    private readonly Context _context;

    public AndroidScreenState(Context context)
    {
        _context = context;
    }

    public ScreenStateEnum GetScreenState()
    {
        try
        {
            // ==========================================
            // 1. Kiểm tra màn hình khóa
            // ==========================================

            KeyguardManager keyguardManager = _context.GetSystemService(Context.KeyguardService) as KeyguardManager;

            if (keyguardManager != null && keyguardManager.IsKeyguardLocked)
            {
                LogWriter.WriteLine("ScreenStateEnum.Locked keyguardManager != null");
                return ScreenStateEnum.Locked;
            }


            // ==========================================
            // 2. Tìm ứng dụng đang ở foreground
            // ==========================================

            UsageStatsManager usageStatsManager = _context.GetSystemService(Context.UsageStatsService) as UsageStatsManager;

            if (usageStatsManager == null)
            {
                LogWriter.WriteLine("ScreenStateEnum.Unknown usageStatsManager == null");
                return ScreenStateEnum.Unknown;
            }

            long endTime = JavaSystem.CurrentTimeMillis();
            long startTime = endTime - 10000; // 10 giây gần nhất

            IList<UsageStats> stats = usageStatsManager.QueryUsageStats(
                    UsageStatsInterval.Best, startTime, endTime);

            if (stats == null || stats.Count == 0)
            {
                LogWriter.WriteLine("ScreenStateEnum.Unknown stats == null");
                return ScreenStateEnum.Unknown;
            }

            // ==========================================
            // 3. Lấy package được sử dụng gần nhất
            // ==========================================

            UsageStats latest = null;

            foreach (UsageStats item in stats)
            {
                if (item == null)
                    continue;

                if (latest == null ||
                    item.LastTimeUsed > latest.LastTimeUsed)
                {
                    latest = item;
                    string _packageName = latest.PackageName;
                    LogWriter.WriteLine( "GetScreenState Package = " + _packageName);
                }
            }

            if (latest == null ||
                string.IsNullOrEmpty(latest.PackageName))
            {
                LogWriter.WriteLine("ScreenStateEnum.Unknown latest == null");
                return ScreenStateEnum.Unknown;
            }

            string packageName = latest.PackageName;
            LogWriter.WriteLine("GetScreenState packageName = " + packageName);

            // ==========================================
            // 4. Kiểm tra package có phải Home Launcher
            // ==========================================

            Intent homeIntent = new Intent(Intent.ActionMain);
            homeIntent.AddCategory( Intent.CategoryHome);
            homeIntent.AddCategory(Intent.CategoryDefault);

            ResolveInfo resolveInfo =_context.PackageManager.ResolveActivity(
                    homeIntent,PackageInfoFlags.MatchAll);

            if (resolveInfo != null && resolveInfo.ActivityInfo != null)
            {
                string homePackage = resolveInfo.ActivityInfo.PackageName;
                LogWriter.WriteLine("GetScreenState HomePackage = " + homePackage);
                if (string.Equals( packageName,homePackage, StringComparison.OrdinalIgnoreCase))
                {   
                    LogWriter.WriteLine("ScreenStateEnum.Home");
                    return ScreenStateEnum.Home;
                }
            }

            // ==========================================
            // 5. Không phải Launcher
            // ==========================================
            LogWriter.WriteLine("ScreenStateEnum.Application");
            return ScreenStateEnum.Application;
        }
        catch (System.Exception ex)
        {
            LogWriter.WriteLine("GetScreenState Error = " + ex.Message);
            return ScreenStateEnum.Unknown;
        }
    }
}