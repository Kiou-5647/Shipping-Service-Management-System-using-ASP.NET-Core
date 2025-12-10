using Shipping.Models.Enums;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Shipping.Models
{
	[PrimaryKey(nameof(DonHangId), nameof(ThuTu))]
	public class ChuyenHang
	{
		[DisplayName("Mã đơn hàng")]
		[Column(TypeName = "nvarchar(10)")]
		[ForeignKey("DonHang")]
		public string DonHangId { get; set; } = null!;
		public virtual DonHang DonHang { get; set; } = null!;

		[DisplayName("Thứ tự")]
		public int ThuTu { get; set; }

		[DisplayName("Ngày bắt đầu")]
		public DateTime? NgayTao { get; set; }

		[DisplayName("Ngày hoàn thành")]
		public DateTime? NgayHoanThanh { get; set; }

		[Required]
		[DisplayName("Trạng thái chuyến")]
		public TrangThaiDonHang TrangThai { get; set; } = TrangThaiDonHang.ChoXuLy;

		[Required]
		[DisplayName("Địa chỉ gửi")]
		[Column(TypeName = "nvarchar(max)")]
		public string DiaChiGui { get; set; } = null!;

		[Required]
		[DisplayName("Địa chỉ nhận")]
		[Column(TypeName = "nvarchar(max)")]
		public string DiaChiNhan { get; set; } = null!;

		[DisplayName("Hình ảnh giao hàng")]
		[Column(TypeName = "nvarchar(max)")]
		public string? HinhAnhGiaoHang { get; set; } = null!;

		[Column(TypeName = "nvarchar(max)")]
		public string? ViTri { get; set; } = null!;

		[Column(TypeName = "nvarchar(max)")]
		public string? GhiChu {  get; set; } = null!;


		//Foreign key
		[ForeignKey("Shipper")]
		[DisplayName("Shipper")]
		public int? ShipperId { get; set; }
		public virtual Shipper Shipper { get; set; } = null!;

		[Required]
		[ForeignKey("TinhThanhGui")]
		[DisplayName("Tỉnh/Thành gửi")]
		[Column(TypeName = "nvarchar(10)")]
		public string TinhGuiId { get; set; } = null!;
		[ValidateNever]
		[InverseProperty("GuiChuyenHangs")]
		public virtual TinhThanh TinhThanhGui { get; set; } = null!;

		[Required]
		[ForeignKey("PhuongXaGui")]
		[DisplayName("Phường/xã gửi")]
		[Column(TypeName = "nvarchar(10)")]
		public string PhuongGuiId { get; set; } = null!;
		[ValidateNever]
		[InverseProperty("GuiChuyenHangs")]
		public virtual PhuongXa PhuongXaGui { get; set; } = null!;

		[Required]
		[ForeignKey("TinhThanhNhan")]
		[DisplayName("Tỉnh/Thành nhận")]
		[Column(TypeName = "nvarchar(10)")]
		public string TinhNhanId { get; set; } = null!;
		[ValidateNever]
		[InverseProperty("NhanChuyenHangs")]
		public virtual TinhThanh TinhThanhNhan { get; set; } = null!;

		[Required]
		[ForeignKey("PhuongXaNhan")]
		[DisplayName("Phường/xã nhận")]
		[Column(TypeName = "nvarchar(10)")]
		public string PhuongNhanId { get; set; } = null!;
		[ValidateNever]
		[InverseProperty("NhanChuyenHangs")]
		public virtual PhuongXa PhuongXaNhan { get; set; } = null!;
	}
}
