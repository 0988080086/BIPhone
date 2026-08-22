using BIPhone;
using Newtonsoft.Json.Linq;
using System.Data;
using System.Net;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;

public class ClsConnService
{
    private const string mRedirectUrl = "http://www.vnbis.com.vn/biservice/bimaui.asmx";
    private const string mRedirectUrl2 = "http://nguyencongdan.name.vn/biservice/bimaui.asmx";
    private string mRedirectCode;
    private string mRedirectUserName;
    private string mRedirectUserPass;
    private int mSoapTimeOut;

    private string mRedirectUrlAvailable;
    private string mRedirectUrlAvailable2;
    private string mUrlTrueService;
    private string mSoapUserAgent = "BI.Application";
    private string mLastMessage;

    private string mUserName;
    private string mUserPass;
    private string mSecurityCode;

    private static readonly ClsConnService _instance = new ClsConnService();

    public static ClsConnService Instance => _instance;

    private ClsConnService()
    {
        mRedirectCode = "";
        mRedirectUserName = "";
        mRedirectUserPass = "";
        mSoapTimeOut = 30;
        mRedirectUrlAvailable = "";
        mRedirectUrlAvailable2 = "";
        mUrlTrueService = "";
        mLastMessage = "";

        mUserName = "";
        mUserPass = "";
        mSecurityCode = "";
    }

    public ClsConnService Clone()
    {
        ClsConnService mItem = new ClsConnService();

        mItem.mRedirectUrlAvailable = mRedirectUrlAvailable;
        mItem.mRedirectUrlAvailable2 = mRedirectUrlAvailable2;
        mItem.mRedirectCode = mRedirectCode;
        mItem.mRedirectUserName = mRedirectUserName;
        mItem.mRedirectUserPass = mRedirectUserPass;
        mItem.mUrlTrueService = mUrlTrueService;
        mItem.mSoapTimeOut = mSoapTimeOut;
        mItem.mSoapUserAgent = mSoapUserAgent;
        mItem.mUserName = mUserName;
        mItem.mUserPass = mUserPass;
        mItem.mSecurityCode = mSecurityCode;
        mItem.mLastMessage = mLastMessage;

        return mItem;
    }

    #region Properties

    public bool MauiLogined
    {
        get
        {
            // Nếu Platform chưa khởi tạo mDeviceID
            if (string.IsNullOrEmpty(AppSettings.DeviceID) || string.IsNullOrEmpty(AppSettings.Platform))
                return false;
            //Nếu chưa có tham số Redirect
            if (string.IsNullOrEmpty(mRedirectCode) || string.IsNullOrEmpty(mRedirectUserName) || string.IsNullOrEmpty(mRedirectUserPass))
                return false;
            //Nếu chưa hỏi redirect
            if (string.IsNullOrEmpty(mRedirectUrlAvailable) && string.IsNullOrEmpty(mRedirectUrlAvailable2))
                return false;
            //Nếu chưa trả lại mUrlTrueService
            if (string.IsNullOrEmpty(mUrlTrueService))
                return false;
            //Nếu chưa có tham số Login
            if (string.IsNullOrEmpty(mUserName) || string.IsNullOrEmpty(mUserPass) || string.IsNullOrEmpty(mSecurityCode))
                return false;
            return true;
        }
    }

    public string RedirectCode
    {
        get { return mRedirectCode; }
        set { mRedirectCode = value; }
    }

    public string RedirectUserName
    {
        get { return mRedirectUserName; }
        set { mRedirectUserName = value; }
    }

    public string RedirectUserPass
    {
        get { return mRedirectUserPass; }
        set { mRedirectUserPass = value; }
    }

    public decimal SoapTimeOut
    {
        get { return mSoapTimeOut; }
        set { mSoapTimeOut = (int)value; }
    }

    public string UserName
    {
        get { return mUserName; }
        set { mUserName = value; }
    }

    public string UserPass
    {
        get { return mUserPass; }
        set { mUserPass = value; }
    }

    public string SecurityCode { get { return mSecurityCode; } }
    public string RedirectUrlAvailable { get { return mRedirectUrlAvailable; } }
    public string RedirectUrlAvailable2 { get { return mRedirectUrlAvailable2; } }
    public string LastMessage { get { return mLastMessage; } }
    public string UrlTrueService { get { return mUrlTrueService; } }

    #endregion

    /// <summary>Bước 1: Tìm địa chỉ nào hỗ trợ Redirect.Chạy tuần tự và đợi từng UrlAvailableCheckAsync hoàn thành.</summary>
    public async Task<bool> RedirectAvailableGetAsync(int timeout = 10)
    {
        long count = 0;
        mLastMessage = "";

        if (!string.IsNullOrEmpty(mRedirectUrl))
        {
            if (await UrlAvailableCheckAsync(mRedirectUrl, timeout))
            {
                mRedirectUrlAvailable = mRedirectUrl;
                count++;
            }
        }

        if (!string.IsNullOrEmpty(mRedirectUrl2))
        {
            if (await UrlAvailableCheckAsync(mRedirectUrl2, timeout))
            {
                mRedirectUrlAvailable2 = mRedirectUrl2;
                count++;
            }
        }

        return count > 0;
    }

    /// <summary>Bước 2: Gọi MauiRedirect tìm mUrlTrueService (POST được await hoàn thành trước khi xử lý response)</summary>
    public async Task<bool> MauiRedirectAsync()
    {
        mLastMessage = "";
        if (string.IsNullOrEmpty(mRedirectUrlAvailable) && string.IsNullOrEmpty(mRedirectUrlAvailable2))
        {
            mLastMessage = "mRedirectUrlAvailable và mRedirectUrlAvailable2 chưa có";
            mUrlTrueService = "";
            return false;
        }
        DataTable tblParameter = new DataTable("Parameter");
        tblParameter.Columns.Add("RedirectCode", typeof(string));
        tblParameter.Columns.Add("RedirectUserName", typeof(string));
        tblParameter.Columns.Add("RedirectUserPass", typeof(string));
        tblParameter.Rows.Add(mRedirectCode, mRedirectUserName, mRedirectUserPass);
        DataSet ds = new DataSet();
        ds.Tables.Add(tblParameter);
        string dataPost = ClienJsonPOST("", "MauiRedirect", ds);
        string targetUrl = !string.IsNullOrEmpty(mRedirectUrlAvailable) ? mRedirectUrlAvailable : mRedirectUrlAvailable2;
        string responseStr = await SubmitMauiAsync(targetUrl, dataPost);
        if (string.IsNullOrEmpty(responseStr))
        {
            return false;
        }
        string errorNum;
        string parseErrorDesc;
        DataSet dsSet;
        string error;
        if (!ClienJsonParse(responseStr, out errorNum, out parseErrorDesc, out dsSet, out error))
        {
            mLastMessage = "MauiRedirect.ClienJsonParse: " + error;
            return false;
        }
        long errCode;
        if (long.TryParse(errorNum, out errCode) && errCode == 200 && dsSet != null && dsSet.Tables.Contains("Parameter") && dsSet.Tables["Parameter"].Rows.Count > 0)
        {
            DataTable tbl = dsSet.Tables["Parameter"];
            string serverUrl = tbl.Rows[0]["ServerUrl"].ToString();
            serverUrl = serverUrl.Replace("biservice.asmx", "bimaui.asmx").Replace("biserviceV2.asmx", "bimaui.asmx");
            mUrlTrueService = serverUrl;
            mLastMessage = "";
            return true;
        }
        mLastMessage = "MauiRedirect: ErrorNumber=" + errorNum + ", ErrorDescript=" + parseErrorDesc;
        mUrlTrueService = "";
        return false;
    }
    /// <summary>Bước 3: MauiLogin.POST được await hoàn thành trước khi parse response.</summary>
    public async Task<bool> MauiLoginAsync()
    {
        mLastMessage = "";

        if (string.IsNullOrEmpty(mUrlTrueService))
        {
            mLastMessage = "Không tồn tại đường dẫn máy chủ đích";
            return false;
        }

        if (!await UrlAvailableCheckAsync(mUrlTrueService, mSoapTimeOut))
        {
            mLastMessage = $"Máy chủ dữ liệu [{mUrlTrueService}] không phản hồi";
            return false;
        }

        if (string.IsNullOrEmpty(mUserName) || string.IsNullOrEmpty(mUserPass))
        {
            mLastMessage = "Chưa khai báo tài khoản người sử dụng";
            return false;
        }

        DataTable tblParameter = new DataTable("Parameter");
        tblParameter.Columns.Add("UserName", typeof(string));
        tblParameter.Columns.Add("UserPass", typeof(string));
        tblParameter.Columns.Add("DeviceID", typeof(string));
        tblParameter.Columns.Add("Platform", typeof(string));
        tblParameter.Columns.Add("PhoneToken", typeof(string));
        //tblParameter.Columns.Add("AppType", typeof(string));

        //tblParameter.Rows.Add(mUserName, mUserPass, AppSettings.DeviceID, AppSettings.Platform, AppSettings.PhoneToken, AppSettings.AppType.ToString());
        tblParameter.Rows.Add(mUserName, mUserPass, AppSettings.DeviceID, AppSettings.Platform, AppSettings.PhoneToken);
        DataSet ds = new DataSet();
        ds.Tables.Add(tblParameter);
        string dataPost = ClienJsonPOST("", "MauiLogin", ds);
        if (string.IsNullOrEmpty(dataPost))
        {
            mLastMessage = "MauiLogin.ClienJsonPOST: " + mLastMessage;
            return false;
        }

        string responseStr = await SubmitMauiAsync(mUrlTrueService, dataPost);
        if (string.IsNullOrEmpty(responseStr))
        {
            if (string.IsNullOrEmpty(mLastMessage))
            {
                mLastMessage = "MauiLogin.SubmitMaui: Server không trả dữ liệu";
            }
            return false;
        }

        string errorNum;
        string errorDesc;
        DataSet dsSet;
        string error;

        if (!ClienJsonParse(responseStr, out errorNum, out errorDesc, out dsSet, out error))
        {
            mLastMessage = "MauiLogin.ClienJsonParse: " + error;
            return false;
        }

        long errCode;

        if (!string.IsNullOrEmpty(errorNum) && long.TryParse(errorNum, out errCode) && errCode == 200 && dsSet != null && dsSet.Tables.Contains("Parameter") && dsSet.Tables["Parameter"].Rows.Count == 1)
        {
            DataTable tbl = dsSet.Tables["Parameter"];
            mSecurityCode = tbl.Rows[0]["SecurityCode"].ToString();
            if (tbl.Columns.Contains("AppType") == true && tbl.Rows[0]["AppType"] != DBNull.Value && tbl.Rows[0]["AppType"] != null)
            {
                // Chuyển sang long rồi ép kiểu (cast) trực tiếp về PhoneAppTypeEnum
                long appTypeValue = Convert.ToInt64(tbl.Rows[0]["AppType"]);
                AppSettings.AppType = (long)appTypeValue;
            }
            else
            {
                // Mặc định nếu dữ liệu bị null                
                AppSettings.AppType = (long)PhoneAppTypeEnum.KhachHang;
            }
            string _Str; long _Long;
            if (tbl.Columns.Contains("DeviceRecord") == true && tbl.Rows[0]["DeviceRecord"] != DBNull.Value && tbl.Rows[0]["DeviceRecord"] != null)
            {
                _Str = Convert.ToString(tbl.Rows[0]["DeviceRecord"]);
                AppSettings.DeviceRecord = _Str;
            }
            if (tbl.Columns.Contains("ChoPhep_NhanDienDt_Den") == true && tbl.Rows[0]["ChoPhep_NhanDienDt_Den"] != DBNull.Value && tbl.Rows[0]["ChoPhep_NhanDienDt_Den"] != null)
            {
                _Long = Convert.ToInt64(tbl.Rows[0]["ChoPhep_NhanDienDt_Den"]);
                AppSettings.ChoPhep_NhanDienDt_Den = _Long;
            }
            if (tbl.Columns.Contains("ChoPhep_NhanDienDt_HienPopup") == true && tbl.Rows[0]["ChoPhep_NhanDienDt_HienPopup"] != DBNull.Value && tbl.Rows[0]["ChoPhep_NhanDienDt_HienPopup"] != null)
            {
                _Long = Convert.ToInt64(tbl.Rows[0]["ChoPhep_NhanDienDt_HienPopup"]);
                AppSettings.ChoPhep_NhanDienDt_HienPopup = _Long;
            }
            if (tbl.Columns.Contains("ChoPhep_NhanDienDt_Di") == true && tbl.Rows[0]["ChoPhep_NhanDienDt_Di"] != DBNull.Value && tbl.Rows[0]["ChoPhep_NhanDienDt_Di"] != null)
            {
                _Long = Convert.ToInt64(tbl.Rows[0]["ChoPhep_NhanDienDt_Di"]);
                AppSettings.ChoPhep_NhanDienDt_Di = _Long;
            }
            if (tbl.Columns.Contains("ChoPhep_GhiAmDt") == true && tbl.Rows[0]["ChoPhep_GhiAmDt"] != DBNull.Value && tbl.Rows[0]["ChoPhep_GhiAmDt"] != null)
            {
                _Long = Convert.ToInt64(tbl.Rows[0]["ChoPhep_GhiAmDt"]);
                AppSettings.ChoPhep_GhiAmDt = _Long;
            }
            if (tbl.Columns.Contains("ChoPhep_GuiTinSms") == true && tbl.Rows[0]["ChoPhep_GuiTinSms"] != DBNull.Value && tbl.Rows[0]["ChoPhep_GuiTinSms"] != null)
            {
                _Long = Convert.ToInt64(tbl.Rows[0]["ChoPhep_GuiTinSms"]);
                AppSettings.ChoPhep_GuiTinSms = _Long;
            }
            if (tbl.Columns.Contains("ChoPhep_LuuTruGps") == true && tbl.Rows[0]["ChoPhep_LuuTruGps"] != DBNull.Value && tbl.Rows[0]["ChoPhep_LuuTruGps"] != null)
            {
                _Long = Convert.ToInt64(tbl.Rows[0]["ChoPhep_LuuTruGps"]);
                AppSettings.ChoPhep_LuuTruGps = _Long;
            }
            if (tbl.Columns.Contains("ChoPhep_TimKiemKh") == true && tbl.Rows[0]["ChoPhep_TimKiemKh"] != DBNull.Value && tbl.Rows[0]["ChoPhep_TimKiemKh"] != null)
            {
                _Long = Convert.ToInt64(tbl.Rows[0]["ChoPhep_TimKiemKh"]);
                AppSettings.ChoPhep_TimKiemKh = _Long;
            }
            if (tbl.Columns.Contains("ChoPhep_ThemMoiKh") == true && tbl.Rows[0]["ChoPhep_ThemMoiKh"] != DBNull.Value && tbl.Rows[0]["ChoPhep_ThemMoiKh"] != null)
            {
                _Long = Convert.ToInt64(tbl.Rows[0]["ChoPhep_ThemMoiKh"]);
                AppSettings.ChoPhep_ThemMoiKh = _Long;
            }
            if (tbl.Columns.Contains("ChoPhep_SuaKh") == true && tbl.Rows[0]["ChoPhep_SuaKh"] != DBNull.Value && tbl.Rows[0]["ChoPhep_SuaKh"] != null)
            {
                _Long = Convert.ToInt64(tbl.Rows[0]["ChoPhep_SuaKh"]);
                AppSettings.ChoPhep_SuaKh = _Long;
            }
            if (tbl.Columns.Contains("ChoPhep_TaoChungTu") == true && tbl.Rows[0]["ChoPhep_TaoChungTu"] != DBNull.Value && tbl.Rows[0]["ChoPhep_TaoChungTu"] != null)
            {
                _Long = Convert.ToInt64(tbl.Rows[0]["ChoPhep_TaoChungTu"]);
                AppSettings.ChoPhep_TaoChungTu = _Long;
            }
            if (tbl.Columns.Contains("ChoPhep_SuaChungTu") == true && tbl.Rows[0]["ChoPhep_SuaChungTu"] != DBNull.Value && tbl.Rows[0]["ChoPhep_SuaChungTu"] != null)
            {
                _Long = Convert.ToInt64(tbl.Rows[0]["ChoPhep_SuaChungTu"]);
                AppSettings.ChoPhep_SuaChungTu = _Long;
            }
            if (tbl.Columns.Contains("ChoPhep_XemGpsNhanVien") == true && tbl.Rows[0]["ChoPhep_XemGpsNhanVien"] != DBNull.Value && tbl.Rows[0]["ChoPhep_XemGpsNhanVien"] != null)
            {
                _Long = Convert.ToInt64(tbl.Rows[0]["ChoPhep_XemGpsNhanVien"]);
                AppSettings.ChoPhep_XemGpsNhanVien = _Long;
            }
            if (tbl.Columns.Contains("ServerID") == true && tbl.Rows[0]["ServerID"] != DBNull.Value && tbl.Rows[0]["ServerID"] != null)
            {
                _Long = Convert.ToInt64(tbl.Rows[0]["ServerID"]);
                AppSettings.ServerID = _Long;
            }
            if (tbl.Columns.Contains("ServerCpuID") == true && tbl.Rows[0]["ServerCpuID"] != DBNull.Value && tbl.Rows[0]["ServerCpuID"] != null)
            {
                _Str = Convert.ToString(tbl.Rows[0]["ServerCpuID"]);
                AppSettings.ServerCpuID = _Str;
            }
            if (tbl.Columns.Contains("PathData") == true && tbl.Rows[0]["PathData"] != DBNull.Value && tbl.Rows[0]["PathData"] != null)
            {
                _Str = Convert.ToString(tbl.Rows[0]["PathData"]);
                AppSettings.PathData = _Str;
            }
            if (tbl.Columns.Contains("PathRoot") == true && tbl.Rows[0]["PathRoot"] != DBNull.Value && tbl.Rows[0]["PathRoot"] != null)
            {
                _Str = Convert.ToString(tbl.Rows[0]["PathRoot"]);
                AppSettings.PathRoot = _Str;
            }
            if (tbl.Columns.Contains("PathHost") == true && tbl.Rows[0]["PathHost"] != DBNull.Value && tbl.Rows[0]["PathHost"] != null)
            {
                _Str = Convert.ToString(tbl.Rows[0]["PathHost"]);
                AppSettings.PathHost = _Str;
            }
            mLastMessage = "";
            return true;
        }
        else
        {
            mLastMessage = "MauiLogin.ClienJsonParse: ErrorNumber=" + errorNum + ", ErrorDescript=" + errorDesc;
            return false;
        }
    }

    /// <summary>Đồng bộ khách hàng</summary>
    public async Task<DataTable> MauiCustomerAsync()
    {
        mLastMessage = "";

        //Chuẩn bị dữ liệu POST CustomerAsync        
        DataTable mTblParameter = new DataTable("Parameter");
        mTblParameter.Columns.Add("SecurityCode", typeof(System.String));
        mTblParameter.Columns.Add("DtID", typeof(System.String));
        mTblParameter.Columns.Add("NgayCn", typeof(System.String));
        mTblParameter.Rows.Add(mSecurityCode, AppSettings.DoiTuongSync_DtID, AppSettings.DoiTuongSync_NgayCn);
        DataSet _DsSet = new DataSet();
        _DsSet.Tables.Add(mTblParameter);

        //Chuyển đổi thành JSON khi POST lên
        string mDataPost = ClienJsonPOST(SecurityCode, "MauiCustomer", _DsSet);
        if (string.IsNullOrEmpty(mDataPost))
        {
            mLastMessage = "MauiCustomerAsync: ClienJsonPOST trả về rỗng.";
            LogWriter.WriteLine(mLastMessage);
            return null;
        }

        //Gọi POST
        string mStr = await SubmitMauiAsync(mUrlTrueService, mDataPost);
        if (string.IsNullOrEmpty(mStr))
        {
            mLastMessage = "MauiCustomerAsync: SubmitMauiAsync trả về rỗng. " + mLastMessage;
            LogWriter.WriteLine(mLastMessage);
            return null;
        }

        //Phân tích JSON trả về
        string mErrorNum = "";
        string mErrorDesc = "";
        DataSet mDsReturn = null;
        string mError = "";
        bool mParseOK = ClienJsonParse(mStr, out mErrorNum, out mErrorDesc, out mDsReturn, out mError);
        if (mParseOK == false)
        {
            mLastMessage = "MauiCustomerAsync: ClienJsonParse lỗi. " + mError;
            LogWriter.WriteLine(mLastMessage);
            return null;
        }

        //Kiểm tra lỗi trả về
        int xErrorCode = -1;
        int.TryParse(mErrorNum, out xErrorCode);
        if (xErrorCode != 200) return null;
        if (mDsReturn == null) return null;
        if (mDsReturn.Tables.Contains("DoiTuong") == false) return null;

        //Đọc dữ liệu trả về
        DataTable mTbl = mDsReturn.Tables["DoiTuong"];
        if (mTbl == null || mTbl.Rows.Count == 0) return null;
        DataRow[] mRows = mTbl.Select("", "NgayCn ASC, DtID ASC");
        if (mRows.Length > 0)
        {
            //Đánh dấu để lấy page tiếp theo, dựa vào (mDtID,mNgayCn) trang hiện tại
            AppSettings.DoiTuongSync_DtID = Convert.ToInt64(mRows[mRows.Length - 1]["DtID"]);
            AppSettings.DoiTuongSync_NgayCn = Convert.ToDouble(mRows[mRows.Length - 1]["NgayCn"]);
            return mTbl.Copy();
        }
        else
        {
            //Không còn dữ liệu
            return null;
        }
    }

    public bool ClienJsonParse(string jsonText, out string errorNumber, out string errorDescript, out DataSet dsSet, out string error)
    {
        errorNumber = "";
        errorDescript = "";
        dsSet = null;
        error = "";

        if (string.IsNullOrEmpty(jsonText))
        {
            error = "jsonText rỗng";
            return false;
        }

        DataSet vDsSet = new DataSet();

        try
        {
            JObject root = JObject.Parse(jsonText);

            if (root["ErrorNumber"] != null)
                errorNumber = root["ErrorNumber"].ToString();
            else if (root["errornumber"] != null)
                errorNumber = root["errornumber"].ToString();

            if (root["ErrorDescript"] != null)
                errorDescript = root["ErrorDescript"].ToString();
            else if (root["errordescript"] != null)
                errorDescript = root["errordescript"].ToString();

            JObject dataObj = null;

            if (root["data"] != null &&
                root["data"].Type == JTokenType.Object)
            {
                dataObj = (JObject)root["data"];
            }
            else if (root["Data"] != null &&
                     root["Data"].Type == JTokenType.Object)
            {
                dataObj = (JObject)root["Data"];
            }

            if (dataObj != null)
            {
                foreach (var tableProp in dataObj)
                {
                    string tableName = tableProp.Key;
                    DataTable dt = new DataTable(tableName);

                    if (tableProp.Value != null && tableProp.Value.Type == JTokenType.Array)
                    {
                        JArray rowsArray = (JArray)tableProp.Value;

                        bool colsCreated = false;

                        foreach (JToken rowToken in rowsArray)
                        {
                            if (rowToken.Type != JTokenType.Object)
                                continue;
                            JObject rowObj = (JObject)rowToken;

                            if (!colsCreated)
                            {
                                foreach (var colProp in rowObj)
                                {
                                    dt.Columns.Add(colProp.Key, typeof(string));
                                }
                                colsCreated = true;
                            }

                            DataRow dr = dt.NewRow();

                            foreach (var colProp in rowObj)
                            {
                                dr[colProp.Key] = colProp.Value != null ? colProp.Value.ToString() : string.Empty;
                            }
                            dt.Rows.Add(dr);
                        }
                    }

                    vDsSet.Tables.Add(dt);
                }
            }

            dsSet = vDsSet;
            return true;
        }
        catch (Exception ex)
        {
            dsSet = null;
            errorNumber = "";
            errorDescript = "";
            mLastMessage = ex.ToString();
            error = ex.ToString();
            return false;
        }
    }

    public string ClienJsonPOST(string securityCode, string functionName)
    {
        return ClienJsonPOST(securityCode, functionName, null);
    }

    public string ClienJsonPOST(string securityCode, string functionName, DataSet dsSet)
    {
        try
        {
            JObject root = new JObject();
            root.Add("SecurityCode", securityCode);
            root.Add("FunctionName", functionName);
            if (dsSet != null && dsSet.Tables.Count > 0)
            {
                JObject dataObj = new JObject();
                int tableIndex = 1;

                foreach (DataTable table in dsSet.Tables)
                {
                    JArray rowsArray = new JArray();

                    foreach (DataRow row in table.Rows)
                    {
                        JObject rowObj = new JObject();
                        foreach (DataColumn col in table.Columns)
                        {
                            object val = row[col];
                            string strVal = string.Empty;
                            if (val != DBNull.Value &&
                                val != null)
                            {
                                DateTime dateTimeVal;

                                if (val is DateTime)
                                {
                                    dateTimeVal = (DateTime)val;

                                    strVal = dateTimeVal.ToString("yyyy-MM-dd HH:mm:ss");
                                }
                                else
                                {
                                    strVal = val.ToString();
                                }
                            }
                            rowObj.Add(col.ColumnName, strVal);
                        }
                        rowsArray.Add(rowObj);
                    }

                    string tableName = table.TableName;
                    if (string.IsNullOrEmpty(tableName))
                    {
                        tableName = "Table" + tableIndex.ToString();
                        tableIndex++;
                    }
                    dataObj.Add(tableName, rowsArray);
                }
                root.Add("data", dataObj);
            }
            else
            {
                root.Add("data", JValue.CreateNull());
            }
            return root.ToString(Newtonsoft.Json.Formatting.None);
        }
        catch (Exception ex)
        {
            mLastMessage = ex.ToString();
            return "";
        }
    }

    /// <summary>Hàm mới, sử dụng HttpClient (Bắt buộc khai báo android:usesCleartextTraffic="true" trong AndroidManifest.xml)</summary>
    private async Task<string> SubmitMauiAsync(string mTrueUrlService, string jsonPost)
    {
        mLastMessage = "";

        if (string.IsNullOrEmpty(jsonPost))
        {
            mLastMessage = "SubmitMaui: JSON POST rỗng";
            return "";
        }
        if (string.IsNullOrEmpty(mTrueUrlService))
        {
            mLastMessage = "SubmitMaui: URL rỗng";
            return "";
        }
        try
        {
            // 1. Chuẩn hóa URL
            int nReaden = mTrueUrlService.IndexOf('?');
            if (nReaden >= 0)
            {
                mTrueUrlService = mTrueUrlService.Substring(0, nReaden);
            }
            mTrueUrlService = mTrueUrlService.TrimEnd('/');
            string mUrl = mTrueUrlService + "/SubmitMaui";
            // 2. Thiết lập Timeout
            int nTimeoutSeconds = mSoapTimeOut > 0 ? mSoapTimeOut : 10;
            // 3. HttpClientHandler
            using (HttpClientHandler handler = new HttpClientHandler())
            {
                // Nếu Server có trả gzip/deflate thì HttpClient tự giải nén.
                handler.AutomaticDecompression = System.Net.DecompressionMethods.GZip | System.Net.DecompressionMethods.Deflate;
                // Giữ hành vi gần giống HttpWebRequest vb.net
                handler.AllowAutoRedirect = true;
                // Không sử dụng Cookie của HttpClient này.
                handler.UseCookies = false;
                // 4. HttpClient
                using (HttpClient client = new HttpClient(handler))
                {
                    client.Timeout = TimeSpan.FromSeconds(nTimeoutSeconds);
                    client.DefaultRequestHeaders.UserAgent.ParseAdd("BI.Application");
                    // 5. Tạo POST Content
                    using (StringContent content = new StringContent(jsonPost, System.Text.Encoding.UTF8, "application/json"))
                    {
                        using (HttpResponseMessage response = await client.PostAsync(mUrl, content))
                        {
                            // 7. Kiểm tra HTTP Status
                            if (!response.IsSuccessStatusCode)
                            {
                                string errorBody = "";
                                try
                                {
                                    errorBody = await response.Content.ReadAsStringAsync();
                                }
                                catch
                                {
                                }
                                mLastMessage = "SubmitMaui HTTP Error: " + ((int)response.StatusCode).ToString() + " " + response.ReasonPhrase;
                                if (!string.IsNullOrEmpty(errorBody))
                                {
                                    mLastMessage += " | Response=" + errorBody;
                                }
                                return "";
                            }
                            // 10. Đọc toàn bộ Response Không tự ReadAsync từng block nữa.
                            string result = await response.Content.ReadAsStringAsync();
                            // 11. Kiểm tra kết quả
                            if (string.IsNullOrEmpty(result))
                            {
                                mLastMessage = "SubmitMaui: Server trả body rỗng";
                                return "";
                            }
                            // 12. Log kích thước Response
                            int resultUtf8Length = System.Text.Encoding.UTF8.GetByteCount(result);
                            // 14. Thành công
                            mLastMessage = "";
                            return result;
                        }
                    }
                }
            }
        }
        catch (TaskCanceledException ex)
        {
            mLastMessage = "SubmitMaui HttpClient Timeout/Canceled: " + ex.ToString();
            LogWriter.WriteLine(mLastMessage);
            return "";
        }
        catch (HttpRequestException ex)
        {
            mLastMessage = "SubmitMaui HttpClient RequestException: " + ex.ToString();
            LogWriter.WriteLine(mLastMessage);
            return "";
        }
        catch (Exception ex)
        {
            mLastMessage = "SubmitMaui HttpClient Exception: " + ex.ToString();
            LogWriter.WriteLine(mLastMessage);
            return "";
        }
    }

    /// <summary>
    /// Async kiểm tra URL khả dụng bằng HttpClient (Đã xử lý triệt để NetworkOnMainThreadException).
    /// </summary>
    private async Task<bool> UrlAvailableCheckAsync(string url, int timeoutInSecond = 10)
    {
        if (string.IsNullOrWhiteSpace(url))
            return false;
        if (timeoutInSecond <= 0) timeoutInSecond = 10;
        // Đẩy toàn bộ tác vụ mạng ra Background Thread để tránh crash trên Android UI Thread
        return await Task.Run(async () =>
        {
            try
            {
                using (var handler = new HttpClientHandler())
                {
                    // Bỏ qua kiểm tra chứng chỉ SSL
                    handler.ServerCertificateCustomValidationCallback = (sender, cert, chain, sslPolicyErrors) => true;
                    using (var client = new HttpClient(handler))
                    {
                        client.Timeout = TimeSpan.FromSeconds(timeoutInSecond);

                        string userAgent = !string.IsNullOrWhiteSpace(mSoapUserAgent) ? mSoapUserAgent : "BI.Application";
                        client.DefaultRequestHeaders.UserAgent.ParseAdd(userAgent);

                        // Chỉ đọc Header để kiểm tra Status Code
                        using (var response = await client.GetAsync(url, HttpCompletionOption.ResponseHeadersRead).ConfigureAwait(false))
                        {
                            int status = (int)response.StatusCode;
                            return status >= 100 && status < 400;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                mLastMessage = $"UrlAvailableCheck Exception: {ex.Message}";
                LogWriter.WriteLine(mLastMessage);
                return false;
            }
        }).ConfigureAwait(false);
    }

    /// <summary>Thức giấc chỉ được gọi từ CallBroadcastReceiver=>AgentService=>LoadSecureStorage (Để Post API)</summary>
    public async Task<bool> LoadByWakeup()
    {
        if (string.IsNullOrEmpty(mSecurityCode) == true)  //mSecurityCode="" thì cần đọc lại khi thức giấc
        {
            //Mục đích khi thức giấc (ứng dụng đã đóng), thì đọc lại SecurityCode => POST API
            //(Điều kiện: AppSettings.SecurityCode + AppSettings.UrlTrueService phải khác rỗng)
            mUrlTrueService = AppSettings.UrlTrueService;
            mSecurityCode = AppSettings.SecurityCode;
            if (string.IsNullOrEmpty(mSecurityCode) || string.IsNullOrEmpty(mUrlTrueService))
            {
                return false;
            }
            else
            {
                //Không cần đọc các tham số khác, vì AgentService chỉ cần 2 tham số này để POST data thôi
                return true;
            }
        }
        else
        {
            return false;
        }
    }
    public async Task<bool> LoadByAppStartup()
    {
        mRedirectCode = AppSettings.RedirectCode;
        mRedirectUserName = AppSettings.RedirectUserName;
        mRedirectUserPass = AppSettings.RedirectUserPass;
        mRedirectUrlAvailable = AppSettings.RedirectUrlAvailable;
        mRedirectUrlAvailable2 = AppSettings.RedirectUrlAvailable2;
        if (AppSettings.LoginRememberAccount == true)
        {
            mUserName = AppSettings.LoginUserName;
            mUserPass = AppSettings.LoginUserPass;
        }
        else
        {
            mUserName = AppSettings.LoginUserName;
            mUserPass = "";
        }
        return true;
    }
}