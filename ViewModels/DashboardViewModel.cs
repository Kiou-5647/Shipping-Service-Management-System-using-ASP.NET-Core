using Shipping.Models;

namespace Shipping.ViewModels
{
	public class DashboardViewModel
	{
		public int DonHangMoiHomNay { get; set; }
		public decimal DoanhThuHomNay { get; set; }
		public int DonHangChoXuLy { get; set; }
		public int ShipperDangBan { get; set; }


		public List<string> LabelsNgay { get; set; } = new List<string>();
		public List<decimal> DataDoanhThu { get; set; } = new List<decimal>();
		public List<int> DataDonHang { get; set; } = new List<int>();


		public int CountDaGiao { get; set; }
		public int CountDangGiao { get; set; }
		public int CountHuy { get; set; }


		public IEnumerable<DonHang> DonHangMoiNhat { get; set; } = new List<DonHang>();
	}
}
