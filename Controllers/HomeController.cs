using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Shipping.Repositories.Dashboard;

namespace Shipping.Controllers
{
	public class HomeController : Controller
	{
		private readonly IDashboardService _dashboardService;

		public HomeController(IDashboardService dashboardService)
		{
			_dashboardService = dashboardService;
		}

		public async Task<IActionResult> Index()
		{
			if (User.IsInRole("Shipper"))
			{
				return RedirectToAction("Dashboard", "Shipper");
			}
			if (User.IsInRole("KhachHang"))
			{
				return RedirectToAction("MyOrders", "DonHang");
			}

			var vm = await _dashboardService.GetDashboardData();
			return View(vm);
		}
	}
}
