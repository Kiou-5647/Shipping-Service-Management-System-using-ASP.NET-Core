using Shipping.Models.Enums;
using System.ComponentModel.DataAnnotations;

namespace Shipping.ViewModels
{
	public class ChuyenHangViewModel
	{
		public string DonHangId { get; set; } = null!;
		public int ThuTu { get; set; }


		public LoaiDiaDiem LoaiDiemDi { get; set; }
		public int? ChiNhanhDiId { get; set; }

		public LoaiDiaDiem LoaiDiemDen { get; set; }
		public int? ChiNhanhDenId { get; set; }

	
		[Required(ErrorMessage = "Vui lòng chọn Shipper")]
		public int ShipperId { get; set; }
		public string? TenShipper { get; set; }
		public string? SDTShipper { get; set; }

		
		public string? DiaChiGui { get; set; }
		public string? TinhGuiId { get; set; }
		public string? PhuongGuiId { get; set; }


		public string? TenTinhGui { get; set; }
		public string? TenPhuongGui { get; set; }


		public string? DiaChiNhan { get; set; }
		public string? TinhNhanId { get; set; }
		public string? PhuongNhanId { get; set; }


		public string? TenTinhNhan { get; set; }
		public string? TenPhuongNhan { get; set; }

		public decimal COD { get; set; }          
		public decimal TongTien { get; set; }     
		public string? TenNguoiNhanGoc { get; set; } 
		public string? SDTNguoiNhanGoc { get; set; } 
		public string? GhiChuDonHang { get; set; }

		public string? GhiChu { get; set; }
		public TrangThaiDonHang TrangThai { get; set; }
		public DateTime NgayTao { get; set; }

		public string? HinhAnhGiaoHang { get; set; }
	}
}
