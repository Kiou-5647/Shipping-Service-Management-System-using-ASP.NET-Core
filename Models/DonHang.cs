using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using Shipping.Models.Enums;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Shipping.Models
{
	public class DonHang
	{
		[Key]
		[DisplayName("Mã đơn hàng")]
		[Column(TypeName = "nvarchar(10)")]
		public string Id { get; set; } = null!;

		[DisplayName("Ngày tạo")]
		public DateTime NgayTao { get; set; } = DateTime.Now;

		[DisplayName("Ngày cập nhật")]
		public DateTime NgayCapNhat { get; set; } = DateTime.Now;

		[Required]
		[DisplayName("Trạng thái đơn hàng")]
		public TrangThaiDonHang TrangThaiDH { get; set; } = TrangThaiDonHang.ChoXuLy;

		[DisplayName("Người gửi")]
		public string? TTNguoiGui { get; set; } = null!;

		[Required]
		[DisplayName("Địa chỉ gửi")]
		[Column(TypeName = "nvarchar(max)")]
		public string DiaChiGui { get; set; } = null!;

		[DisplayName("Người nhận")]
		public string? TTNguoiNhan { get; set; } = null!;

		[Required]
		[DisplayName("Địa chỉ nhận")]
		[Column(TypeName = "nvarchar(max)")]
		public string DiaChiNhan { get; set; } = null!;


		[Required]
		[DisplayName("Chiều dài")]
		[Column(TypeName = "decimal(12, 2)")]
		public decimal ChieuDai { get; set; }

		[Required]
		[DisplayName("Chiều rộng")]
		[Column(TypeName = "decimal(12, 2)")]
		public decimal ChieuRong { get; set; }

		[Required]
		[DisplayName("Chiều cao")]
		[Column(TypeName = "decimal(12, 2)")]
		public decimal ChieuCao { get; set; }

		[Required]
		[DisplayName("Trọng lượng đơn hàng")]
		[Column(TypeName = "decimal(12, 2)")]
		public decimal TrongLuongThuc { get; set; }

		[Required]
		[DisplayName("Trọng lượng quy đổi")]
		[Column(TypeName = "decimal(12, 2)")]
		public decimal TrongLuongQuyDoi { get; set; }


		[Required]
		[DisplayName("COD")]
		[Column(TypeName = "decimal(12, 0)")]
		public decimal COD { get; set; }

		[Required]
		[DisplayName("Phí giao hàng")]
		[Column(TypeName = "decimal(12, 0)")]
		public decimal PhiGiaoHang { get; set; }

		[Required]
		[DisplayName("Phí phụ trợ")]
		[Column(TypeName = "decimal(12, 0)")]
		public decimal TongPhiPhuTro { get; set; }

		[DisplayName("Thông tin phí phụ trợ")]
		[Column(TypeName = "nvarchar(max)")]
		public string? ChiTietPhi { get; set; } = null!;

		[Required]
		[DisplayName("Thành tiền")]
		[Column(TypeName = "decimal(12, 0)")]
		public decimal ThanhTien { get; set; }

		[Required]
		[DisplayName("Phương thức thanh toán")]
		public PhuongThucThanhToan PTThanhToan { get; set; }

		[Required]
		[DisplayName("Trạng thái thanh toán")]
		public TrangThaiThanhToan TrangThaiTT { get; set; } = TrangThaiThanhToan.ChoThanhToan;


		[DisplayName("Hình ảnh đơn hàng")]
		[Column(TypeName = "nvarchar(max)")]
		public string? HinhAnhGoiHang { get; set; }

		[DisplayName("Hình ảnh xác nhận")]
		[Column(TypeName = "nvarchar(max)")]
		public string? HinhAnhXacNhan { get; set; }

		[DisplayName("Ghi chú")]
		[Column(TypeName = "nvarchar(max)")]
		public string? GhiChu { get; set; }


		//Foreign Key
		[ForeignKey("KhachHang")]
		[DisplayName("Khách hàng")]
		public int? KhachHangId { get; set; }
		[ValidateNever]
		public virtual KhachHang? KhachHang { get; set; }

		[ForeignKey("NhanVien")]
		[DisplayName("Nhân viên thanh toán")]
		public int? NhanVienId { get; set; }
		[ValidateNever]
		public virtual NhanVien? NhanVien { get; set; }

		[Required]
		[ForeignKey("LoaiDichVu")]
		[DisplayName("Loại dịch vụ")]
		public int LoaiDichVuId { get; set; }
		[ValidateNever]
		public virtual LoaiDichVu LoaiDichVu { get; set; } = null!;

		[Required]
		[ForeignKey("TinhThanhGui")]
		[DisplayName("Tỉnh/thành gửi")]
		[Column(TypeName = "nvarchar(10)")]
		public string TinhGuiId { get; set; } = null!;
		[ValidateNever]
		[InverseProperty("GuiDonHangs")]
		public virtual TinhThanh TinhThanhGui { get; set; } = null!;

		[Required]
		[ForeignKey("PhuongXaGui")]
		[DisplayName("Phường/xã gửi")]
		[Column(TypeName = "nvarchar(10)")]
		public string PhuongGuiId { get; set; } = null!;
		[ValidateNever]
		[InverseProperty("GuiDonHangs")]
		public virtual PhuongXa PhuongXaGui { get; set; } = null!;

		[Required]
		[ForeignKey("TinhThanhNhan")]
		[DisplayName("Tỉnh/thành nhận")]
		[Column(TypeName = "nvarchar(10)")]
		public string TinhNhanId { get; set; } = null!;
		[ValidateNever]
		[InverseProperty("NhanDonHangs")]
		public virtual TinhThanh TinhThanhNhan { get; set; } = null!;

		[Required]
		[ForeignKey("PhuongXaNhan")]
		[DisplayName("Phường/xã nhận")]
		[Column(TypeName = "nvarchar(10)")]
		public string PhuongNhanId { get; set; } = null!;
		[ValidateNever]
		[InverseProperty("NhanDonHangs")]
		public virtual PhuongXa PhuongXaNhan { get; set; } = null!;

		//Navigation
		public virtual ICollection<ChuyenHang> ChuyenHangs { get; set; } = null!;
	}
}
