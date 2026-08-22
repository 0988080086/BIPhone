using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Telecom;
using Android.Util;
using Android.Views;
using System;
using System.Data;

namespace BIPhone.Platforms.Android;

public class OverlayManager
{
    #region Singleton Implementation
    private static readonly Lazy<OverlayManager> _instance = new Lazy<OverlayManager>(() => new OverlayManager());
    public static OverlayManager Instance => _instance.Value;
    #endregion

    private Context? _context;
    private global::Android.Views.IWindowManager? _windowManager;
    private global::Android.Views.View? _overlayView;
    private WindowManagerLayoutParams? _layoutParam;
    private bool _isShowing = false;

    // Handler đưa tác vụ về Main Thread
    private readonly global::Android.OS.Handler _mainHandler;
    // Tác vụ hẹn giờ tự động đóng
    private Action? _autoCloseAction;

    // Controls UI
    private global::Android.Widget.TextView? _lblPhone;
    private global::Android.Widget.ListView? _lstInfo;
    private global::Android.Widget.Button? _btnAnswer;
    private global::Android.Widget.Button? _btnReject;
    private global::Android.Widget.Button? _btnShare;
    private global::Android.Widget.Button? _btnClose;

    /// <summary>
    /// Cho biết Overlay có thực sự đang mở trên màn hình hay không
    /// </summary>
    public bool IsShowing => _isShowing && _overlayView != null && _overlayView.IsAttachedToWindow;

    private OverlayManager()
    {
        _mainHandler = new global::Android.OS.Handler(global::Android.OS.Looper.MainLooper);
    }

    /// <summary>
    /// Khởi tạo Context cho Singleton Manager. Gọi 1 lần duy nhất từ Application/Service/MainActivity.
    /// </summary>
    public void Initialize(Context context)
    {
        _context = context ?? throw new ArgumentNullException(nameof(context));
    }

    /// <summary>
    /// Hiển thị Popup Overlay:
    /// - Nếu đang mở: Tự động Cập nhật Dữ liệu mới (Refresh) + Reset hẹn giờ 60s (Tránh giật nháy màn hình).
    /// - Nếu đang đóng: Tạo mới View và Show lên màn hình.
    /// </summary>
    public void Show(CrmDienThoaiItem _Item)
    {
        if (_Item == null) return;

        _mainHandler.Post(() =>
        {
            if (_context == null)
            {
                LogWriter.WriteLine("OverlayManager Error: Context chưa được khởi tạo. Hãy gọi Initialize(context) trước.");
                return;
            }

            if (!global::Android.Provider.Settings.CanDrawOverlays(_context))
            {
                LogWriter.WriteLine("OverlayManager.Show không kích hoạt do chưa cấp quyền Overlay");
                return;
            }

            try
            {
                // Khởi tạo WindowManager nếu chưa có
                if (_windowManager == null)
                {
                    var serviceObj = _context.GetSystemService(Context.WindowService);
                    if (serviceObj == null) return;
                    _windowManager = serviceObj.JavaCast<global::Android.Views.IWindowManager>();
                    if (_windowManager == null) return;
                }

                // =========================================================================
                // TRƯỜNG HỢP 1: OVERLAY ĐANG HIỂN THỊ -> CẬP NHẬT DỮ LIỆU VÀ RESET TIMER
                // =========================================================================
                if (IsShowing)
                {
                    // 1. Cập nhật lại Số điện thoại
                    if (_lblPhone != null)
                    {
                        _lblPhone.Text = _Item.DienThoai;
                    }

                    // 2. Cập nhật lại Adapter dữ liệu
                    if (_lstInfo != null)
                    {
                        _lstInfo.Adapter = new OverlayData(_context, _Item);
                    }

                    // 3. Phục hồi hiển thị lại nút Nghe/Hủy (phòng trường hợp cuộc gọi trước đã bấm Nghe làm ẩn)
                    if (_btnAnswer != null) _btnAnswer.Visibility = ViewStates.Visible;
                    if (_btnReject != null) _btnReject.Visibility = ViewStates.Visible;

                    // 4. Tính toán và Cập nhật lại vị trí/kích thước Overlay
                    UpdateLayoutParams();
                    if (_overlayView != null && _layoutParam != null)
                    {
                        _windowManager.UpdateViewLayout(_overlayView, _layoutParam);
                    }

                    // 5. Reset hẹn giờ 60s tự đóng
                    ResetAutoCloseTimer();

                    LogWriter.WriteLine("OverlayManager Show: Đã cập nhật (Refresh) thông tin cuộc gọi mới thành công.");
                    return;
                }

                // =========================================================================
                // TRƯỜNG HỢP 2: OVERLAY ĐÃ ĐÓNG / CHƯA MỜ -> TẠO MỚI VIEW VÀ ADD TO WINDOW
                // =========================================================================

                // 1. Lấy kích thước thực tế màn hình
                var (screenWidth, screenHeight) = GetScreenSize();
                if (screenWidth == 0 || screenHeight == 0) return;

                // 2. Bơm (Inflate) View từ XML
                var themeContext = new ContextThemeWrapper(_context, global::Android.Resource.Style.ThemeDeviceDefaultLight);
                LayoutInflater? inflater = LayoutInflater.From(themeContext);
                if (inflater == null) return;

                _overlayView = inflater.Inflate(BIPhone.Resource.Layout.phonecall_overlay, null);
                if (_overlayView == null) return;

#pragma warning disable CA1416
                // 3. Cấu hình WindowManagerLayoutParams
                _layoutParam = new global::Android.Views.WindowManagerLayoutParams(
                    global::Android.Views.WindowManagerLayoutParams.MatchParent,
                    global::Android.Views.WindowManagerLayoutParams.WrapContent,
                    global::Android.Views.WindowManagerTypes.ApplicationOverlay,
                    global::Android.Views.WindowManagerFlags.NotTouchModal |
                    global::Android.Views.WindowManagerFlags.LayoutInScreen |
                    global::Android.Views.WindowManagerFlags.ShowWhenLocked |
                    global::Android.Views.WindowManagerFlags.TurnScreenOn |
                    global::Android.Views.WindowManagerFlags.DismissKeyguard,
                    global::Android.Graphics.Format.Opaque);

                if (global::Android.OS.Build.VERSION.SdkInt >= global::Android.OS.BuildVersionCodes.O)
                {
                    _layoutParam.Flags |= global::Android.Views.WindowManagerFlags.KeepScreenOn;
                }
#pragma warning restore CA1416

                // 4. Định vị trí hiển thị
                UpdateLayoutParams();

                // 5. Ánh xạ Controls
                _lblPhone = _overlayView.FindViewById<global::Android.Widget.TextView>(BIPhone.Resource.Id.lblPhone);
                _lstInfo = _overlayView.FindViewById<global::Android.Widget.ListView>(BIPhone.Resource.Id.lstInfo);
                _btnAnswer = _overlayView.FindViewById<global::Android.Widget.Button>(BIPhone.Resource.Id.btnAnswer);
                _btnReject = _overlayView.FindViewById<global::Android.Widget.Button>(BIPhone.Resource.Id.btnReject);
                _btnShare = _overlayView.FindViewById<global::Android.Widget.Button>(BIPhone.Resource.Id.btnShare);
                _btnClose = _overlayView.FindViewById<global::Android.Widget.Button>(BIPhone.Resource.Id.btnClose);

                // 6. Đổ dữ liệu ban đầu
                if (_lblPhone != null) _lblPhone.Text = _Item.DienThoai;
                if (_lstInfo != null) _lstInfo.Adapter = new OverlayData(_context, _Item);

                // 7. Gán sự kiện cho Nút bấm
                if (_btnAnswer != null)
                {
                    _btnAnswer.Click += (s, e) =>
                    {
                        AnswerCall();
                        if (_btnAnswer != null) _btnAnswer.Visibility = ViewStates.Gone;
                        if (_btnReject != null) _btnReject.Visibility = ViewStates.Gone;
                    };
                }

                if (_btnReject != null)
                {
                    _btnReject.Click += (s, e) =>
                    {
                        try
                        {
                            EndCall();
                        }
                        catch (Exception ex)
                        {
                            LogWriter.WriteLine("Lỗi ngắt cuộc gọi: " + ex.Message);
                        }
                        finally
                        {
                            Close();
                        }
                    };
                }

                if (_btnShare != null)
                {
                    _btnShare.Click += (s, e) => { ShareInfo(); };
                }

                if (_btnClose != null)
                {
                    _btnClose.Click += (s, e) => { Close(); };
                }

                // 8. Đẩy cửa sổ hiển thị lên màn hình
                _windowManager.AddView(_overlayView, _layoutParam);
                _isShowing = true;

                // 9. Bật Timer 60s tự đóng
                ResetAutoCloseTimer();

                LogWriter.WriteLine("OverlayManager Show: Đã tạo mới và mở Popup Overlay thành công.");
            }
            catch (Exception ex)
            {
                _isShowing = false;
                _overlayView = null;
                LogWriter.WriteLine("OverlayManager Show Error: " + ex.ToString());
            }
        });
    }

    /// <summary>
    /// Đóng và giải phóng Popup Overlay
    /// </summary>
    public void Close()
    {
        _mainHandler.Post(() =>
        {
            try
            {
                // Hủy bộ đếm tự đóng
                CancelAutoCloseTimer();

                if (_isShowing && _windowManager != null && _overlayView != null && _overlayView.IsAttachedToWindow)
                {
                    _windowManager.RemoveView(_overlayView);
                }
            }
            catch (Exception ex)
            {
                LogWriter.WriteLine("OverlayManager Close Error: " + ex.Message);
            }
            finally
            {
                _lblPhone = null;
                _lstInfo = null;
                _btnAnswer = null;
                _btnReject = null;
                _btnShare = null;
                _btnClose = null;

                _overlayView?.Dispose();
                _overlayView = null;
                _isShowing = false;
                LogWriter.WriteLine("OverlayManager: Đã đóng Popup Overlay.");
            }
        });
    }

    /// <summary>
    /// Chia sẻ thông tin cuộc gọi sang ứng dụng khác (Zalo)
    /// </summary>
    public void ShareInfo()
    {
        _mainHandler.Post(() =>
        {
            try
            {
                if (_context == null || _lstInfo == null || _lstInfo.Adapter == null)
                {
                    LogWriter.WriteLine("ShareInfo thất bại: ListView hoặc Adapter đang trống.");
                    return;
                }

                if (_lstInfo.Adapter is OverlayData dataAdapter)
                {
                    DataTable currentTable = dataAdapter.ViewData;

                    if (currentTable == null || currentTable.Rows.Count == 0)
                    {
                        LogWriter.WriteLine("ShareInfo: DataTable rỗng, không có gì để chia sẻ.");
                        return;
                    }

                    System.Text.StringBuilder shareBuilder = new System.Text.StringBuilder();
                    shareBuilder.AppendLine("[THÔNG TIN KHÁCH HÀNG VNBIS]");

                    string currentPhone = _lblPhone?.Text ?? "Chưa rõ số";
                    shareBuilder.AppendLine($"- {currentPhone}");
                    shareBuilder.AppendLine("---------------------------");

                    foreach (DataRow row in currentTable.Rows)
                    {
                        string tieuDe = row["TieuDe"]?.ToString() ?? "";
                        string noiDung = row["NoiDung"]?.ToString() ?? "";

                        if (!string.IsNullOrEmpty(tieuDe) || !string.IsNullOrEmpty(noiDung))
                        {
                            shareBuilder.AppendLine($"- {tieuDe}: {noiDung}");
                        }
                    }

                    string finalShareContent = shareBuilder.ToString();
                    LogWriter.WriteLine("Nội dung text chuẩn bị gửi sang Zalo:\n" + finalShareContent);

                    global::Android.Content.Intent shareIntent = new global::Android.Content.Intent(global::Android.Content.Intent.ActionSend);
                    shareIntent.SetType("text/plain");
                    shareIntent.PutExtra(global::Android.Content.Intent.ExtraText, finalShareContent);
                    shareIntent.AddFlags(global::Android.Content.ActivityFlags.NewTask);

                    global::Android.Content.Intent chooserIntent = global::Android.Content.Intent.CreateChooser(shareIntent, "Chia sẻ thông tin cuộc gọi");
                    chooserIntent.AddFlags(global::Android.Content.ActivityFlags.NewTask);

                    _context.StartActivity(chooserIntent);
                }
                else
                {
                    LogWriter.WriteLine("ShareInfo lỗi: Adapter của ListView không phải là OverlayData.");
                }
            }
            catch (Exception ex)
            {
                LogWriter.WriteLine("Lỗi thực thi trong hàm ShareInfo: " + ex.Message);
            }
        });
    }

    #region Private Helper Methods

    private void UpdateLayoutParams()
    {
        if (_context == null || _layoutParam == null) return;

        var (screenWidth, screenHeight) = GetScreenSize();
        if (screenWidth == 0 || screenHeight == 0) return;

        long topMarginPercent = AppSettings.TopMarginPercent;
        long bottomMarginPercent = AppSettings.ButtonMarginPecent;

        BIPhone.Platforms.Android.AndroidScreenState clsScreenState = new BIPhone.Platforms.Android.AndroidScreenState(_context);
        BIPhone.Platforms.Android.ScreenStateEnum screenStateEnum = clsScreenState.GetScreenState();

        _layoutParam.Gravity = global::Android.Views.GravityFlags.Top | global::Android.Views.GravityFlags.CenterHorizontal;

        if (screenStateEnum == BIPhone.Platforms.Android.ScreenStateEnum.Locked ||
            screenStateEnum == BIPhone.Platforms.Android.ScreenStateEnum.Home)
        {
            _layoutParam.Y = 0;
            _layoutParam.Height = (int)(screenHeight * (100 - bottomMarginPercent) / 100);
        }
        else
        {
            _layoutParam.Y = (int)(screenHeight * topMarginPercent / 100);
            _layoutParam.Height = (int)(screenHeight * (100 - topMarginPercent - bottomMarginPercent) / 100);
        }
    }

    private void ResetAutoCloseTimer()
    {
        CancelAutoCloseTimer();
        _autoCloseAction = () => { Close(); };
        _mainHandler.PostDelayed(_autoCloseAction, 60000); // 60 Giây
    }

    private void CancelAutoCloseTimer()
    {
        if (_autoCloseAction != null)
        {
            _mainHandler.RemoveCallbacks(_autoCloseAction);
            _autoCloseAction = null;
        }
    }

    private bool IsScreenLocked()
    {
        try
        {
            if (_context == null) return false;
            var keyguardManager = _context.GetSystemService(Context.KeyguardService)?.JavaCast<KeyguardManager>();
            return keyguardManager != null && keyguardManager.IsKeyguardLocked;
        }
        catch (Exception ex)
        {
            LogWriter.WriteLine("IsScreenLocked Error: " + ex.Message);
            return false;
        }
    }

    private (int Width, int Height) GetScreenSize()
    {
        try
        {
            if (_context == null) return (0, 0);

            var windowManager = _context.GetSystemService(Context.WindowService)?.JavaCast<IWindowManager>();
            if (windowManager == null) return (0, 0);

            if (Build.VERSION.SdkInt >= BuildVersionCodes.R)
            {
                var metrics = windowManager.CurrentWindowMetrics;
                var bounds = metrics.Bounds;
                return (bounds.Width(), bounds.Height());
            }
            else
            {
                var displayMetrics = new DisplayMetrics();
#pragma warning disable CS0618
                windowManager.DefaultDisplay?.GetRealMetrics(displayMetrics);
#pragma warning restore CS0618
                return (displayMetrics.WidthPixels, displayMetrics.HeightPixels);
            }
        }
        catch (Exception ex)
        {
            LogWriter.WriteLine("GetScreenSize Error: " + ex.Message);
            return (0, 0);
        }
    }

    private void AnswerCall()
    {
        try
        {
            if (_context == null) return;
            var telecomManager = (TelecomManager)_context.GetSystemService(Context.TelecomService);
            if (telecomManager != null && Build.VERSION.SdkInt >= BuildVersionCodes.O)
            {
                telecomManager.AcceptRingingCall();
            }
        }
        catch (Exception ex)
        {
            LogWriter.WriteLine("Lỗi không thể trả lời cuộc gọi: " + ex.Message);
        }
    }

    private void EndCall()
    {
        try
        {
            if (_context == null) return;
            var telecomManager = (TelecomManager)_context.GetSystemService(Context.TelecomService);
            if (telecomManager != null)
            {
                if (Build.VERSION.SdkInt >= BuildVersionCodes.P)
                {
                    telecomManager.EndCall();
                }
                else
                {
                    try
                    {
                        var telephonyManager = (global::Android.Telephony.TelephonyManager)_context.GetSystemService(Context.TelephonyService);
                        var classTelephony = Java.Lang.Class.ForName(telephonyManager.Class.Name);
                        var methodGetITelephony = classTelephony.GetDeclaredMethod("getITelephony");
                        methodGetITelephony.Accessible = true;
                        var iTelephony = methodGetITelephony.Invoke(telephonyManager);
                        var classITelephony = Java.Lang.Class.ForName(iTelephony.Class.Name);
                        var methodEndCall = classITelephony.GetDeclaredMethod("endCall");
                        methodEndCall.Invoke(iTelephony);
                    }
                    catch (Exception exOld)
                    {
                        LogWriter.WriteLine("EndCall Reflection Error: " + exOld.Message);
                    }
                }
            }
        }
        catch (Exception ex)
        {
            LogWriter.WriteLine("Lỗi ngắt cuộc gọi: " + ex.Message);
        }
    }

    #endregion
}