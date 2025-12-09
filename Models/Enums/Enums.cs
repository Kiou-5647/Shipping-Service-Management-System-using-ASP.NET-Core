using System.ComponentModel.DataAnnotations;

namespace Shipping.Models.Enums
{
	public enum LoaiShipper
	{
		[Display(Name = "Giao hàng")]
		GiaoHang = 1,

		[Display(Name = "Vận chuyển")]
		VanChuyen = 2
	}

	public enum TrangThaiShipper
	{
		[Display(Name = "Sẵn sàng")]
		SanSang = 1,

		[Display(Name = "Bận")]
		Ban = 2,

		[Display(Name = "Nghỉ")]
		Nghi = 3
	}

	public enum TrangThaiDonHang
	{
		[Display(Name = "Chờ xử lý")]
		ChoXuLy = 1,

		[Display(Name = "Đang giao")]
		DangGiao = 2,

		[Display(Name = "Đã giao")]
		DaGiao = 3,

		[Display(Name = "Thất bại")]
		ThatBai = 4
	}

	public enum TrangThaiThanhToan
	{
		[Display(Name = "Chờ thanh toán")]
		ChoThanhToan = 1,

		[Display(Name = "Đã thanh toán")]
		DaThanhToan = 2
	}

	public enum PhuongThucThanhToan
	{
		[Display(Name = "Tiền mặt")]
		TienMat = 1,

		[Display(Name = "COD (Thu hộ)")]
		ThuHo = 2,

		[Display(Name = "Chuyển khoản")]
		NganHang = 3,
	}

	public enum LoaiVungGia
	{
		[Display(Name = "Nội tỉnh")]
		NoiTinh = 1,

		[Display(Name = "Nội miền")]
		NoiMien = 2,

		[Display(Name = "Liên miền")]
		LienMien = 3
	}
	public enum LoaiKhachHang
	{
		[Display(Name = "Cá nhân")]
		CaNhan = 1,

		[Display(Name = "Doanh nghiệp")]
		DoanhNghiep = 2
	}

	public enum LoaiConfig
	{
		[Display(Name = "Tự động")]
		TuDong = 1,

		[Display(Name = "Lựa chọn")]
		LuaChon = 2
	}

	public enum LoaiGiaTri
	{
		[Display(Name = "Phần trăm")]
		Percent = 1,

		[Display(Name = "Giá trị")]
		Fixed = 2,

		[Display(Name = "Trọng lượng")]
		Mass = 3,

		[Display(Name = "Độ dài")]
		Length = 4,

		[Display(Name = "Không có")]
		None = 5,
	}

	public enum LoaiDiaDiem
	{
		[Display(Name = "Địa chỉ Người gửi (Kho lấy)")]
		NguoiGui = 1,

		[Display(Name = "Địa chỉ Người nhận (Kho giao)")]
		NguoiNhan = 2,

		[Display(Name = "Chi nhánh / Kho trung chuyển")]
		ChiNhanh = 3,

		[Display(Name = "Tiếp nối chuyến trước")]
		TiepNoi = 4,

		[Display(Name = "Khác (Nhập tay)")]
		Khac = 5
	}
}
