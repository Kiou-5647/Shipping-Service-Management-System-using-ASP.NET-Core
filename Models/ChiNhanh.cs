using Shipping.Models;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Shipping.Models
{
	public class ChiNhanh
	{
		[Key]
		public int Id { get; set; }

		[Required]
		[MaxLength(255)]
		[DisplayName("Tên chi nhánh")]
		public string TenChiNhanh { get; set; } = null!;

		[Required]
		[Column(TypeName = "nvarchar(max)")]
		[DisplayName("Địa chỉ")]
		public string DiaChi { get; set; } = null!;

		[MaxLength(15)]
		[Column(TypeName = "nvarchar(15)")]
		[DisplayName("Số điện thoại")]
		public string SDT { get; set; } = null!;


		//Foreign key
		[Required]
		[ForeignKey("TinhThanh")]
		[DisplayName("Tỉnh/Thành phố")]
		[Column(TypeName = "nvarchar(10)")]
		public string TinhThanhId { get; set; } = null!;
		[ValidateNever]
		public virtual TinhThanh TinhThanh { get; set; } = null!;

		[Required]
		[ForeignKey("PhuongXa")]
		[DisplayName("Phường/Xã")]
		[Column(TypeName = "nvarchar(10)")]
		public string PhuongXaId { get; set; } = null!;
		[ValidateNever]
		public virtual PhuongXa PhuongXa { get; set; } = null!;


		//Navigation
		[ValidateNever]
		public virtual ICollection<NhanVien> NhanViens { get; set; } = null!;

		[ValidateNever]
		public virtual ICollection<Shipper> Shippers { get; set; } = null!;
	}
}
