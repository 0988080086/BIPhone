using BIPhone.Dynamic;
namespace BIPhone.Dynamic;
// Các loại ô nhập liệu
public enum DynamicControlType
{
    Text = 1,              // Họ tên (Entry)
    RadioGroup = 4,        // Giới tính (Nam/Nữ)
    EditableCombobox = 5,  // Chức vụ (Gõ tự do hoặc chọn)
    ComboListKey = 6,      // Xã/Phường (Chọn danh sách -> Lưu Key Decimal)
    MultiColumnLookup = 7, // Mã số thuế/Cơ quan thuế (Grid tìm kiếm nhiều cột)
    MultilineText = 8,     // Ý kiến cá nhân (Tự co giãn chiều cao)
    DetailGrid = 9         // Quá trình công tác (Bảng danh sách con)
}
public class OptionItem
{
    public object Key { get; set; } = null!;
    public string Display { get; set; } = string.Empty;
}
public class FieldMetadata
{
    public string FieldCode { get; set; } = string.Empty;
    public string FieldName { get; set; } = string.Empty;
    public DynamicControlType ControlType { get; set; } = DynamicControlType.Text;
    public bool IsRequired { get; set; } = false;
    public double WidthPercent { get; set; } = 100;

    // Nguồn dữ liệu & Binding
    public string? DataSourceSql { get; set; }
    public string ValueMember { get; set; } = "Key";    // Lưu Key Decimal
    public string DisplayMember { get; set; } = "Text"; // Hiện Text
    public string? ParentFieldCode { get; set; }        // Lọc theo Cha (Tỉnh -> Huyện -> Xã)

    public List<OptionItem>? StaticOptions { get; set; } // Dùng cho Nam/Nữ
    public Dictionary<string, string>? LookupColumns { get; set; } // Cột tìm kiếm MST
    public List<FieldMetadata>? SubGridColumns { get; set; } // Cột của Quá trình công tác
}

public class DynamicFeature
{
    public string FeatureId { get; set; } = string.Empty;
    public string Title { get; set; } = string.Empty;
    public List<FieldMetadata> Fields { get; set; } = new();
}

public class TaxAgencyItem
{
    public decimal MaCqtDecimal { get; set; }        // Key Decimal dùng để lưu SQL
    public string MaSoThue { get; set; } = string.Empty;       // Mã số thuế
    public string TenCoQuanThue { get; set; } = string.Empty;  // Tên hiển thị
    public string DiaChi { get; set; } = string.Empty;         // Địa chỉ
    public string CanBoPhuTrach { get; set; } = string.Empty;  // Cán bộ quản lý
}