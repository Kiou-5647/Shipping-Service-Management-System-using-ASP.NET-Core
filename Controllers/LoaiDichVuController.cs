using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Shipping.Models;
using Shipping.Repositories.ConfigService;
using Shipping.Repositories.CrudService;


namespace Shipping.Controllers
{
	[Authorize(Roles = "Admin")]
	public class LoaiDichVuController : Controller
    {
		private readonly ICrudService<LoaiDichVu> _loaiDichVuRepo;
		private readonly ICrudService<CauTrucGiaCuoc> _giaCuocRepo;
		private readonly IConfigService _configRepo;

		public LoaiDichVuController(ICrudService<LoaiDichVu> loaiDichVuRepo,
									ICrudService<CauTrucGiaCuoc> giaCuocRepo,
									IConfigService configRepo)
		{
			_loaiDichVuRepo = loaiDichVuRepo;
			_giaCuocRepo = giaCuocRepo;
			_configRepo = configRepo;
		}

		// GET: LoaiDichVu
		[AllowAnonymous]
		public IActionResult Index()
		{
			var dv = _loaiDichVuRepo.GetAll();
			return View(dv);
		}

		[AllowAnonymous]
		public async Task<IActionResult> Details(int? id)
		{
			var dv = await _loaiDichVuRepo.GetById(id);
			if (dv == null)
				return NotFound();

			ViewBag.Config = _configRepo.GetAll();
			return View(dv);
		}

		// GET: LoaiDichVu/Edit/5
		public async Task<IActionResult> Edit(int? id)
		{
			if (id == null)
			{
				return NotFound();
			}

			var loaiDichVu = await _loaiDichVuRepo.GetById(id);

			if (loaiDichVu == null)
			{
				return NotFound();
			}

			ViewBag.Config = _configRepo.GetAll();
			return View(loaiDichVu);
		}

		// POST: LoaiDichVu/Edit/5
		[HttpPost]
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> Edit(int id, [Bind("Id,TenDichVu,MaDV,MoTa,DonViTangThem,MocCanNang,ThoiGianToiThieu,ThoiGianToiDa")] LoaiDichVu loaiDichVu)
		{
			if (id != loaiDichVu.Id)
			{
				return NotFound();
			}

			if (ModelState.IsValid)
			{
				var successDv = await _loaiDichVuRepo.Edit(loaiDichVu);

				if (successDv.Completed)
				{
					return RedirectToAction(nameof(Details), new {id = loaiDichVu.Id});
				}
				ModelState.AddModelError(string.Empty, successDv.Message);
			}

			ViewBag.Config = _configRepo.GetAll();
			return View(loaiDichVu);
		}

		public async Task<IActionResult> EditGiaCuoc(int? id)
		{
			if (id == null)
			{
				return NotFound();
			}
			var loaiDichVu = await _loaiDichVuRepo.GetById(id);

			if (loaiDichVu == null)
			{
				return NotFound();
			}

			ViewBag.Config = _configRepo.GetAll();
			return View(loaiDichVu);
		}

		[HttpPost]
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> EditGiaCuoc([Bind("Id,LoaiDichVuId,LoaiVungGia,GiaCoBan,GiaTangThem,GiaVuot,ThoiGianGiao")] CauTrucGiaCuoc giaCuoc)
		{
			if (giaCuoc.LoaiDichVuId == 0)
			{
				TempData["ErrorMessage"] = "Không tìm thấy dịch vụ liên quan.";
				return RedirectToAction(nameof(EditGiaCuoc), new { id = giaCuoc.LoaiDichVuId });
			}

			if (ModelState.IsValid)
			{
				var result = await _giaCuocRepo.Edit(giaCuoc);
				if (result.Completed)
				{
					TempData["SuccessMessage"] = $"Cấu trúc giá cước đã được cập nhật thành công.";
				}
				else
				{
					TempData["ErrorMessage"] = $"Cập nhật giá cước thất bại.";
				}
			}
			else
			{
				TempData["ErrorMessage"] = $"Cập nhật giá cước cho vùng {giaCuoc.LoaiVungGia.ToString()} thất bại do lỗi dữ liệu. Vui lòng kiểm tra lại.";
			}

			return RedirectToAction(nameof(EditGiaCuoc), new { id = giaCuoc.LoaiDichVuId });
		}
	}
}
