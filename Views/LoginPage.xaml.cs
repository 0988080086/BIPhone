using System;
using System.Threading.Tasks;
using Microsoft.Maui.Storage;

namespace BIPhone.Views;

public partial class LoginPage : ContentPage
{
    private readonly ClsConnService mConnService;
    private bool mBusy = false;

    public LoginPage()
	{
		InitializeComponent();
        mConnService = ClsConnService.Instance;
        LoadSavedAccount();
    }
    private async Task LoadSavedAccount()
    {
        bool rememberAccount = AppSettings.LoginRememberAccount;
        bool rememberAutoLogin = AppSettings.LoginRememberAutoLogin;
        chkRememberAccount.IsChecked = rememberAccount;
        chkRememberAutoLogin.IsChecked = rememberAutoLogin;

        txtRedirectCode.Text = AppSettings.RedirectCode;
        txtRedirectName.Text = AppSettings.RedirectUserName;
        txtRedirectPass.Text = AppSettings.RedirectUserPass;

        if (rememberAccount)
        {
            txtUserName.Text = AppSettings.LoginUserName;
            txtUserPass.Text = AppSettings.LoginUserPass;
        }
        else
        {
            txtUserName.Text = AppSettings.LoginUserName;
            txtUserPass.Text = "";
        }        
    }
    private void SaveConfiguration()
    {
        //Chỉ lưu khi đã login thành công: VÀ CHỈ LƯU GIAO DIỆN VÀ CẤU HÌNH ĐĂNG NHẬP TỰ ĐỘNG
        if (mConnService.MauiLogined)
        {
            AppSettings.LoginRememberAccount = chkRememberAccount.IsChecked;
            AppSettings.LoginRememberAutoLogin = chkRememberAutoLogin.IsChecked;
            AppSettings.RedirectCode = mConnService.RedirectCode;
            AppSettings.RedirectUserName = mConnService.RedirectUserName;
            AppSettings.RedirectUserPass = mConnService.RedirectUserPass;
            AppSettings.UrlTrueService = mConnService.UrlTrueService;
            if (chkRememberAccount.IsChecked)
            {
                AppSettings.LoginUserName = mConnService.UserName;
                AppSettings.LoginUserPass = mConnService.UserPass;
            }
            else
            {
                AppSettings.LoginUserName = mConnService.UserName;
                AppSettings.LoginUserPass = "";
            }
            //AppSettings.AppType = Không cần lưu, vì Login tự gán trong mConnService
            AppSettings.SecurityCode = mConnService.SecurityCode;
        }        
    }
    private async Task<bool> LoginProcessAsync()
    {
        if (mBusy)
            return false;
        mBusy = true;
        string _LastMessage = "";
        bool ok = false;
        //RedirectAvailable
        (ok,_LastMessage) = await mConnService.RedirectAvailableGetAsync(30);
        if (!ok)
        {
            lblMessage.Text = "Không kết nối được máy chủ Redirect.\n" + _LastMessage;
            mBusy = false;
            return false;
        }
        else
        {
            lblMessage.Text = "Đang xác định máy chủ dịch vụ...";
        }

        // MauiRedirectAsync
        mConnService.RedirectCode = txtRedirectCode.Text;
        mConnService.RedirectUserName = txtRedirectName.Text;
        mConnService.RedirectUserPass = txtRedirectPass.Text;
        mConnService.SoapTimeOut = 30;
        bool _Redirect = false;
        _LastMessage = "";
        (_Redirect,_LastMessage) = await mConnService.MauiRedirectAsync();
        if (_Redirect == true && mConnService.UrlTrueService != null)
        {
            lblMessage.Text = "Đã xác định máy chủ";
            lblTrueService.Text = mConnService.UrlTrueService;
        }
        else
        {
            lblMessage.Text = "Máy chủ redirect không phản hồi \n" + _LastMessage;
            lblTrueService.Text = "";
            mBusy =false;
            return false;
        }
        
        // MauiLogin
        mConnService.UserName = txtUserName.Text;
        mConnService.UserPass = txtUserPass.Text;
        bool _Login = false;
        _LastMessage = "";
        (_Login,_LastMessage) = await mConnService.MauiLoginAsync();
        if (!_Login)
        {
            lblMessage.Text = "Lỗi đăng nhập \n" + _LastMessage;
            mBusy = false;
            return false;
        }
        else
        {
            lblTrueService.Text = "Kết nối thành công !";
            mBusy = false;
            return true;
        }
    }

    // NÚT ĐĂNG NHẬP
    private async void OnLoginClicked(object sender,EventArgs e)
    {
        if (mBusy == true) { return; }

        bool _login= await LoginProcessAsync();
        if (true)
        {
            //Lưu cấu hình
            SaveConfiguration();

            //Đăng nhập thành công thì gửi yêu cầu xin quyền luôn            
            string _RequestPermisionStr = AppSettings.RequestPermisionList;
            if (!string.IsNullOrEmpty(_RequestPermisionStr))
            {
                //Gửi tín hiệu đăng ký quyền, để Android tự thực hiện
                EventMessenger.Send(this, EventEnum.RequestPermissions, _RequestPermisionStr);
                EventMessenger.Send(this, EventEnum.StartForeGroundService, "AgentService");
            }

            //Đóng cửa sổ
            await Navigation.PopAsync();
            return;
        }
    }
    
    private async void OnSkipClicked(object sender,EventArgs e)
    {
        //Cần gọi đăng nhập từ đầu, nhưng với tài khoản Guest
        if (mBusy == true) { return; }
        mBusy = true;

        if (string.IsNullOrEmpty(txtRedirectCode.Text))
        {
            //Nếu txtRedirectCode chưa nhập, thì thoát
            lblMessage.Text = "Bạn cần nhập mã máy chủ, trước khi sử dụng tài khoản guest";
            mBusy = false;            
            return;
        }

        lblMessage.Text = "Đang đăng nhập khách guest";

        //RedirectAvailable
        bool _Ok = false;
        string _LastMessage = "";
        (_Ok,_LastMessage) = await mConnService.RedirectAvailableGetAsync(30);
        if (!_Ok)
        {
            lblMessage.Text = "Không kết nối được máy chủ Redirect.\n" + _LastMessage;
            mBusy = false;
            return;
        }
        else
        {
            lblMessage.Text = "Đang xác định máy chủ dịch vụ...";
        }
        
        // MauiRedirectAsync
        mConnService.RedirectCode = txtRedirectCode.Text;
        mConnService.RedirectUserName = txtRedirectName.Text;
        mConnService.RedirectUserPass = txtRedirectPass.Text;
        mConnService.SoapTimeOut = 30;
        bool _Redirect = false;
        _LastMessage = "";
        (_Redirect,_LastMessage) = await mConnService.MauiRedirectAsync();
        if (_Redirect == true && mConnService.UrlTrueService != null)
        {
            lblMessage.Text = "Đã xác định máy chủ";
            lblTrueService.Text= mConnService.UrlTrueService;
        }
        else
        {
            lblMessage.Text = "Máy chủ redirect không phản hồi \n" + _LastMessage;
            mBusy = false;
            return;
        }
        // MauiLogin
        txtUserName.Text = "guest";
        txtUserPass.Text = "12345";
        mConnService.UserName = txtUserName.Text;
        mConnService.UserPass = txtUserPass.Text;
        bool _Login = false;
        _LastMessage = "";
        (_Login,_LastMessage) = await mConnService.MauiLoginAsync();
        if (!_Login)
        {
            lblMessage.Text = "MauiLogin lỗi.\n" + _LastMessage;
            mBusy = false;
            return;
        }
        else
        {
            lblTrueService.Text = "Kết nối thành công !";
            //Lưu cấu hình
            SaveConfiguration();

            //Sự kiện đăng nhập thành công: Không cần, vì không cần kích hoạt quyền
            //EventMessenger.Send(this, EventEnum.Logined, true);

            mBusy = false;

            //Đóng cửa sổ
            await Navigation.PopAsync();

            return;
        }
    }
    private async void OnExitClicked(object sender,EventArgs e)
    {
        if (mBusy == true) { return; }

        await Navigation.PopAsync();
        return;
    }
    private void OnRememberAccountLabelTapped(object sender, EventArgs e)
    {
        chkRememberAccount.IsChecked = !chkRememberAccount.IsChecked;
    }

    private void OnRememberAutoLoginLabelTapped(object sender, EventArgs e)
    {
        chkRememberAutoLogin.IsChecked = !chkRememberAutoLogin.IsChecked;
    }
}