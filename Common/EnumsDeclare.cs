namespace BIPhone;

public enum ConnectionStateEnum
{
    /// <summary>Kết nối đang đóng (Close).</summary>
    Closed = 0,

    /// <summary>Kết nối đang mở và sẵn sàng sử dụng.</summary>
    Open = 1,

    /// <summary>Kết nối đang trong quá trình kết nối tới dữ liệu (đang mở).</summary>
    Connecting = 2,

    /// <summary>Kết nối đang thực thi một lệnh (Command execution).</summary>
    Executing = 4,

    /// <summary>Kết nối đang lấy dữ liệu từ Server/DB.</summary>
    Fetching = 8,

    /// <summary>Kết nối bị ngắt đột ngột hoặc hỏng (không thể sử dụng tiếp).</summary>
    Broken = 16
}

public enum EventCodeEnum : int
{
    UdpConnect = 1,
    UdpConnected = 2,
    UdpDisConnect = 3,
    UdpDisConnected = 4,
    ModuleLoad = 5,
    ModuleUnLoad = 6,
    ChangeUserPass = 7,

    /// <summary>Sự kiện phát sinh khi Thay đổi ngôn ngữ Form</summary>
    ChangeLanguage = 8,

    // ----------------------------------
    /// <summary>Giá trị TrangThai: 0 - 1</summary>
    TrangThai = 10,

    /// <summary>Tính chất tài khoản</summary>
    TaiKhoanTinhChat = 11,

    /// <summary></summary>
    ChungTuLoai = 12,

    /// <summary>Đơn vị tính thời gian (1.Ngày, 2.Tháng, 3.Năm)</summary>
    DvtThoiGian = 13,

    // ----------------------------------
    /// <summary>Thay đổi nội dung thuộc tính</summary>
    ThuocTinhItem = 14,

    // ----------------------------------
    /// <summary>Ngày trong tuần</summary>
    NgayTrongTuan = 15,

    /// <summary>Tháng trong năm</summary>
    ThangTrongNam = 16,

    /// <summary>Hằng số 0. Không, 1. Có</summary>
    KhongCo = 17,

    // ----------------------------------        
    /// <summary>Tín hiệu cuộc gọi điện thoại</summary>
    Crm_TelePhone = 21,

    /// <summary>Tín hiệu tin nhắn SMS</summary>
    Crm_Sms = 22,

    /// <summary>Tín hiệu Email</summary>
    Crm_Email = 23,

    /// <summary>Tín hiệu Chat</summary>
    Crm_Chatting = 24,

    // ----------------------------------
    /// <summary>Hướng cuộc gọi, SMS, Chat, ...</summary>
    CrmHuong = 25,

    /// <summary>Tình trạng cuộc gọi (Nhỡ, đang nghe, gác máy, ghi âm)</summary>
    CrmTelTinhTrang = 26,

    /// <summary>Tình trạng SMS (1: Chưa gửi nhận, 2: Đang gửi nhận, 3: Thất bại, 4: Thành công)</summary>
    CrmSmsTinhTrang = 27,

    /// <summary>Tính chất kênh điện thoại (1: Cố định, 2: GMS, 3: Tổng đài IP)</summary>
    CrmTelKenhTinhChat = 28,

    /// <summary>Tình trạng Email (1: Chưa gửi nhận, 2: Đang gửi nhận, 3: Thất bại, 4: Thành công)</summary>
    CrmEmailTinhTrang = 29,

    // ----------------------------------
    /// <summary>1: Khoản thu, 2: Khoản chi</summary>
    KhoanMucPhiTinhChat = 30,

    /// <summary>Nguồn vốn</summary>
    NguonVon = 31,

    /// <summary>1: Đầu vào, 2: Đầu ra</summary>
    HoaDonTinhChat = 33,

    /// <summary>Danh mục hành chính các cấp</summary>
    HanhChinhCacCap = 36,

    /// <summary>Tính chất đơn vị hành chính (Quốc gia, Vùng miền - Bang, Tỉnh thành, Quận huyện, Xã Phường, Thôn xóm, Tổ)</summary>
    HanhChinhTinhChat = 37,

    DuongPho = 38,
    CrmBlackList = 39,

    // ----------------------------------
    /// <summary>TaiKhoan</summary>
    TaiKhoan = 50,

    /// <summary>TaiKhoanNhom</summary>
    TaiKhoanNhom = 51,

    // TaiKhoanKetChuyen = 52,
    // TaiKhoanNgamDinh = 53,
    // TaiKhoanTuDong = 54,

    /// <summary>YeuToChiPhi</summary>
    YeuToChiPhi = 55,

    /// <summary>Sản phẩm công trình, yếu tố tập hợp chi phí</summary>
    SanPhamCongTrinh = 56,

    /// <summary>Khoản mục phí, Khoản mục thu</summary>
    KhoanMucThuChi = 57,

    /// <summary>Tiền tệ Item</summary>
    TienTe = 58,

    /// <summary>PhongBanItem</summary>
    PhongBan = 59,

    /// <summary>Tính chất phòng ban</summary>
    PhongBanTinhChat = 60,

    /// <summary>Thuế GTGT</summary>
    ThueGTGT = 61,

    /// <summary>Thuế TNCN</summary>
    ThueTNCN = 62,

    /// <summary>Thuế TTĐB</summary>
    ThueTTDB = 63,

    /// <summary>Thuế XNK</summary>
    ThueXNK = 64,

    /// <summary>Tính chất tiền tệ</summary>
    TienTeTinhChat = 65,

    /// <summary>Tính chất kiểm kê</summary>
    KiemKeTinhChat = 66,

    KiemKeTimKiemTinhChat = 67,

    // ----------------------------------
    /// <summary>Đối tượng Item</summary>
    DoiTuong = 100,

    /// <summary>Phân nhóm khách hàng, nhà cung cấp</summary>
    DoiTuong_Nhom = 101,

    /// <summary>Kiểu quan hệ khách hàng, nhà cung cấp</summary>
    DoiTuong_KieuQH = 102,

    /// <summary>Phân loại khách hàng, nhà cung cấp</summary>
    DoiTuong_Loai = 103,

    /// <summary>Tính chất đối tượng</summary>
    DoiTuongTinhChat = 104,

    /// <summary>Nhà cung cấp hàng hóa</summary>
    DoiTuong_NhaCungCap = 105,

    /// <summary>Khách mua hàng</summary>
    DoiTuong_KhachHang = 106,

    /// <summary>Chủ sở hữu, cổ đông</summary>
    DoiTuong_ChuSoHuu = 107,

    /// <summary>Nhân viên trong công ty</summary>
    DoiTuong_NhanVien = 108,

    /// <summary>Cơ quan hành chính</summary>
    DoiTuong_CoQuanHanhChinh = 109,

    /// <summary>Tính chất ảnh của đối tượng</summary>
    DoiTuongTinhChatAnh = 110,

    /// <summary>Xưng Hô ("Ông", "Bà", "Anh", "Chị", "Em")</summary>
    DoiTuong_XungHo = 111,

    /// <summary>Nguồn hình thành đối tượng</summary>
    DoiTuong_Nguon = 112,

    HangThanhVien = 113,

    // ----------------------------------
    /// <summary>Hàng hóa Item</summary>
    HangHoa = 150,

    /// <summary>Phân nhóm hàng hóa</summary>
    HangHoa_Nhom = 151,

    /// <summary>Phân kiểu hàng hóa</summary>
    HangHoa_Kieu = 152,

    /// <summary>Phân loại hàng hóa</summary>
    HangHoa_Loai = 153,

    /// <summary>Tính chất vật tư, hàng hóa</summary>
    HangHoaTinhChat = 154,

    /// <summary>Tính chất ảnh vật tư, hàng hóa</summary>
    HangHoaTinhChatAnh = 155,

    HangHoaGasNuoc = 156,
    HangHoaBoxType = 157,
    HangHoaBox = 158,
    HangHoaPhongBan = 159,

    // ----------------------------------
    /// <summary>Tài sản Item</summary>
    TaiSan = 160,

    /// <summary>Phân nhóm tài sản</summary>
    TaiSan_Nhom = 161,

    /// <summary>Phân kiểu tài sản</summary>
    TaiSan_Kieu = 162,

    /// <summary>Phân loại tài sản</summary>
    TaiSan_Loai = 163,

    /// <summary>TaiSan_TinhChat</summary>
    TaiSanTinhChat = 164,

    /// <summary>Mục đích sử dụng</summary>
    TaiSan_MucDichSuDung = 165,

    /// <summary>Lý do tăng giảm tài sản</summary>
    TaiSan_LyDoTangGiam = 166,

    // ----------------------------------
    /// <summary>Văn Bản Item</summary>
    VanBan = 170,

    /// <summary>Loại văn bản</summary>
    VanBan_Loai = 171,

    /// <summary>Nhóm văn bản</summary>
    VanBan_Nhom = 172,

    /// <summary>Cơ quan ban hành văn bản</summary>
    VanBan_CoQuan = 173,

    /// <summary>Độ khẩn cấp VB</summary>
    VanBanDoKhan = 174,

    /// <summary>Độ bảo mật VB</summary>
    VanBanDoBaoMat = 175,

    /// <summary>Độ quan trọng VB</summary>
    VanBanDoQuanTrong = 176,

    /// <summary>Hướng văn bản: 1: Đến, 2: Đi, 3: Nội bộ</summary>
    VanBanHuong = 177,

    /// <summary>Văn bản bước</summary>
    VanBanBuoc = 178,

    /// <summary>Tình trạng xử lý (0:Chưa xử lý , 1: Đang xử lý, 2: Đã xử lý, 3: Hoàn thành, 4: Hoàn thành - Chưa thanh lý HĐ, 5: Đình chỉ, 6: Hủy bỏ)</summary>
    VanBanTinhTrangXuLy = 179,

    /// <summary>Tính chất mẫu văn bản (1: Hợp đồng, 2: Kiểm định chất lượng, ...)</summary>
    VanBanMauTinhChat = 180,

    /// <summary>1: Hợp đồng mua, 2: Hợp đồng bán</summary>
    HopDongTinhChat = 181,

    /// <summary>0:Chưa xử lý , 1: Đang xử lý, 2: Đã xử lý, 3: Hoàn thành, 4: Hoàn thành - Chưa thanh lý HĐ, 5: Đình chỉ, 6: Hủy bỏ</summary>
    HopDongTinhTrangXuLy = 182,

    VanBanTinhChat = 183,

    /// <summary>Văn bản pháp luật</summary>
    VanBan_PL = 190,

    // ----------------------------------
    /// <summary>Danh mục</summary>
    DanToc_DanToc = 201,

    /// <summary>Danh mục</summary>
    DanToc_TonGiao = 202,

    /// <summary>Liệt kê chức vụ, sau thống kê cho thuận tiện (Đối với đơn vị nhiều Cán bộ, Nhân viên)</summary>
    NsChucVu = 203,

    /// <summary>Phân cấp bậc học: Tiểu học, Trung Học, Đại Học, Cao Học</summary>
    NsTrinhDoHocVan = 204,

    /// <summary>Liệt kê chuyên ngành học: Cấp Trung Học, Cao Đẳng, Đại học, nghề, sẽ có ngành học chuyên môn</summary>
    NsChuyenNganh = 205,

    /// <summary>Bằng cấp thì có nhiều, một người có thể có nhiều bằng cấp khác nhau</summary>
    NsBangCap = 206,

    /// <summary>Loại đào tạo: Chính Quy, Tại Chức, Từ xa</summary>
    NsLoaiDaoTao = 207,

    /// <summary>Trình độ ngoại ngữ: Tiếng Anh C, ...</summary>
    NsTrinhDoNgoaiNgu = 208,

    // ----------------------------------
    /// <summary>CtMuaHang</summary>
    CtMuaHang = 300,

    /// <summary>Hình thức thanh toán</summary>
    HinhThucThanhToan = 311,

    /// <summary>Phương thức vận chuyển</summary>
    PhuongThucVanChuyen = 312,

    /// <summary>1: Giảm trừ giá trị cụ thể, 2: Giảm trừ theo tỷ lệ, 3: Áp dụng giá cố định</summary>
    PhuongThucChietKhau = 313,

    GiaoHangTinhTrang = 315,
    PhuongTienVanChuyen = 316,
    BaoHanhTinhTrang = 317,

    CtNhapKhoHangHoa = 331,
    CtNhapMua = 332,
    CtNhapMuaKhongQuaKho = 333,
    CtNhapMuaXuatTra = 334,
    CtNhapMuaXuatTraKhongQuaKho = 335,
    CtNhapKyGui = 336,
    CtNhapKyGuiXuatTra = 337,
    CtXacNhanMuaNhapKyGui = 338,
    CtGhiNoVoPhaiTra = 339,
    CtXuatDoiVoBinh = 340,
    CtXacNhanBanKyGuiTraThuaNcc = 341,
    CtNhapKhoCCDC = 342,
    CtNhapThanhPham = 343,
    CtNhapKhuyenMai = 344,
    CtNhapKhoVatTu = 345,
    CtNhapKhuyenMaiTuDong = 346,
    CtNhapLaiHangHuy = 347,
    CtDieuChuyenNoiBo = 348,
    CtXuatKhoHangHoa = 349,
    CtXuatBan = 350,
    CtXuatBanKhongQuaKho = 351,
    CtHangBanBiTraLai = 352,
    CtHangBanBiTraLaiKhongQuaKho = 353,
    CtXuatKyGui = 354,
    CtXuatKyGuiTraLai = 355,
    CtXacNhanBanKyGui = 356,
    CtGhiNoVoPhaiThu = 357,
    CtNhapDoiVoBinh = 358,
    CtXacNhanMuaKyGuiKhTraThua = 359,
    CtXuatKhoCCDC = 360,
    CtXuatThanhPham = 361,
    CtXuatKhuyenMai = 362,
    CtXuatKhoVatTu = 363,
    CtXuatKhuyenMaiTuDong = 364,
    CtXuatHuyHangHong = 365,
    CtGiaoHang = 366,
    CtThuTien = 367,
    CtChiTien = 368,
    CtNhapQuy = 369,
    CtXuatQuy = 370,
    CtDieuChuyenQuy = 371,
    CtKeToan = 372,
    CtKiemKeTon = 373,
    CtKiemKeCanDoiThua = 374,
    CtKiemKeCanDoiThieu = 375,
    CtDonDatHang = 376,
    CtDonDatMua = 377,
    CtBaoGia = 378,
    CtChaoGia = 379,
    CtHoaDonVat_DauVao = 380,
    CtHoaDonVat_DauRa = 381,
    CtSanXuat = 382,
    CtSanXuatNhatKy = 383,
    CtSanXuatYeuCau = 384,
    CtSanXuatKeHoachVatTu = 388,
    CtBaoHanh = 386,
    CtBaoHanhTraLai = 387,
    CtBaoHanhXuatNcc = 388,
    CtBaoHanhNhapNcc = 389,

    /// <summary>CongViec</summary>
    CongViec = 401,

    /// <summary>Tình trạng công việc</summary>
    CongViecTinhTrang = 402,

    CongViecNhom = 403,

    /// <summary>AdminNsd</summary>
    AdminNsd = 600,

    /// <summary>AdminNsdNhom</summary>
    AdminNsdNhom = 601,

    /// <summary>AdminTramLamViec</summary>
    AdminTramLamViec = 602,

    /// <summary>AdminServer</summary>
    AdminServer = 603,

    AdminNsdTinhChat = 604,

    // --------------------------------
    /// <summary>PathDataEnum</summary>
    PathData = 610,

    /// <summary>RowStateEnum</summary>
    RowState = 611,

    // --------------------------------
    /// <summary>ThuocTinhTinhChat</summary>
    ThuocTinhTinhChat = 613,

    /// <summary>Loại nhập thành phẩm</summary>
    SxLoaiID = 614,

    /// <summary>Nguồn lập đơn sản xuất</summary>
    SxTcID = 615,

    /// <summary>Danh mục tính chất hệ đo lường</summary>
    HeDoLuongTinhChat = 616,

    /// <summary>Thời hạn: Còn, hết hạn</summary>
    ThoiHanEnum = 617,

    /// <summary>HangHoaTinhChatGas</summary>
    HangHoaTinhChatGas = 618,

    /// <summary>Kiểu đơn giá</summary>
    HangHoaDonGiaKieu = 619,

    /// <summary>Đơn vị tính hàng hóa</summary>
    HangHoaDvt = 620,

    /// <summary>Kiểu nhập xuất Serial</summary>
    HangHoaSerialKieuNX = 621,

    /// <summary>Thứ tự danh mục: Nhóm - Kiểu - Loại</summary>
    DanhMucNhomKieuLoai = 622,

    /// <summary>Sinh số chứng từ tự động kiểu</summary>
    CtSoDuKienKieu = 623,

    /// <summary>Phép toán so sánh</summary>
    QuerryOperator = 624,

    /// <summary>Kiểu dữ liệu</summary>
    DataType = 625,

    /// <summary>Kiểu dữ liệu Crm (Điện thoại, Sms, Email)</summary>
    CrmContactType = 626,

    /// <summary>Tiến độ thực hiện</summary>
    LlvTienDo = 627,

    // --------------------------------
    /// <summary>Phân nhóm tài liệu truyền thông ra bên ngoài</summary>
    TaiLieu_NhomID = 630,

    /// <summary>TaiLieu_GioiThieu</summary>
    TaiLieu_GioiThieu = 631,

    /// <summary>Tài liệu huấn luyện an toàn</summary>
    TaiLieu_HLAT = 632,

    // --------------------------------
    /// <summary>Câu hỏi khảo sát</summary>
    TaiLieu_KhaoSat = 633,

    /// <summary>Lĩnh vực khảo sát</summary>
    TaiLieu_KhaoSatLinhVuc = 634,

    // --------------------------------
    /// <summary>Loại chi phí sản xuất SxChiPhiLoai</summary>
    SxChiPhiLoai = 635,

    /// <summary>Giới tính</summary>
    NsGioiTinh = 637,

    /// <summary>Nhân viên</summary>
    NsNhanVien = 638,

    /// <summary>Ký hiệu chấm công</summary>
    NsChamCongKyHieu = 639,

    /// <summary>Hợp đồng lao động</summary>
    NsHopDongLaoDong = 640,

    /// <summary>Tính chất hợp đồng lao động</summary>
    NsHdLdTinhChat = 641,

    /// <summary>Tín hiệu Ca làm việc (Không có Danh mục, Danh mục tính chất)</summary>
    NsCaLamViec = 642,

    /// <summary>Tín hiệu thay đổi ngôn ngữ Danh Mục Loại</summary>
    NgonNguDanhMucLoai = 643,

    /// <summary>Tín hiệu thay đổi ngôn ngữ Danh Mục</summary>
    NgonNguDanhMuc = 644,

    /// <summary>Tín hiệu thay đổi ngôn ngữ Danh Mục Tính chất</summary>
    NgonNguDanhMucTinhChat = 645,

    /// <summary>Tín hiệu thay đổi danh mục</summary>
    DanhMuc = 646,

    /// <summary>Tín hiệu thay đổi danh mục tính chất</summary>
    DanhMucTinhChat = 647,

    /// <summary>Tín hiệu thay đổi danh mục loại</summary>
    DanhMucLoai = 648,

    /// <summary>Đối tượng tập hợp chi phí</summary>
    SxDoiTuongTHCP = 649,

    /// <summary>Tính chất đối tượng tập hợp chi phí</summary>
    SxDoiTuongTHCPTinhChat = 650,

    // -------------------------------
    /// <summary>Tính chất Giá bao gồm thuế</summary>
    CtGiaBaoGomThue = 651,

    /// <summary>Tính chất Chiết khấu trước thuế</summary>
    CtChietKhauTruocThue = 652,

    HangHoaTemChinhHang_TinhTrang = 653,

    /// <summary>Định mức sản xuất</summary>
    SxDinhMuc = 654,

    /// <summary>Đối tượng Chat</summary>
    ChatDoiTuong = 657,

    /// <summary>Tín hiệu chấm công</summary>
    NsChamCong = 658,

    /// <summary>Tài khoản chấm công</summary>
    NsChamCongTaiKhoan = 659,

    /// <summary>Quyền tài khoản chấm công</summary>
    NsChamCongTkQuyen = 660,

    /// <summary>Thiết bị chấm công</summary>
    NsChamCongDevice = 661,

    /// <summary>Kiểu kết nối thiết bị ngoại vi</summary>
    ConnectType = 662,

    /// <summary>Kiểu hiển thị 1. Ngang, 2: Lưới, 3: Dọc</summary>
    ViewType = 663,

    /// <summary>Nguồn tạo ra công việc</summary>
    CongViecNguonPhatSinh = 665,

    /// <summary>Khuyến mại tính chất</summary>
    KhuyenMaiTinhChat = 666,

    /// <summary>Baner quảng cáo</summary>
    CrmBanner = 667,

    /// <summary>Tính chất banner</summary>
    CrmBannerTinhChat = 668,

    /// <summary>Vị trí xuất hiện banner (1: Trên, 2: Dưới, 3: Trái, 4: Phải, 5: Giữa)</summary>
    CrmBannerViTri = 669,

    /// <summary>Tình trạng chu kỳ mua hàng của đối tượng</summary>
    CrmDoiTuongChuKy = 670,

    /// <summary>AdminNsdPhone, tài khoản khi đăng ký thiết bị di động</summary>
    AdminNsdPhone = 671,

    /// <summary>Chat về việc gì (Nhặt 3 loại trong CongViecNguonPhatSinh)</summary>
    CrmChatType = 672,

    /// <summary>Chu kỳ thời gian tính khuyến mại sau khi bán hàng</summary>
    CrmKmSbhChuKy = 673,

    /// <summary>Định dạng hiển thị dữ liệu</summary>
    DinhDangHienThi = 674,

    /// <summary>Kiểu bản đồ</summary>
    MapsType = 675,

    /// <summary>Khảo sát - Kiểu nhập dữ liệu</summary>
    KhaoSatKieuNhapDL = 676,

    /// <summary>Trạng thái xuất nhập kho (1 đã nhập xuất, 0: Chưa nhập xuất)</summary>
    NhapXuatKhoTrangThai = 677,

    /// <summary>Danh mục ngân hàng</summary>
    NganHang = 678,

    /// <summary>Thay đổi giá trị bảng tính KM SBH</summary>
    CrmKmSbhDuDau = 679,

    // Tên dạng tài khoản
    PhoneAppType = 680,

    // Hằng số sự kiện BIService gửi tới máy tính
    EventToPc = 681,

    // Thay đổi thông tin Kênh cuộc gọi điện thoại
    CrmTelKenh = 682,

    // Tình trạng thanh toán đơn hàng
    ThanhToanTinhTrang = 683,

    // Phương tiện chi trả khuyến mại: Tiền mặt, Thẻ tích điểm, Phiếu mua hàng, Hàng hóa
    KhuyenMaiPhuongTien = 684,

    // Lớp mạng: WAN, LAn
    NetworkLayer = 685,

    // Kiểu tài khoản tham gia mua hàng (Tiêu dùng, hoặc Đại lý hưởng đa cấp)
    KieuTaiKhoan = 686,

    // Phương pháp tính giá vốn
    PhuongPhapTinhGiaVon = 687,

    // Lịch thanh toán
    LichThanhToan = 688,

    /// <summary>Danh sách Event_Name của Zalo được hỗ trợ</summary>
    ZaloEventName = 700,

    /// <summary>Có tin nhắn từ Zalo</summary>
    ZaloMessage = 701,

    /// <summary>Mẫu in phiếu từ xa</summary>
    AdminRemotePrint = 702,

    KtDinhKhoan = 703,

    /// <summary>Kiểu công nợ</summary>
    KieuCongNo = 704,

    /// <summary>Loại</summary>
    VanBanPlLoai = 705,

    /// <summary>Cơ quan</summary>
    VanBanPlCoQuan = 706,

    /// <summary>Người ký</summary>
    VanBanPlNguoiKy = 707,

    /// <summary>Lĩnh vực (Đệ quy)</summary>
    VanBanPlLinhVuc = 708,

    /// <summary>Thủ tục Cơ quan</summary>
    VanBanPlTtCoQuan = 709,

    /// <summary>Thủ tục lĩnh vực (Đệ quy)</summary>
    VanBanPlTtLinhVuc = 710,

    /// <summary>Tuyến đường vận chuyển hàng hóa</summary>
    TuyenDuongVanChuyen = 711,

    /// <summary>Danh sách Event_Name của FaceBook được hỗ trợ</summary>
    FaceBookEventName = 712,

    /// <summary>Có tin nhắn từ FaceBook</summary>
    FaceBookMessage = 713,

    CrmSmsGsm = 714,

    /// <summary>Chặn sửa dữ liệu theo: Ngày cụ thể, hoặc số ngày cũ</summary>
    LockDataType = 715,

    /// <summary>Loại khuyến mại tự động</summary>
    KhuyenMaiTuDongLoai = 716,

    /// <summary>Trạng thái kết quả tìm kiếm (1: Tồn tại, 0: Không tồn tại, -1 Lỗi)</summary>
    TimKiemKq = 717,

    /// <summary>Loại giấy tờ cá nhân</summary>
    GiayToCaNhanLoai = 718,

    /// <summary>Loại hình doanh nghiệp</summary>
    DoanhNghiepLoaiHinh = 719,

    /// <summary>Danh mục đơn vị tính hàng hóa</summary>
    DonViTinh = 720,

    /// <summary>Các mẫu xml xuất hóa đơn điện tử, Tải lên Cơ Quan Thuế</summary>
    HoaDon_XmlTemplate = 721,

    /// <summary>Đối tượng trên hóa đơn (1 Cá nhân, 2: Doanh Nghiệp, 3. Hành chính sự nghiệp</summary>
    DoiTuongHoaDon = 722,

    /// <summary>Loại thuế: 1 Thuế XNK, 2. Thuế Ttđb, 3. Gtgt, 4. TNDN, 5. TNCN</summary>
    ThueLoai = 723,

    /// <summary>1 Thường xuyên, 2: Không thường xuyên</summary>
    ThueTncnTinhChat = 723_1, // Lưu ý: Do trong mã nguồn gốc dòng này thiếu giá trị khởi tạo nên ở đây gán 723_1 hoặc tự động theo thứ tự C#

    TaskerEvent = 724,

    /// <summary>Nhà cung cấp hóa đơn</summary>
    HoaDonNhaCungCap = 725
}
public enum TrangThaiEnum : int
{
    /// <summary>Còn hiệu lực</summary>
    ConHieuLuc = 1,
    /// <summary>Hết hiệu lực</summary>
    HetHieuLuc = 2,
    /// <summary>Xóa bởi hệ thống, không khôi phục</summary>
    XoaBo = 3,
    /// <summary>Xóa bởi người sử dụng (cho trạng thái = StateUsed)</summary>
    DeleteByUser = 4,
    /// <summary>Xóa bởi người sử dụng (cho trạng thái = StateUnUsed)</summary>
    DeleteByUserUnused = 5
}

public enum TaiKhoanTinhChatEnum : int
{
    DuNo = 1,
    DuCo = 2,
    LuongTinh = 3,
    KhongCoSoDu = 4
}

public enum CtLoaiEnum : int
{
    /// <summary>Nhập kho hàng hóa (Nhập từ: Điều chuyển, Sửa chữa, Nhập thành phẩm, Đổi, ...)</summary>
    NhapKhoHangHoa = 1,
    /// <summary>Nhập mua (Nhập mua kinh doanh)</summary>
    NhapMua = 2,
    /// <summary>Nhập mua hàng hóa không qua kho (Nhập mua kinh doanh không qua kho)</summary>
    NhapMuaKhongQuaKho = 3,
    /// <summary>Trả hàng lại NCC, giảm tồn kho</summary>
    NhapMuaXuatTra = 4,
    /// <summary>Trả hàng lại NCC, không giảm tồn kho</summary>
    NhapMuaXuatTraKhongQuaKho = 5,
    /// <summary>Nhận ký gửi, bán hộ, mượn (Nhận ký gửi, bán hộ, MƯỢN VỎ RỜI)</summary>
    NhapKyGui = 6,
    /// <summary>Trả vỏ cũ cho NCC, giảm công nợ vỏ với NCC (Trả vỏ nhà cung cấp - Đều giảm kho)</summary>
    NhapKyGuiXuatTra = 7,
    /// <summary>Ghi nhận MUA lượng vỏ đang nợ (Giảm công nợ vỏ)</summary>
    XacNhanMuaNhapKyGui = 8,
    /// <summary>Kế toán ghi nợ phải trả hoặc trả thừa</summary>
    GhiNoVoPhaiTra = 9,
    /// <summary>Xuất kho trả lại vỏ bình mua, Không giảm nợ phải trả vỏ (Vì vỏ này là kèm bình mua) khác với xuất trả ký gửi CtLoaiID = NhapKyGuiXuatTra</summary>
    XuatDoiVoBinh = 10,
    /// <summary>Xác nhận bán ký gửi trả thừa Ncc (Thừa do nhiều lần mua trước, vv...)</summary>
    XacNhanBanKyGuiTraThuaNcc = 11,
    /// <summary>Nhập kho CCDC (Nhập kho công cụ dụng cụ - Theo dõi tồn theo Ccdc)</summary>
    NhapKhoCCDC = 12,
    /// <summary>Nhập kho thành phẩm (Thành phẩm tạo thành)</summary>
    NhapThanhPham = 13,
    /// <summary>Nhập hàng khuyến mại (Có giá nhập, nhưng không phát sinh chi phí, giá vốn = 0)</summary>
    NhapKhuyenMai = 14,
    /// <summary>Nhập kho vật tư, do sản xuất dư thừa, hoặc hoàn nguyên</summary>
    NhapKhoNvl = 15,
    NhapKhuyenMaiTuDong = 16,
    /// <summary>Nhập lại hàng hỏng, hủy, hết date do xuất không đúng</summary>
    NhapLaiHangHuy = 17,
    /// <summary>Điều chuyển hàng hóa nội bộ, lưu một dòng có KhoXuatID và KhoNhapID > 0</summary>
    DieuChuyenNoiBo = 20,
    /// <summary>Xuất kho hàng hóa (Xuất: Điều chuyển, Lắp ráp, sản xuất, hủy hàng hỏng, xuất đổi, ...)</summary>
    XuatKhoHangHoa = 21,
    /// <summary>Xuất bán (Xuất bán kinh doanh, có giảm tồn kho)</summary>
    XuatBan = 22,
    /// <summary>Xuất bán kinh doanh, không giảm tồn kho (Xuất bán vỏ trên bình, không ghi nhận giảm kho)</summary>
    XuatBanKhongQuaKho = 23,
    /// <summary>Khách trả lại hàng, có tăng tồn kho</summary>
    HangBanBiTraLai = 24,
    /// <summary>Mua vỏ trên bình "hàng bán trả lại"</summary>
    HangBanBiTraLaiKhongQuaKho = 25,
    /// <summary>Xuất ký gửi thường, Xuất vỏ rời cho mượn</summary>
    XuatKyGui = 26,
    /// <summary>Trả vỏ, giảm công nợ vỏ</summary>
    XuatKyGuiTraLai = 27,
    /// <summary>Bán vỏ khách đang nợ, ko giảm kho, giảm vỏ nợ phải thu</summary>
    XacNhanBanKyGui = 28,
    /// <summary>Ghi nợ (N) hoặc trả thừa (C) vỏ khách mua hàng</summary>
    GhiNoVoPhaiThu = 29,
    /// <summary>Nhập kho khách trả lại vỏ bình, không ghi giảm nợ phải thu vỏ (Vì vỏ này là vỏ kèm bình đi bán), khác với xuất ký gửi CtLoaiID = XuatKyGui</summary>
    NhapDoiVoBinh = 30,
    /// <summary>Xác nhận mua lại vỏ khách trả thừa (Tức phải thu nợ vỏ âm)</summary>
    XacNhanMuaKyGuiKhTraThua = 31,
    /// <summary>Xuất kho Công cụ dụng cụ</summary>
    XuatKhoCCDC = 32,
    /// <summary>Xuất kho thành phẩm (Để sửa chữa thành phẩm bị lỗi)</summary>
    XuatThanhPham = 33,
    /// <summary>Xuất khuyến mại (Có giá bán, nhưng không tính doanh thu)</summary>
    XuatKhuyenMai = 34,
    /// <summary>Xuất kho vật tư để sản xuất</summary>
    XuatKhoNvl = 35,
    XuatKhuyenMaiTuDong = 36,
    /// <summary>Xuất hủy hàng hỏng, hết date</summary>
    XuatHuyHangHong = 37,
    /// <summary>Mua nguyên bình</summary>
    NhapMuaBinh = 38,
    /// <summary>Bán nguyên bình</summary>
    XuatBanBinh = 39,
    /// <summary>Dữ liệu lưu trữ trong CtGiaohang (Nhưng vẫn có một dòng header trong CtNghiepVuCt)</summary>
    GiaoHang = 41,
    /// <summary>Thu tiền - Lưu dữ liệu trong CtChungTu,CtChungTuChiTiet (Thu tiền bán hàng, thu tiền cược: Phân biệt dựa vào tài khoản kế toán)</summary>
    ThuTien = 61,
    /// <summary>Chi tiền - Lưu dữ liệu trong CtChungTu,CtChungTuChiTiet (Chi tiền mua hàng, chi tiền cược: Phân biệt dựa vào tài khoản kế toán)</summary>
    ChiTien = 62,
    /// <summary>Nhập quỹ tiền mặt</summary>
    NhapQuy = 63,
    /// <summary>Xuất quỹ tiền mặt</summary>
    XuatQuy = 64,
    /// <summary>Điều chuyển quỹ tiền</summary>
    DieuChuyenQuy = 65,
    /// <summary>Chứng từ kế toán (Nsd tự hạch toán tài khoản)</summary>
    KeToan = 100,
    /// <summary>Kiểm kê, điều chỉnh tồn</summary>
    KiemKeTon = 101,
    /// <summary>Hàng thực tế nhiều hơn máy (Thừa hàng) phải xử lý giảm số lượng trên máy</summary>
    KiemKeCanDoiThua = 102,
    /// <summary>Hàng thực tế ÍT hơn máy (Thiếu hàng) phải xử lý tăng số lượng trên máy</summary>
    KiemKeCanDoiThieu = 103,
    /// <summary>Khách đặt mua hàng</summary>
    DonDatHang = 120,
    /// <summary>Doanh nghiệp đặt mua từ nhà cung cấp</summary>
    DonDatMua = 121,
    /// <summary>Báo giá cho khách hàng</summary>
    BaoGia = 122,
    /// <summary>Nhà cung cấp chào giá</summary>
    ChaoGia = 123,
    /// <summary>Hóa đơn VAT đầu vào</summary>
    HoaDonVat_DauVao = 124,
    /// <summary>Hóa đơn VAT đầu ra</summary>
    HoaDonVat_DauRa = 125,
    /// <summary>Lệnh Sản xuất hàng hóa</summary>
    SanXuat = 200,
    /// <summary>Nhật ký sản xuất hàng hóa</summary>
    SanXuatNhatKy = 202,
    /// <summary>Yêu cầu sản xuất</summary>
    SanXuatYeuCau = 203,
    /// <summary>Kế hoạch vật tư</summary>
    SanXuatKeHoachVatTu = 204,
    /// <summary>Nhận Bảo hành sản phẩm từ khách hàng</summary>
    BaoHanh = 317,
    /// <summary>Trả bảo hành sản phẩm cho khách hàng</summary>
    BaoHanhTraLai = 318,
    /// <summary>Xuất hàng tới hãng sản xuất</summary>
    BaoHanhXuatNcc = 319,
    /// <summary>Nhận lại bảo hành từ hãng</summary>
    BaoHanhNhapNcc = 320,
    /// <summary>Tính lương và lưu chứng từ hạch toán lương</summary>
    HachToanLuong = 330,
    /// <summary>Thực hiện chi tiền lương</summary>
    ChiTraLuong = 331
}

public enum DvtThoiGianEnum : int
{
    Gio = 1,
    Ngay = 2,
    Thang = 3,
    Nam = 4
}

public enum NgayTrongTuanEnum : int
{
    CN = 1,
    T2 = 2,
    T3 = 3,
    T4 = 4,
    T5 = 5,
    T6 = 6,
    T7 = 7
}

public enum ThangTrongNamEnum : int
{
    TH01 = 1,
    TH02 = 2,
    TH03 = 3,
    TH04 = 4,
    TH05 = 5,
    TH06 = 6,
    TH07 = 7,
    TH08 = 8,
    TH09 = 9,
    TH10 = 10,
    TH11 = 11,
    TH12 = 12
}
internal class Enumdeclare
{
}
public enum KhongCoEnum : int
{
    Co = 1,
    Khong = 2
}

// 25
public enum CrmHuongEnum : int
{
    _In = 1,
    _Out = 2,
    _Internal = 3
}

public enum CrmTelTinhTrangEnum : int
{
    Unknown = 0,
    // Khởi động cuộc gọi - Mức 1
    /// <summary>Cuộc gọi đến (đang gọi đến)</summary>
    CallIn = 1,
    /// <summary>Cuộc gọi đi (Đang gọi đi)</summary>
    CallOut = 2,

    // Thất bại - Mức 2
    /// <summary>Không trả lời cuộc gọi đến (Gọi nhỡ)</summary>
    Unanswered = 3,
    /// <summary>Hủy khi có cuộc gọi</summary>
    Rejected = 4,

    // Có nghe gọi - Mức 3
    /// <summary>Nghe máy (Trả lời điện thoại gọi đến)</summary>
    HookOff = 5,
    /// <summary>Trạng thái đang đàm thoại</summary>
    Talking = 6,    
    /// <summary>Gác máy (Kết thúc cuộc gọi (Áp dụng cả gọi đến và gọi đi)</summary>
    HangUp = 7,

    // Có ghi âm (Lớn nhất) - Mức 4
    /// <summary>File ghi âm cuộc đàm thoại</summary>
    RecordFile = 8
}

public enum CrmSmsTinhTrangEnum : int
{
    SoanThao = 0,
    ChuaGuiNhan = 1,
    DangGuiNhan = 2,
    ThatBai = 3,
    DaGuiNhan = 4
}

/// <summary>28. CrmDienThoaiKenh.TinhChat</summary>
public enum CrmTelKenhTinhChatEnum : int
{
    CoDinh = 1,
    DiDong = 2,
    TongDaiPbx = 3,
    SmsBrandName = 4,
    SmsGsmModem = 5
}
/// <summary>29. CrmEmailTinhTrang</summary>
public enum CrmEmailTinhTrangEnum : int
{
    /// <summary>Email soạn thảo để gửi đi nhưng chưa gửi</summary>
    ChuaGuiNhan = 1,
    /// <summary>Email nhận về, chưa tải nội dung, chỉ tải header</summary>
    DangGuiNhan = 2,
    /// <summary>Email gửi đi/ nhận về thất bại</summary>
    ThatBai = 3,
    /// <summary>Email đã gửi/ nhận về thành công</summary>
    DaGuiNhan = 4,
    /// <summary>Email đã nhận nhưng chưa đọc</summary>
    ChuaDoc = 5
}

/// <summary>30. KhoanMucPhiTinhChatEnum</summary>
public enum KhoanMucPhiTinhChatEnum : int
{
    KhoanThu = 1,
    KhoanChi = 2
}

/// <summary>33. HoaDonTinhChatEnum</summary>
public enum HoaDonTinhChatEnum : int
{
    DauVao = 1,
    DauRa = 2
}

/// <summary>37. HanhChinhTinhChatEnum</summary>
public enum HanhChinhTinhChatEnum : int
{
    QuocGia = 1,
    VungMien = 2,
    TinhThanh = 3,
    QuanHuyen = 4,
    PhuongXa = 5,
    ThonXomTo = 6
}

/// <summary>60. PhongBanTinhChatID</summary>
public enum PhongBanTinhChatEnum : int
{
    /// <summary>Không chứa hàng</summary>
    PhongBanChiNhanh = 1,
    /// <summary>Lưu trữ hàng hóa</summary>
    KhoQuay = 2,
    /// <summary>Sản xuất và lưu trữ hàng hóa</summary>
    XuongSanXuat = 3,
    /// <summary>Vị trí thống kê, báo cáo</summary>
    KhuVucViTri = 4,
    /// <summary>Xe bán hàng</summary>
    XeBanHang = 5
}

/// <summary>65. TienTeTinhChat</summary>
public enum TienTeTinhChatEnum : int
{
    /// <summary>Tiền mặt</summary>
    TienMat = 1,
    /// <summary>Ngoại tệ</summary>
    NgoaiTe = 2,
    /// <summary>Vàng bạc</summary>
    VangBac = 3,
    /// <summary>Tích điểm</summary>
    TichDiem = 4,
    /// <summary>Phiếu mua hàng</summary>
    PhieuMuaHang = 5
}

/// <summary>104. DoiTuong_TinhChat</summary>
public enum DoiTuongTinhChatEnum : int
{
    NhaCungCap = 1,
    KhachHang = 2,
    ChuSoHuu = 3,
    NhanVien = 4,
    CoDong = 5
}

/// <summary>110. DoiTuong_TinhChatAnh</summary>
public enum DoiTuongTinhChatAnhEnum : int
{
    AnhDaiDien = 1,
    AnhCMTND = 2,
    AnhHangHoa = 3
}
/// <summary>113. Hạng thành viên</summary>
public enum HangThanhVienEnum : int
{
    KimCuong = 1,
    Vang = 2,
    Bac = 3,
    Dong = 4
}

/// <summary>154. HangHoa_TinhChat</summary>
public enum HangHoaTinhChatEnum : int
{
    HangHoaVatTu = 1,
    LapRap = 2,
    DichVu = 3,
    ThanhPham = 4,
    KhuyenMaiDoiCoin = 5,
    MaGop = 6,
    DienGiai = 9
}

/// <summary>155. HangHoa_TinhChatAnh</summary>
public enum HangHoaTinhChatAnhEnum : int
{
    AnhDaiDien = 1,
    AnhHangHoa = 2,
    AnhThatGia = 3
}

/// <summary>156. HangHoa_Gas</summary>
public enum HangHoaGasNuocEnum : int
{
    Binh = 1,
    Vo = 2,
    Vo_CongNo = 3,
    Ruot = 4,
    VoKien = 5
}

/// <summary>157. HangHoaBoxType</summary>
public enum HangHoaBoxTypeEnum : int
{
    /// <summary>Khuyến mại sốc</summary>
    FlashSale = 1,
    /// <summary>Bán chạy</summary>
    BestSaler = 2,
    /// <summary>Đặt hàng nhanh theo từng nhóm hàng</summary>
    FastOrder = 3,
    /// <summary>Sản phẩm yêu thích</summary>
    Favourite = 4
}

/// <summary>311. HinhThucThanhToan</summary>
public enum HinhThucThanhToanEnum : int
{
    /// <summary>Tiền mặt 111</summary>
    TienMat = 1,
    /// <summary>Điện tử - Chuyển khoản</summary>
    ChuyenKhoan = 2,
    /// <summary>Điện tử - Ví điện tử</summary>
    ViDienTu = 3,
    /// <summary>Điện tử - Quét thẻ máy POS</summary>
    QuetTheMayPos = 4,
    /// <summary>Trả góp (Lũy kế theo khách hàng)</summary>
    TraGop = 5,
    /// <summary>Thẻ tích điểm (Lũy kế theo khách hàng)</summary>
    TheTichDiem = 6,
    /// <summary>Phiếu mua hàng (Lũy kế theo khách hàng)</summary>
    PhieuMuaHang = 7,
    QuyTieuDung = 18
}

/// <summary>312. PhuongThucVanChuyen</summary>
public enum PhuongThucVanChuyenEnum : int
{
    /// <summary>Giao hàng tận nhà</summary>
    GiaoHangTanNha = 1,
    /// <summary>Khách tự mang về</summary>
    KhachMangVe = 2,
    /// <summary>Sử dụng tại quầy</summary>
    DungTaiQuay = 3,
    /// <summary>Thuê dịch vụ vận chuyển ngoài</summary>
    ThueNgoai = 4
}

/// <summary>315. Tình trạng giao hàng</summary>
public enum GiaoHangTinhTrangEnum : int
{
    /// <summary>Chờ duyệt - Chờ xác nhận</summary>
    ChoXacNhan = 1,
    /// <summary>Chờ lấy hàng - đã tiếp nhận</summary>
    ChoLayHang = 2,
    /// <summary>Đang giao hàng</summary>
    DangGiao = 3,
    /// <summary>Đã giao hàng thành công</summary>
    DaGiao = 4,
    /// <summary>Khách hủy đơn hàng</summary>
    KhachHuyBo = 5,
    /// <summary>Giao thất bại - không liên lạc được với khách hàng</summary>
    GiaoThatBai = 6,
    /// <summary>Đơn hàng bị trả lại</summary>
    BiTraLai = 7,
    /// <summary>Người bán hủy đơn</summary>
    NguoiBanHuyBo = 8
}

/// <summary>317. Tính chất bảo hành, sửa chữa</summary>
public enum BaoHanhTinhTrangEnum : int
{
    NhanBaoHanh = 1,
    XuatBaoHanh = 2,
    NhanSuaChua = 3,
    TraSuaChua = 4,
    NhanBaoHanhTuHang = 5,
    ChuyenBaoHanhLenHang = 6
}

/// <summary>402. CongViec_TinhTrang</summary>
public enum CongViecTinhTrangEnum : int
{
    // Lập công việc
    ChuaXem = 1,
    DaXem = 2,

    // Chưa tiếp nhận (Không hoặc đang ủy quyền)
    KhongTiepNhan = 3,
    UyQuyen = 4,

    // Tiếp nhận và làm việc
    TiepNhan = 5,
    DangThucHien = 6,
    HoanThanh = 7,

    // Phê duyệt sau hoàn thành
    ChuaPheDuyet = 8,
    DaPheDuyet = 9,
    HuyBo = 100
}

/// <summary>604. AdminNsdTinhChat</summary>
public enum AdminNsdTinhChatEnum : int
{
    /// <summary>Tài khoản PC, dùng với Nsd trên máy tính</summary>
    AdminNsd = 1,
    /// <summary>Tài khoản nhân viên, có thể chỉnh sửa đơn hàng cho khách Smart Phone</summary>
    AdminNsd_Nv = 2,
    /// <summary>Tài khoản khách hàng, Chỉ order và chỉnh sửa khi chưa duyệt Smart Phone</summary>
    AdminNsd_Kh = 3
}
/// <summary>610. PathData</summary>
public enum PathDataEnum : int
{
    HangHoa = 1,
    DoiTuong = 2,
    CrmEmail = 3,
    VanBan = 4,
    VanBanMau = 5,
    TaiSan = 6,
    CrmGhiChu = 7,
    HopDong = 8
}

/// <summary>611. RowState</summary>
public enum RowStateEnum : int
{
    None = 1,
    AddNew = 2,
    Modified = 3,
    Deleted = 4
}

/// <summary>616. HeDoLuongEnum</summary>
public enum HeDoLuongTinhChatEnum : int
{
    ChieuDai = 4,
    TheTich = 5,
    KhoiLuong = 6,
    DienTich = 7,
    TocDo = 8,
    ApSuat = 9,
    KhoGiay = 10,
    ThoiGian = 11
}

/// <summary>617. ThoiHanEnum</summary>
public enum ThoiHanEnum : int
{
    DangConHan = 1,
    DaHetHan = 2
}

/// <summary>619. DonGiaKieu</summary>
public enum HangHoaDonGiaKieuEnum : int
{
    GiaNhap = 1,
    GiaBanLe = 2,
    GiaBanBuon = 3,
    GiaHang = 4
}

/// <summary>HangHoaDvt = 620</summary>
public enum HangHoaDvtEnum : int
{
    Dvt1 = 1,
    Dvt2 = 2,
    Dvt3 = 3
}

/// <summary>621. HangHoaSerialKieuNX</summary>
public enum HangHoaSerialKieuNX : int
{
    NhapMua = 1,
    XuatBan = 2,
    NoiBo = 4,
    DonHang = 8
}
/// <summary>Hoán đổi toàn bộ dữ liệu DBType, SystemType về dạng của phần mềm</summary>
public enum BIDataEnum : int
{
    /// <summary>Số nguyên</summary>
    _Number = 1,
    /// <summary>Số thập phân</summary>
    _Float = 2,
    /// <summary>Ngày tháng, giờ</summary>
    _Date = 3,
    /// <summary>Chuỗi ký tự</summary>
    _VarChar = 4,
    /// <summary>Byte, nhị phân</summary>
    _Binary = 5,
    /// <summary>Dữ liệu chuỗi có độ dài lớn VD: nText</summary>
    _Text = 6,
    /// <summary>dữ liệu rowguid, tạo trong sql với hàm newid()</summary>
    _Guid = 7
}
/// <summary>626. CrmType</summary>
public enum CrmContactTypeEnum : int
{
    DienThoai = 1,
    Sms = 2,
    Email = 3,
    FaceToFace = 4
}
/// <summary>627. Tiến độ công việc</summary>
public enum LlvTienDoEnum : int
{
    /// <summary>Dữ liệu khởi tạo có trường TienDo = -1</summary>
    ChuaThucHien = 1,
    /// <summary>Thiết lập = 0, sẽ tiếp nhận công việc</summary>
    TiepNhan = 2,
    /// <summary>Giá trị 0 &lt; tiến độ &lt; 100</summary>
    DangThucHien = 3,
    /// <summary>Thường là 100 (100%, hoàn thành)</summary>
    HoanThanh = 4
}

public enum KieuCongNoThem : int
{
    /// <summary>số dư đầu kỳ trong bảng TaiKhoanDuDauChiTietXXXX &lt;= TuNgay</summary>
    DauKySoDu = 1,
    /// <summary>số dư phát sinh &lt;= TuNgay, &gt;= ngày có số dư trong bảng TaiKhoanDuDauChiTietXXXX</summary>
    DauKyPhatSinh = 2,
    /// <summary>phát sinh trong khoảng TuNgay đến hết DenNgay</summary>
    TrongKyPhatSinh = 3
}

public enum BaoGiaEnum : int
{
    BaoGia_BanRa = CtLoaiEnum.BaoGia,
    BaoGia_MuaVao = CtLoaiEnum.ChaoGia
}

public enum DonDatHangEnum : int
{
    DonDatHang = CtLoaiEnum.DonDatHang,
    DonDatMua = CtLoaiEnum.DonDatMua
}

/// <summary>637. Giới tính Đối tượng, Nhân viên</summary>
public enum NsGioiTinhEnum : int
{
    Nam = 1,
    Nu = 2
}

// 641 Tính chất hợp đồng lao động
public enum NsHdLdTinhChatEnum : int
{
    ChinhThuc = 1,
    ThuViec = 2,
    ThoiVu = 3
}

// 650 Tính chất đối tượng tập hợp chi phí
// 1: Công trình, vụ việc, 2: Đơn hàng, 3: Phân xưởng, phòng ban, 4: Quy trình công nghệ, 5: Sản phẩm
public enum SxDoiTuongTHCPTinhChatEnum : int
{
    CongTrinhVuViec = 1,
    DonHang = 2,
    PhanXuongSanXuat = 3,
    QuyTrinhCongNghe = 4,
    SanPham = 5
}

// 651 Tính chất giá bao gồm thuế trong chứng từ
public enum CtGiaBaoGomThueEnum : int
{
    BaoGomThue = 1,
    KhongBaoGomThue = 2
}

// 652 Tính chất chiết khấu trong chứng từ
public enum CtChietKhauTruocThueEnum : int
{
    TruocThue = 1,
    SauThue = 2
}

// 653 Tình trạng tem chính hãng
public enum HangHoaTemChinhHangEnum : int
{
    KhoiTao = 1,
    InTem = 2,
    PhatHanh = 3,
    SuDung = 4,
    HuyBo = 5
}
public enum NsChamCongTKQuyenEnum : int
{
    Normal = 0,
    Enroll = 1,
    Admin = 2,
    SupperAdmin = 3,
    UserDefinedRoll = 4,
    UnDefine = 5
}

// 662 Kiểu kết nối thiết bị ngoại vi
public enum ConnectTypeEnum : int
{
    TCP_IP = 1,
    RS232_485 = 2,
    USB = 3
}

// 663 Kiểu hiển thị
public enum ViewTypeEnum : int
{
    /// <summary>Nhiều item trên một hàng ngang</summary>
    Ngang = 1,
    /// <summary>Nhiều item trên Nhiều cột nhiều dòng (2 chiều)</summary>
    Luoi = 2,
    /// <summary>Mỗi dòng 1 item, Dạng bài viết báo</summary>
    Doc = 3
}

// 665 Nguồn tạo ra công việc - Để lọc cho từng ứng dụng khác nhau
public enum CongViecNguonPhatSinhEnum : int
{
    GiaoViec = 1,             // Giao việc thủ công, tạo bằng phần mềm

    DienThoai = 2,            // TelID
    Chat = 3,                 // ChatID     Phát sinh từ nhắn tin
    Sms = 4,                  // Tin nhắn đến
    Email = 5,                // Từ Email đến
    GhiChu = 6,               // Việc được tạo ra từ liên hệ trực tiếp GcID
    ChamCong = 7,             // ChamCongID Phát sinh từ chấm công
    HopDong = 8,              // HdID        Phát sinh từ hợp đồng
    GasAlert_RoRiGas = 9,     // Cảnh báo rò rỉ gas (Hỗ trợ khách hàng)
    GasAlert_HetGas = 10,     // Báo hết gas

    HangHoaBaoTri = 30,       // Việc cần bảo trì (Cho HangHoa có HangHoa,BaoTri > 0, VD: 30 ngày thay lõi lọc nước số 1)
    HangHoaChuKy = 31,        // Việc cần mời mua mới khi hết hạn sử dụng (Cho HangHoa có HangHoa.HanSuDung > 0), VD 1 tháng hết bình gas, sau 1 tháng sẽ liệt kê để thay bình mới

    CtBanHang = 150,          // CtID Công việc bán hàng
    CtBaoGia = 151,           // BaoGiaID   Công việc báo giá
    CtBaoHanh = 152,          // BhID       Công việc bảo hành
    CtHoaDon = 153,           // HdID       Công việc xuất hóa đơn
    CtDonDatHang = 154,       // DdhID      Công việc đặt hàng
    CtThuTien = 155,          // CtID       Công việc thu tiền
    CtGiaoHang = 156,         // GhID       Công việc giao hàng
    CtDonDatHangStatus = 157, // Trạng thái đơn hàng
    CtPheDuyet = 158,         // Phát sinh công việc "Phê duyệt chứng từ"

    // Đánh giá từ khách hàng
    DanhGia = 200,         // Đánh giá chung của khách hàng (PbID,CtID,DdhID,NvID)
    DanhGiaCH = 201,       // Đánh giá về cửa hàng bán
    DanhGiaDdh = 202,      // Đánh giá đơn đặt hàng
    DanhGiaGh = 203,       // Đánh đơn giao
    DanhGiaNv = 204,       // Đánh nhân viên giao
    DanhGiaHh = 205        // Đánh sản phẩm khách mua
}

// 666 Khuyến mại tính chất
public enum KhuyenMaiTinhChatEnum : int
{
    /// <summary>Giá bán theo khách hàng, thay cho HangHoaDonGiaKh,HangHoaDonGiaKhNhatKy</summary>
    GiaTheoKhach = 1,
    /// <summary>Khuyến mại bán hàng</summary>
    KhuyenMaiDon = 2,
    /// <summary>Khuyến mại sau bán hàng, thường tính tổng các đơn bán sau 1 khoảng thời gian</summary>
    KhuyenMaiSauBan = 3,
    /// <summary>Khuyến mại cho khách hàng mới khởi tại, lập mới (Dựa vào ngày tạo)</summary>
    KhachMoiKhoiTao = 4
}

// 668 CrmBannerTinhChat
public enum CrmBannerTinhChatEnum : int
{
    // 1: BIsolution, 2: SmartPhone, 3: Web
    BIsolution = 1,
    SmartPhone = 2,
    Web = 3
}

// 669 CrmBannerViTri
public enum CrmBannerViTriEnum : int
{
    // 1: Trên, 2: Dưới, 3: Trái, 4: Phải, 5: Giữa
    Tren = 1,
    Duoi = 2,
    Trai = 3,
    Phai = 4,
    Giua = 5
}

public enum CrmDoiTuongChuKyEnum : int
{
    ChuaCoGiaoDich = 1,
    ChuaDenHan = 2,
    DaDenHan = 3,
    DaQuaHan = 4,
    CoTheMat = 5
}

// 672 ChatType
public enum CrmChatTypeEnum : int
{
    PhongBan = 1,
    NhanVien = 2,
    DonDatHang = 3,
    HangHoa = 4
}

public enum ComboAddZeroType : int
{
    /// <summary>Không chèn thêm giá trị 0 nào, Combo chỉ các các item thật</summary>
    NoAdd = 0,
    /// <summary>Chèn thêm 'Tất cả' với giá trị 0, để không chọn điều kiện tìm kiếm</summary>
    FindAll = 1,
    /// <summary>Chèn item rỗng với giá trị 0, để nhập ID = 0</summary>
    Blank = 2
}

// 675 Kiểu bản đồ
public enum MapsTypeEnum : int
{
    HYBRID = 1,
    ROADMAP = 2,
    SATELLITE = 3,
    TERRAIN = 4
}
// 677 NhapXuatKhoTrangThaiEnum Khảo sát - Kiểu nhập dữ liệu
public enum NhapXuatKhoTrangThaiEnum : int
{
    /// <summary>Đã nhập/xuất kho</summary>
    DaNhapXuat = 1,
    /// <summary>Chưa xuất/nhập kho</summary>
    ChuaNhapXuat = 2
}
// 680 AdminNsdPhoneAccountType
public enum PhoneAppTypeEnum : int
{
    /// <summary>Ứng dụng nhân viên</summary>
    NhanVien = 1,
    /// <summary>Ứng dụng khách hàng</summary>
    KhachHang = 2
}
// 681 Hằng số sự kiện BIService gửi tới máy tính
public enum EventToPcEnum : int
{
    // Các hằng tín hiệu nội tại BIservice truyền về PC, không phải dạng trượt hỗ trợ khác
    biservice_PhoneTelSet = 1,    // Tín hiệu điện thoại từ SmartPhone => Biservice => PC
    biservice_PhoneTelRecSet = 2,
    biservice_PhoneSmsSet = 3,        // Tín hiệu Sms đến
    biservice_PhoneEmailSet = 4,      // Tín hiệu Email đến
    biservice_PhoneChatSet = 5,       // Tín hiệu Chat đến
    biservice_PhoneCommentSet = 6,    // Ghi chú, đánh giá

    // Các tín hiệu phát sinh từ các hàm khác
    biservice_AccountSet = 7,
    biservice_PhoneLogin = 8,
    biservice_PhoneGpsSet = 9,
    biservice_PhoneDoiTuongSet = 10,
    biservice_PhoneKiemKeSet = 11,
    biservice_PhoneCartToOrder = 12,
    biservice_PhoneOrderSet = 13,
    biservice_PhoneInvoiceSet = 14,
    biservice_AccountVerify = 15,
    biservice_PhoneOrderStatusSet = 16,
    // Tín hiệu rò rỉ gas
    biservice_GasLeakRoRiGas = 17,
    biservice_GasLeakHetGas = 18,
    // Tín hiệu Zalo
    biservice_ZaloOA = 19,
    // Tín hiệu FaceBook
    biservice_FaceBook = 20,
    // Sms cần gửi
    biservice_SmsGsm = 21,
    // Tín hiệu nhận Notifycation của ứng dụng Tasker Notify
    biservice_TaskerEvent = 22
}

// 683 Tình trạng thanh toán đơn hàng
public enum ThanhToanTinhTrangEnum : int
{
    DaThanhToan = 1,
    ThanhToanThieu = 2,
    ChuaThanhToan = 3
}

// 685 Lớp mạng
public enum NetworkLayerEnum : int
{
    IPAddressLAN = 1,
    IPAddressWAN = 2
}

// 686 Kiểu tài khoản
public enum KieuTaiKhoanEnum : int
{
    /// <summary>Khách tiêu dùng thông thường, không hưởng Quy chế Đại lý, Đa cấp</summary>
    TieuDung = 1,
    /// <summary>Khách kinh doanh,Hưởng Quy chế Đại lý, Đa cấp</summary>
    KinhDoanh = 2
}

// 688. Lịch thanh toán
public enum LichThanhToanEnum : int
{
    /// <summary>Thanh toán theo đơn hàng</summary>
    DonHang = 1,
    /// <summary>Thanh toán gối đầu</summary>
    GoiDau = 2,
    /// <summary>Thanh toán cố định một ngày trong tháng</summary>
    NgayTrongThang = 3
}

/// <summary>700. Danh sách Event_Name của Zalo được hỗ trợ</summary>
public enum ZaloEventNameEnum : int
{
    user_received_message = 1,
    oa_send_anonymous_text = 2,
    oa_send_anonymous_image = 3,
    oa_send_anonymous_sticker = 4,
    oa_send_anonymous_file = 5,
    anonymous_send_text = 6,
    anonymous_send_image = 7,
    anonymous_send_sticker = 8,
    anonymous_send_file = 9,
    oa_send_text = 10,
    oa_send_image = 11,
    oa_send_gif = 12,
    oa_send_list = 13,
    oa_send_file = 14,
    oa_send_sticker = 15,
    user_send_text = 16,
    user_send_image = 17,
    user_send_link = 18,
    user_send_audio = 19,
    user_send_video = 20,
    user_send_sticker = 21,
    user_send_location = 22,
    user_send_business_card = 23,
    user_send_file = 24
}

// 704. Kiểu công nợ
public enum KieuCongNoEnum : int
{
    PhaiThu = 1,
    PhaiTra = 2
}

// 715 Chặn sửa dữ liệu cũ
public enum LockDataTypeEnum : int
{
    /// <summary>Căn cứ theo ngày cụ thể</summary>
    ByDate = 1,
    /// <summary>Căn cứ theo số ngày về trước đã nhập</summary>
    ByDayCount = 2
}

// 719 Loại hình doanh nghiệp
public enum DoanhNghiepLoaiHinhEnum : int
{
    HoKinhDoanh = 1,
    CongTy = 2
}

// 723  Loại thuế
public enum ThueLoaiEnum : int
{
    /// <summary>Xuất nhập khẩu</summary>
    XNK = 1,
    /// <summary>Tiêu thụ đặc biệt</summary>
    TTDB = 2,
    /// <summary>Bảo vệ môi trường</summary>
    BVMT = 3,
    /// <summary>Giá trị gia tăng</summary>
    GTGT = 4,
    /// <summary>Thu nhập doanh nghiệp</summary>
    TNDN = 5,
    /// <summary>Thu nhập cá nhân</summary>
    TNCN = 6
}

// 724  Tính chất thuế thu nhập cá nhân
public enum ThueTncnTinhChatEnum : int
{
    ThuongXuyen = 1,
    KhongThuongXuyen = 2
}

// 725  Nhà cung cấp hóa đơn
public enum HoaDonNhaCungCapEnum : int
{
    Viettel = 1,
    Misa = 2,
    Vnpt = 3
}

// 726  Nhà cung cấp hóa đơn
public enum TelSourceEnum
{
    Unknown = 0,
    CallScreening = 1,
    BroadcastReceiver = 2,
}