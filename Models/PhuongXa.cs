using Shipping.Models;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Shipping.Models
{
	public class PhuongXa
	{
		[Key]
		[Column(TypeName = "nvarchar(10)")]
		public string Id { get; set; } = null!;

		[Required]
		[MaxLength(255)]
		public string TenPhuongXa { get; set; } = null!;

		
		//Foreign key
		[Required]
		[ForeignKey("TinhThanh")]
		public string TinhThanhId { get; set; } = null!;
		public virtual TinhThanh TinhThanh { get; set; } = null!;


		//Navigation
		public virtual ICollection<ChiNhanh> ChiNhanhs { get; set; } = null!;

		public virtual ICollection<NhanVien> NhanViens { get; set; } = null!;

		public virtual ICollection<Shipper> Shippers { get; set; } = null!;

		public virtual ICollection<KhachHang> KhachHangs { get; set; } = null!;

		[InverseProperty("PhuongXaGui")]
		public virtual ICollection<DonHang> GuiDonHangs { get; set; } = null!;

		[InverseProperty("PhuongXaNhan")]
		public virtual ICollection<DonHang> NhanDonHangs { get; set; } = null!;

		[InverseProperty("PhuongXaGui")]
		public virtual ICollection<ChuyenHang> GuiChuyenHangs { get; set; } = null!;

		[InverseProperty("PhuongXaNhan")]
		public virtual ICollection<ChuyenHang> NhanChuyenHangs { get; set; } = null!;
	}
}
