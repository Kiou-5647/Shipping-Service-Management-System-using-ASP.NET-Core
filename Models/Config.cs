using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using Shipping.Models.Enums;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Shipping.Models
{
	public class Config
	{
		[Key]
		public string Id { get; set; } = null!;

		[Required]
		[DisplayName("Tên")]
		[ValidateNever]
		public string Ten { get; set; } = null!;

		[Required]
		[DisplayName("Loại")]
		public LoaiConfig Loai { get; set; } = LoaiConfig.TuDong;

		[Required]
		[DisplayName("Loại giá trị")]
		public LoaiGiaTri LoaiGiaTri { get; set; } = LoaiGiaTri.None;

		[Required]
		[DisplayName("Giá trị")]
		[Column(TypeName = "decimal(12, 2)")]
		public decimal GiaTri { get; set; }

		[Column(TypeName = "nvarchar(max)")]
		public string? MoTa { get; set; }

	}
}
