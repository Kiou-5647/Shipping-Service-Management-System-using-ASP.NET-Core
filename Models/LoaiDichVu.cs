using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using Shipping.Models;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Shipping.Models
{
	public class LoaiDichVu
	{
		[Key]
		public int Id { get; set; }

		[Required]
		[MaxLength(100)]
		[DisplayName("Tên dịch vụ")]
		public string TenDichVu { get; set; } = null!;

		[Required]
		[MaxLength(50)]
		[DisplayName("Mã dịch vụ")]
		public string MaDV { get; set; } = null!;

		[Column(TypeName = "nvarchar(max)")]
		[DisplayName("Mô tả")]
		public string MoTa { get; set; } = null!;

		[Required]
		[Column(TypeName = "decimal(12, 2)")]
		[DisplayName("Đơn vị tăng thêm")]
		public decimal DonViTangThem { get; set; }

		[Required]
		[Column(TypeName = "decimal(4, 2)")]
		[DisplayName("Mốc cân nặng")]
		public decimal MocCanNang { get; set; }

		[Required]
		[DisplayName("Thời gian giao tối thiểu")]
		public int ThoiGianToiThieu { get; set; }

		[Required]
		[DisplayName("Thời gian giao tối đa")]
		public int ThoiGianToiDa { get; set; }


		//Navigation
		[ValidateNever]
		public virtual ICollection<CauTrucGiaCuoc> CauTrucGiaCuocs { get; set; } = null!;

		[ValidateNever]
		public virtual ICollection<DonHang> DonHangs { get; set; } = null!;
	}
}
