using Shipping.Models;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Shipping.Models
{
	public class TinhThanh
	{
		[Key]
		[Column(TypeName = "nvarchar(10)")]
		public string Id { get; set; } = null!;

		[Required]
		[MaxLength(255)]
		public string TenTinhThanh { get; set; } = null!;


		//Foreign key
		[Required]
		[ForeignKey("VungMien")]
		public int VungMienId { get; set; }
		public virtual VungMien VungMien { get; set; } = null!;


		//Navigation
		public virtual ICollection<PhuongXa> PhuongXas { get; set; } = null!;

		public virtual ICollection<ChiNhanh> ChiNhanhs { get; set; } = null!;

		public virtual ICollection<NhanVien> NhanViens { get; set; } = null!;

		public virtual ICollection<Shipper> Shippers { get; set; } = null!;

		public virtual ICollection<KhachHang> KhachHangs { get; set; } = null!;

		[InverseProperty("TinhThanhGui")]
		public virtual ICollection<DonHang> GuiDonHangs { get; set; } = null!;

		[InverseProperty("TinhThanhNhan")]
		public virtual ICollection<DonHang> NhanDonHangs { get; set; } = null!;

		[InverseProperty("TinhThanhGui")]
		public virtual ICollection<ChuyenHang> GuiChuyenHangs { get; set; } = null!;

		[InverseProperty("TinhThanhNhan")]
		public virtual ICollection<ChuyenHang> NhanChuyenHangs { get; set; } = null!;
	}
}
