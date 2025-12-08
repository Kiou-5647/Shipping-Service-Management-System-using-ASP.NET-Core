using Microsoft.EntityFrameworkCore;
using Shipping.Data;
using Shipping.Models.Enums;
using Shipping.ViewModels;

namespace Shipping.Repositories.Dashboard
{
	public class DashboardService : IDashboardService
	{
		private readonly ApplicationDbContext _context;

		public DashboardService(ApplicationDbContext context)
		{
			_context = context;
		}

		public async Task<DashboardViewModel> GetDashboardData()
		{
			var today = DateTime.Today;
			var sevenDaysAgo = today.AddDays(-6);

			var vm = new DashboardViewModel();

			vm.DonHangMoiHomNay = await _context.DonHangs
				.CountAsync(d => d.NgayTao >= today);

			vm.DoanhThuHomNay = await _context.DonHangs
				.Where(d => d.NgayTao >= today && (d.TrangThaiDH == TrangThaiDonHang.DaGiao || d.TrangThaiTT == TrangThaiThanhToan.DaThanhToan))
				.SumAsync(d => d.ThanhTien);

			vm.DonHangChoXuLy = await _context.DonHangs
				.CountAsync(d => d.TrangThaiDH == TrangThaiDonHang.ChoXuLy);

			vm.ShipperDangBan = await _context.ChuyenHangs
				.Where(c => c.TrangThai == TrangThaiDonHang.DangGiao)
				.Select(c => c.ShipperId)
				.Distinct()
				.CountAsync();

			var rawData = await _context.DonHangs
				.Where(d => d.NgayTao >= sevenDaysAgo)
				.GroupBy(d => d.NgayTao.Date)
				.Select(g => new { Date = g.Key, Count = g.Count(), Revenue = g.Sum(x => x.ThanhTien) })
				.ToListAsync();

			for (int i = 0; i < 7; i++)
			{
				var date = sevenDaysAgo.AddDays(i);
				var data = rawData.FirstOrDefault(r => r.Date == date);

				vm.LabelsNgay.Add(date.ToString("dd/MM"));
				vm.DataDonHang.Add(data?.Count ?? 0);
				vm.DataDoanhThu.Add(data?.Revenue ?? 0);
			}

			vm.CountDaGiao = await _context.DonHangs.CountAsync(d => d.TrangThaiDH == TrangThaiDonHang.DaGiao);
			vm.CountDangGiao = await _context.DonHangs.CountAsync(d => d.TrangThaiDH == TrangThaiDonHang.DangGiao);
			vm.CountHuy = await _context.DonHangs.CountAsync(d => d.TrangThaiDH == TrangThaiDonHang.ThatBai);

			vm.DonHangMoiNhat = await _context.DonHangs
				.Include(d => d.LoaiDichVu)
				.OrderByDescending(d => d.NgayTao)
				.Take(5)
				.ToListAsync();

			return vm;
		}
	}
}
