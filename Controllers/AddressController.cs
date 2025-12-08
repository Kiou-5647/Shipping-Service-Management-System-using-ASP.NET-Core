using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Shipping.Models;
using Shipping.Repositories.GeoService;

namespace Shipping.Controllers
{
	public class AddressController : Controller
	{
		private readonly IGeoService<PhuongXa> _phuongXaRepo;

		public AddressController(IGeoService<PhuongXa> phuongXaService)
		{
			_phuongXaRepo = phuongXaService;
		}

		[HttpGet]
		public IActionResult GetPhuongXaByTinhThanh(string tinhThanhId)
		{
			Console.WriteLine("Address Controller Woring!");
			if (string.IsNullOrEmpty(tinhThanhId))
			{
				return Json(new List<SelectListItem>());
			}

			var phuongXaList = _phuongXaRepo.GetAll()
											.Where(p => p.TinhThanhId == tinhThanhId)
											.Select(p => new
											{
												id = p.Id,
												name = p.TenPhuongXa
											})
											.ToList();

			return Json(phuongXaList);
		}
	}
}
