using Microsoft.AspNetCore.Mvc;
using Shipping.Repositories.DonHangService;

namespace Shipping.Controllers
{
	public class TrackingController : Controller
	{
		private readonly IDonHangService _donHangService;
		private readonly IChuyenHangService _chuyenHangService;

		public TrackingController(IDonHangService donHangService, IChuyenHangService chuyenHangService)
		{
			_donHangService = donHangService;
			_chuyenHangService = chuyenHangService;
		}

		public async Task<IActionResult> Index(string? searchId)
		{
			if (string.IsNullOrEmpty(searchId))
			{
				return View();
			}
			var donHang = await _donHangService.GetById(searchId.Trim());

			if (donHang == null)
			{
				ViewBag.Error = $"Không tìm thấy vận đơn có mã: {searchId}";
				return View();
			}

			ViewBag.History = await _chuyenHangService.GetByDonHangId(searchId);

			return View(donHang);
		}
	}
}
