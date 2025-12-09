using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Shipping.Data;
using Shipping.Models;
using Shipping.Models.Enums;
using Shipping.Repositories.ConfigService;
using Shipping.Repositories.CrudService;
using Shipping.Repositories.DonHangService;
using Shipping.Repositories.GeoService;
using Shipping.Repositories.UserService;
using Shipping.Utilities;
using Shipping.ViewModels;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace Shipping.Controllers
{
    public class DonHangController : Controller
    {
		private readonly IDonHangService _donHangRepo;
		private readonly UserManager<IdentityUser> _userManager;
		private readonly IGeoService<TinhThanh> _tinhThanhRepo;
		private readonly IGeoService<PhuongXa> _phuongXaRepo;
		private readonly ICrudService<ChiNhanh> _chiNhanhRepo;
		private readonly IUserService<NhanVien, NhanVienViewModel> _nhanVienRepo;
		private readonly IUserService<KhachHang, KhachHangViewModel> _khachHangRepo;
		private readonly IUserService<Shipper, ShipperViewModel> _shipperRepo;
		private readonly ICrudService<LoaiDichVu> _loaiDichVuRepo;
		private readonly IChuyenHangService _chuyenHangService;
		private readonly IConfigService _configService;

		public DonHangController(
			IDonHangService donHangRepo,
			UserManager<IdentityUser> userManager,
			IGeoService<TinhThanh> tinhThanhRepo,
			IGeoService<PhuongXa> phuongXaRepo,
			ICrudService<ChiNhanh> chiNhanhRepo,
			IUserService<NhanVien, NhanVienViewModel> nhanVienRepo,
			IUserService<KhachHang, KhachHangViewModel> khachHangRepo,
			IUserService<Shipper, ShipperViewModel> shipperRepo,
			ICrudService<LoaiDichVu> loaiDichVuRepo,
			IChuyenHangService chuyenHangService,
			IConfigService configService)
		{
			_donHangRepo = donHangRepo;
			_userManager = userManager;
			_tinhThanhRepo = tinhThanhRepo;
			_phuongXaRepo = phuongXaRepo;
			_chiNhanhRepo = chiNhanhRepo;
			_nhanVienRepo = nhanVienRepo;
			_khachHangRepo = khachHangRepo;
			_shipperRepo = shipperRepo;
			_loaiDichVuRepo = loaiDichVuRepo;
			_chuyenHangService = chuyenHangService;
			_configService = configService;
		}

		private void GetData()
		{
			ViewBag.TinhThanh = _tinhThanhRepo.GetAll();
			ViewBag.LoaiDichVu = _loaiDichVuRepo.GetAll();
			ViewBag.PhuongXa = _phuongXaRepo.GetAll();

			var insConfig = _configService.GetAll().FirstOrDefault(c => c.Id == "INSURANCE_THRESHOLD");

			ViewBag.InsuranceThreshold = insConfig?.GiaTri ?? 5000000;
		}

		public IActionResult Index()
		{
			var list = _donHangRepo.GetAll();
			return View(list);
		}

		public async Task<IActionResult> Details(string id)
		{
			if (string.IsNullOrEmpty(id)) return NotFound();

			var entity = await _donHangRepo.GetById(id);

			if (entity == null) return NotFound();

			ViewBag.ListChuyenHang = await _chuyenHangService.GetByDonHangId(id);
			LoadShipmentData();
			return View(entity);
		}

		public async Task<IActionResult> Create()
		{
			var model = new DonHangViewModel();

			var userId = _userManager.GetUserId(User);
			if (!string.IsNullOrEmpty(userId))
			{
				var allKhach = _khachHangRepo.GetAll();
				var khachHang = allKhach.FirstOrDefault(k => k.UserId == userId);

				if (khachHang != null)
				{
					model.KhachHangId = khachHang.Id;

					var userIdentity = await _userManager.FindByIdAsync(userId);
					string sdt = userIdentity?.PhoneNumber ?? "";

					model.TTNguoiGui = $"{khachHang.Ten} - {sdt}";
					model.DiaChiGui = khachHang.DiaChi;
					model.TinhGuiId = khachHang.TinhThanhId;
					model.PhuongGuiId = khachHang.PhuongXaId;
				}
			}

			GetData();
			return View(model);
		}

		[HttpPost]
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> Create(DonHangViewModel model)
		{
			var userId = _userManager.GetUserId(User);
			if (!string.IsNullOrEmpty(userId))
			{
				var allKhach = _khachHangRepo.GetAll();
				var khachHang = allKhach.FirstOrDefault(k => k.UserId == userId);
				if (khachHang != null)
				{
					model.KhachHangId = khachHang.Id;
				}
			}

			var validate = await _donHangRepo.ValidateOrderConstraints(model);
			if (!validate.Completed)
			{
				ModelState.AddModelError(string.Empty, validate.Message);
				GetData();
				return View(model);
			}

			if (ModelState.IsValid)
			{
				var result = await _donHangRepo.Create(model);
				if (result.Completed)
				{
					TempData["Success"] = result.Message;
					if (User.IsInRole("Admin") || User.IsInRole("NhanVien"))
					{
						return RedirectToAction(nameof(Index));
					}

					return RedirectToAction(nameof(MyOrders));

				}
				ModelState.AddModelError(string.Empty, result.Message);
			}

			GetData();
			return View(model);
		}

		[HttpPost]
		public async Task<IActionResult> CalculateFeeApi([FromBody] DonHangViewModel model)
		{
			var result = await _donHangRepo.CalculateShippingFee(model);
			return Json(result);
		}

		[HttpPost]
		public async Task<IActionResult> ConfirmPayment(string id, IFormFile file)
		{
			var userId = _userManager.GetUserId(User);

			var allNhanViens = _nhanVienRepo.GetAll();
			var nhanVien = allNhanViens.FirstOrDefault(nv => nv.UserId == userId);

			var allShippers = _shipperRepo.GetAll();
			var shipper = allShippers.FirstOrDefault(s => s.UserId == userId);

			int? nvId = nhanVien?.Id;
			int? shipperId = shipper?.Id;

			string? paymentImgPath = null;
			if (file != null)
			{
				string folderName = Path.Combine("donhang", id);
				paymentImgPath = Image.Upload(file, folderName);
			}

			var result = await _donHangRepo.UpdatePaymentStatus(id, true, paymentImage: paymentImgPath, nhanVienId: nvId, shipperId: shipperId);

			if (result.Completed) TempData["Success"] = result.Message;
			else TempData["Error"] = result.Message;

			return RedirectToAction(nameof(Details), new { id });
		}

		[HttpPost]
		public async Task<IActionResult> CancelOrder(string id, string lyDo)
		{
			var result = await _donHangRepo.Cancel(id, lyDo);
			if (result.Completed) TempData["Success"] = result.Message;
			else TempData["Error"] = result.Message;

			return RedirectToAction(nameof(Details), new { id });
		}

		[HttpPost]
		public async Task<IActionResult> DeleteOrder(string id)
		{
			var result = await _donHangRepo.Delete(id);
			if (result.Completed) TempData["Success"] = result.Message;
			else TempData["Error"] = result.Message;

			return RedirectToAction(nameof(Index));
		}


		[HttpPost]
		public async Task<IActionResult> AddShipment(ChuyenHangViewModel model)
		{
			if (model.ShipperId > 0 && !string.IsNullOrEmpty(model.DonHangId))
			{
				var result = await _chuyenHangService.PhanCongChuyenHang(model);

				if (result.Completed)
					TempData["Success"] = result.Message;
				else
					TempData["Error"] = result.Message;
			}
			else
			{
				TempData["Error"] = "Vui lòng kiểm tra lại thông tin Shipper hoặc Đơn hàng.";
			}

			return RedirectToAction("Details", new { id = model.DonHangId });
		}

		private void LoadShipmentData()
		{
			var shippers = _shipperRepo.GetAll();

			var shipperGH = shippers.Where(s => s.LoaiShipper == LoaiShipper.GiaoHang);
			var shipperVC = shippers.Where(s => s.LoaiShipper == LoaiShipper.VanChuyen);
			ViewBag.ShipperGHs = new SelectList(shipperGH, "Id", "Ten");
			ViewBag.ShipperVCs = new SelectList(shipperVC, "Id", "Ten");
			ViewBag.ChiNhanhs = new SelectList(_chiNhanhRepo.GetAll(), "Id", "TenChiNhanh");
			ViewBag.TinhThanh = _tinhThanhRepo.GetAll();
		}

		public async Task<IActionResult> MyOrders()
		{
			var userId = _userManager.GetUserId(User);
			if (userId == null) return RedirectToPage("/Account/Login", new { area = "Identity" });

			var allKhach = _khachHangRepo.GetAll();
			var khachHang = allKhach.FirstOrDefault(k => k.UserId == userId);

			if (khachHang == null)
			{
				return View(new List<DonHang>());
			}

			var listDonHang = await _donHangRepo.GetByKhachHangId(khachHang.Id);

			return View(listDonHang);
		}

	}
}
