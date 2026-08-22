using System.Data;
using Android.Content;
using Android.Views;
using Android.Widget;

namespace BIPhone.Platforms.Android;

public class OverlayData : BaseAdapter
{
    private readonly Context _context;
    private readonly DataTable _table;

    public OverlayData(Context context, CrmDienThoaiItem _Item)
    {
        _context = context;

        DataRow aRow; 
        DataTable _Temp = DataTemplate();
        //Phần thông tin trên _Item
        if (_Item.Huong>0)
        {
            if (_Item.Huong == (decimal)CrmHuongEnum._In)
            { aRow = _Temp.NewRow(); aRow["TieuDe"] = "Cuộc gọi"; aRow["NoiDung"] = "Đến"; _Temp.Rows.Add(aRow); }
            else if (_Item.Huong == (decimal)CrmHuongEnum._Out)
            {aRow = _Temp.NewRow(); aRow["TieuDe"] = "Cuộc gọi"; aRow["NoiDung"] = "Đi"; _Temp.Rows.Add(aRow);}
            else { aRow = _Temp.NewRow(); aRow["TieuDe"] = "Cuộc gọi"; aRow["NoiDung"] = "NB"; _Temp.Rows.Add(aRow); }
        }
        if (!string.IsNullOrEmpty(_Item.BatDau))
        {
            aRow = _Temp.NewRow(); aRow["TieuDe"] = "Thời gian"; aRow["NoiDung"] = _Item.BatDau + " -> " + _Item.KetThuc; _Temp.Rows.Add(aRow);
        }
        if (!string.IsNullOrEmpty(_Item.Kenh))
        {
            aRow = _Temp.NewRow(); aRow["TieuDe"] = "Kênh"; aRow["NoiDung"] = _Item.Kenh; _Temp.Rows.Add(aRow);
        }
        if (!string.IsNullOrEmpty(_Item.KenhSoMay))
        {
            aRow = _Temp.NewRow(); aRow["TieuDe"] = "SIM"; aRow["NoiDung"] = _Item.KenhSoMay; _Temp.Rows.Add(aRow);
        }
        if (_Item.Source > 0)
        {
            if (_Item.Source == (decimal)TelSourceEnum.CallScreening)
            { aRow = _Temp.NewRow(); aRow["TieuDe"] = "Nguồn"; aRow["NoiDung"] = "CallScreening"; _Temp.Rows.Add(aRow); }
            else if (_Item.Source == (decimal)TelSourceEnum.BroadcastReceiver)
            { aRow = _Temp.NewRow(); aRow["TieuDe"] = "Nguồn"; aRow["NoiDung"] = "BroadcastReceiver"; _Temp.Rows.Add(aRow); }
            else
            { aRow = _Temp.NewRow(); aRow["TieuDe"] = "Nguồn"; aRow["NoiDung"] = "Unknow"; _Temp.Rows.Add(aRow); }
        }
        bool _ExistInSqlLite = false;
        DoiTuongItem _DtItem = null;
        if (_Item.DtID > 0)
        {
            _DtItem = DoiTuong.Instance.GetByDtID(_Item.DtID);            
        }
        else if (!string.IsNullOrEmpty(_Item.DienThoai) && _Item.DienThoai.Length>6)
        {
            _DtItem = DoiTuong.Instance.GetByTel(_Item.DienThoai);
        }
        if (_DtItem != null)
        {
            _ExistInSqlLite = true;
            if (!string.IsNullOrEmpty(_DtItem.Ma))
            {
                aRow = _Temp.NewRow(); aRow["TieuDe"] = "Mã Kh"; aRow["NoiDung"] = _DtItem.Ma; _Temp.Rows.Add(aRow);
            }
            if (!string.IsNullOrEmpty(_DtItem.Ten))
            {
                aRow = _Temp.NewRow(); aRow["TieuDe"] = "Tên Kh"; aRow["NoiDung"] = _DtItem.Ten; _Temp.Rows.Add(aRow);
            }
            if (!string.IsNullOrEmpty(_DtItem.DiaChi))
            {
                aRow = _Temp.NewRow(); aRow["TieuDe"] = "Địa chỉ"; aRow["NoiDung"] = _DtItem.DiaChi; _Temp.Rows.Add(aRow);
            }
            if (!string.IsNullOrEmpty(_DtItem.DienThoai))
            {
                aRow = _Temp.NewRow(); aRow["TieuDe"] = "Điện thoại"; aRow["NoiDung"] = _DtItem.DienThoai; _Temp.Rows.Add(aRow);
            }
            if (!string.IsNullOrEmpty(_DtItem.DienGiai))
            {
                aRow = _Temp.NewRow(); aRow["TieuDe"] = "Diễn giải"; aRow["NoiDung"] = _DtItem.DienGiai; _Temp.Rows.Add(aRow);
            }
            if (!string.IsNullOrEmpty(_DtItem.DienGiaiPopup))
            {
                aRow = _Temp.NewRow(); aRow["TieuDe"] = "Tổng hợp"; aRow["NoiDung"] = _DtItem.DienGiaiPopup; _Temp.Rows.Add(aRow);
            }            
        }

        if (_ExistInSqlLite == false)
        {
            if (!string.IsNullOrEmpty(_Item.DtMa))
            { aRow = _Temp.NewRow(); aRow["TieuDe"] = "Mã Kh"; aRow["NoiDung"] = _Item.DtMa; _Temp.Rows.Add(aRow); }
            if (!string.IsNullOrEmpty(_Item.DtTen))
            { aRow = _Temp.NewRow(); aRow["TieuDe"] = "Tên Kh"; aRow["NoiDung"] = _Item.DtTen; _Temp.Rows.Add(aRow); }
            if (!string.IsNullOrEmpty(_Item.DtDiaChi))
            { aRow = _Temp.NewRow(); aRow["TieuDe"] = "Địa chỉ"; aRow["NoiDung"] = _Item.DtDiaChi; _Temp.Rows.Add(aRow); }
            if (!string.IsNullOrEmpty(_Item.DtDienThoai))
            { aRow = _Temp.NewRow(); aRow["TieuDe"] = "Điện thoại"; aRow["NoiDung"] = _Item.DtDienThoai; _Temp.Rows.Add(aRow); }
            if (!string.IsNullOrEmpty(_Item.NoiDung))
            { aRow = _Temp.NewRow(); aRow["TieuDe"] = "Nội dung"; aRow["NoiDung"] = _Item.NoiDung; _Temp.Rows.Add(aRow); }
        }
        _table = _Temp;
    }
    private static DataTable DataTemplate()
    {
        DataTable dt = new DataTable();
        dt.Columns.Add("TieuDe", typeof(string));
        dt.Columns.Add("NoiDung", typeof(string));
        return dt;
    }
    public override int Count => _table?.Rows.Count ?? 0;

    public override Java.Lang.Object GetItem(int position) => null;

    public override long GetItemId(int position) => position;

    public override global::Android.Views.View GetView(int position, global::Android.Views.View convertView, ViewGroup parent)
    {
        var view = convertView ?? LayoutInflater.From(_context).Inflate(Resource.Layout.phone_call_itemrows, parent, false);

        var lblTitle = view.FindViewById<TextView>(Resource.Id.lblTitle);
        var lblContent = view.FindViewById<TextView>(Resource.Id.lblContent);

        if (_table != null && position < _table.Rows.Count)
        {
            DataRow row = _table.Rows[position];

            // Đọc tên cột "TieuDe" và "NoiDung" từ DataTable
            string title = row["TieuDe"]?.ToString() ?? "";
            string content = row["NoiDung"]?.ToString() ?? "";

            lblTitle.Text = title.EndsWith(":") ? title : title + ":";
            lblContent.Text = content;
        }

        return view;
    }
    public System.Data.DataTable ViewData
    {
        get { return _table; }
    }
}
