using Shipping.Models.Enums;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Shipping.ViewModels
{
	public class DonHangViewModel
	{
		public string? Id { get; set; }

		[DisplayName("Ngày tạo")]
		public DateTime NgayTao { get; set; } = DateTime.Now;

		[DisplayName("Ngày cập nhật")]
		public DateTime NgayCapNhat { get; set; } = DateTime.Now;

		[Required]
		[DisplayName("Trạng thái đơn hàng")]
		public TrangThaiDonHang TrangThaiDH { get; set; } = TrangThaiDonHang.ChoXuLy;

		[Required]
		[DisplayName("Loại dịch vụ")]
		public int LoaiDichVuId { get; set; }

		//Thông tin người gửi
		[DisplayName("Thông tin Người gửi")]
		public string? TTNguoiGui { get; set; }

		[Required]
		[DisplayName("Địa chỉ gửi chi tiết")]
		public string DiaChiGui { get; set; } = null!;

		[Required]
		[DisplayName("Tỉnh/thành gửi")]
		public string TinhGuiId { get; set; } = null!;

		[Required]
		[DisplayName("Phường/xã gửi")]
		public string PhuongGuiId { get; set; } = null!;


		// Thông tin người nhận
		[DisplayName("Thông tin Người nhận")]
		public string? TTNguoiNhan { get; set; }

		[Required]
		[DisplayName("Địa chỉ nhận chi tiết")]
		public string DiaChiNhan { get; set; } = null!;

		[Required]
		[DisplayName("Tỉnh/thành nhận")]
		public string TinhNhanId { get; set; } = null!;

		[Required]
		[DisplayName("Phường/xã nhận")]
		public string PhuongNhanId { get; set; } = null!;


		//Thông tin đơn hàng
		[Required]
		[DisplayName("Chiều dài (cm)")]
		public decimal ChieuDai { get; set; }

		[Required]
		[DisplayName("Chiều rộng (cm)")]
		public decimal ChieuRong { get; set; }

		[Required]
		[DisplayName("Chiều cao (cm)")]
		public decimal ChieuCao { get; set; }

		[Required]
		[DisplayName("Trọng lượng (gram)")]
		public decimal TrongLuongThuc { get; set; }

		[DisplayName("Trọng lượng quy đổi")]
		public decimal TrongLuongQD { get; set; }

		[Required]
		[DisplayName("COD")]
		[Column(TypeName = "decimal(12, 0)")]
		public decimal COD { get; set; } = 0;

		[Required]
		[DisplayName("Phí giao hàng")]
		[Column(TypeName = "decimal(12, 0)")]
		public decimal PhiGiaoHang { get; set; } = 0;

		[Required]
		[DisplayName("Phí phụ trợ")]
		[Column(TypeName = "decimal(12, 0)")]
		public decimal TongPhiPhuTro { get; set; } = 0;

		[DisplayName("Thông tin phí phụ trợ")]
		[Column(TypeName = "nvarchar(max)")]
		public string? ChiTietPhi { get; set; } = null!;

		[Required]
		[DisplayName("Thành tiền")]
		[Column(TypeName = "decimal(12, 0)")]
		public decimal ThanhTien { get; set; } = 0;

		[Required]
		[DisplayName("Phương thức thanh toán")]
		public PhuongThucThanhToan PTThanhToan { get; set; }

		[Required]
		[DisplayName("Trạng thái thanh toán")]
		public TrangThaiThanhToan TrangThaiTT { get; set; } = TrangThaiThanhToan.ChoThanhToan;


		[DisplayName("Hình ảnh đơn hàng")]
		[Column(TypeName = "nvarchar(max)")]
		public string? HinhAnhGoiHang { get; set; }

		[DisplayName("Hình ảnh gói hàng")]
		public IFormFile? HinhAnhGoiHangFile { get; set; }


		[DisplayName("Hình ảnh xác nhận")]
		[Column(TypeName = "nvarchar(max)")]
		public string? HinhAnhXacNhan { get; set; }

		[DisplayName("Ghi chú")]
		[Column(TypeName = "nvarchar(max)")]
		public string? GhiChu { get; set; }

		[DisplayName("Mã Khách hàng")]
		public int? KhachHangId { get; set; }

		[DisplayName("Mã Nhân viên tạo")]
		public int? NhanVienId { get; set; }

		[DisplayName("Mã Shipper")]
		public int? ShipperId { get; set; }

		[DisplayName("Sử dụng bảo hiểm")]
		public bool SuDungBaoHiem { get; set; } = false;

	}
}
