using Shipping.Models;
using Shipping.Models.Enums;
using Microsoft.AspNetCore.Identity;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Shipping.Models
{
	public class Shipper
	{
		[Key]
		public int Id { get; set; }

		[Required]
		[MaxLength(100)]
		[DisplayName("Họ tên")]
		public string Ten { get; set; } = null!;

		[Required]
		[Column(TypeName = "nvarchar(20)")]
		[DisplayName("CCCD")]
		public string CCCD { get; set; } = null!;

		[Required]
		[DisplayName("Giới tính")]
		public bool GioiTinh { get; set; }

		[Required]
		[Column(TypeName = "date")]
		[DisplayName("Ngày sinh")]
		[DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}", ApplyFormatInEditMode = true)]
		public DateTime NgaySinh { get; set; }

		[Required]
		[Column(TypeName = "nvarchar(200)")]
		[DisplayName("Địa chỉ")]
		public string DiaChi { get; set; } = null!;

		[Required]
		public bool IsDeleted { get; set; } = false;

		[Required]
		[DisplayName("Loại shipper")]
		public LoaiShipper LoaiShipper { get; set; }

		[Required]
		[DisplayName("Trạng thái")]
		public TrangThaiShipper TrangThai { get; set; } = TrangThaiShipper.Nghi;

		[Required]
		[MaxLength(20)]
		[DisplayName("Biển số xe")]
		public string BienSoXe { get; set; } = null!;

		[MaxLength(500)]
		[DisplayName("Ảnh đại diện")]
		public string? HinhAnh { get; set; } = null!;

		[Column(TypeName = "nvarchar(max)")]
		public string? ViTri { get; set; }

		[DisplayName("Ngày cập nhật")]
		public DateTime NgayCapNhat { get; set; } = DateTime.Now;

		//Foreign Key
		[ForeignKey("User")]
		[DisplayName("Mã người dùng")]
		public string UserId { get; set; } = null!;
		public virtual IdentityUser User { get; set; } = null!;

		[ForeignKey("ChiNhanh")]
		[DisplayName("Chi nhánh")]
		public int? ChiNhanhId { get; set; }
		public virtual ChiNhanh? ChiNhanh { get; set; }

		[Required]
		[ForeignKey("TinhThanh")]
		[DisplayName("Tỉnh/Thành phố")]
		[Column(TypeName = "nvarchar(10)")]
		public string TinhThanhId { get; set; } = null!;
		public virtual TinhThanh TinhThanh { get; set; } = null!;

		[Required]
		[ForeignKey("PhuongXa")]
		[DisplayName("Phường/Xã")]
		[Column(TypeName = "nvarchar(10)")]
		public string PhuongXaId { get; set; } = null!;
		public virtual PhuongXa PhuongXa { get; set; } = null!;


		//Navigation
		public virtual ICollection<ChuyenHang> ChuyenHangs { get; set; } = null!;
	}
}
