using Shipping.Models;
using Shipping.Models.Enums;
using Microsoft.AspNetCore.Identity;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Shipping.Models
{
	public class KhachHang
	{
		[Key]
		public int Id { get; set; }

		[Required]
		[MaxLength(100)]
		[DisplayName("Họ tên")]
		public string Ten { get; set; } = null!;

		[Column(TypeName = "nvarchar(20)")]
		[DisplayName("CCCD")]
		public string? CCCD { get; set; }

		[Column(TypeName = "nvarchar(20)")]
		[DisplayName("Mã số thuế")]
		public string? MaSoThue { get; set; }

		[Required]
		[DisplayName("Loại KH")]
		public LoaiKhachHang LoaiKhachHang { get; set; } = LoaiKhachHang.CaNhan;

		[Required]
		[Column(TypeName = "nvarchar(200)")]
		[DisplayName("Địa chỉ")]
		public string DiaChi { get; set; } = null!;

		[Required]
		public bool IsDeleted { get; set; } = false;

		[DisplayName("Ngày cập nhật")]
		public DateTime NgayCapNhat { get; set; } = DateTime.Now;


		//Foreign Key
		[ForeignKey("User")]
		[DisplayName("Mã người dùng")]
		public string UserId { get; set; } = null!;
		public virtual IdentityUser User { get; set; } = null!;

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
		public virtual ICollection<DonHang> DonHangs { get; set; } = null!;
	}
}
