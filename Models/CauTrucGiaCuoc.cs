using Shipping.Models.Enums;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Shipping.Models
{
	public class CauTrucGiaCuoc
	{
		[Key]
		public int Id { get; set; }

		[Required]
		public LoaiVungGia LoaiVungGia { get; set; }

		[Required]
		[Column(TypeName = "decimal(12, 0)")]
		public decimal GiaCoBan { get; set; }

		[Required]
		[Column(TypeName = "decimal(12, 0)")]
		public decimal GiaTangThem { get; set; }

		[Required]
		[Column(TypeName = "decimal(12, 0)")]
		public decimal GiaVuot { get; set; }

		[Required]
		public int ThoiGianGiao {  get; set; }


		//Foreign key
		[Required]
		[ForeignKey("LoaiDichVu")]
		public int LoaiDichVuId { get; set; }
		[ValidateNever]
		public virtual LoaiDichVu LoaiDichVu { get; set; } = null!;
	}
}
