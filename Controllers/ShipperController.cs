using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Shipping.Data;
using Shipping.Models;
using Shipping.Models.Enums;
using Shipping.Repositories.CrudService;
using Shipping.Repositories.DonHangService;
using Shipping.Repositories.GeoService;
using Shipping.Repositories.UserService;
using Shipping.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Shipping.Controllers
{
    public class ShipperController : Controller
    {
		private readonly UserManager<IdentityUser> _userManager;
		private readonly IShipperService _shipperRepo;
		private readonly IChuyenHangService _chuyenHangRepo;
		private readonly ICrudService<ChiNhanh> _chiNhanhRepo;
		private readonly IGeoService<TinhThanh> _tinhThanhRepo;
		private readonly IGeoService<PhuongXa> _phuongXaRepo;

		public ShipperController(UserManager<IdentityUser> userManager,
								 IShipperService shipperRepo,
								 IChuyenHangService chuyenHangRepo,
								 ICrudService<ChiNhanh> chiNhanhRepo,
								 IGeoService<TinhThanh> tinhThanhRepo,
								 IGeoService<PhuongXa> phuongXaRepo)
        {
			_userManager = userManager;
			_shipperRepo = shipperRepo;
			_chuyenHangRepo = chuyenHangRepo;
			_chiNhanhRepo = chiNhanhRepo;
			_tinhThanhRepo = tinhThanhRepo;
			_phuongXaRepo = phuongXaRepo;
		}

		public void GetData()
		{
			ViewBag.TinhThanh = _tinhThanhRepo.GetAll();
			ViewBag.PhuongXa = _phuongXaRepo.GetAll();
			ViewBag.ChiNhanh = _chiNhanhRepo.GetAll();
		}

		// GET: Shipper
		public async Task<IActionResult> Index()
        {
			GetData();
			var shipper = _shipperRepo.GetAllActive();
			return View(shipper);
		}

        // GET: Shipper/Details/5
        public async Task<IActionResult> Details(int? id)
        {
			if (id == null)
			{
				return BadRequest();
			}
			var shipper = await _shipperRepo.GetById(id);
			if (shipper == null)
				return NotFound();

			var model = new ShipperViewModel()
			{
				Id = shipper.Id,
				UserId = shipper.UserId,
				Email = shipper.User.Email,
				Ten = shipper.Ten,
				SDT = shipper.User.PhoneNumber,
				CCCD = shipper.CCCD,
				NgaySinh = shipper.NgaySinh,
				GioiTinh = shipper.GioiTinh,
				TinhThanhId = shipper.TinhThanhId,
				PhuongXaId = shipper.PhuongXaId,
				DiaChi = shipper.DiaChi,
				ChiNhanhId = shipper.ChiNhanhId,
				LoaiShipper = shipper.LoaiShipper,
				BienSoXe = shipper.BienSoXe,
				HinhAnh = shipper.HinhAnh,
				IsDeleted = shipper.IsDeleted
			};

			GetData();
			return View(model);
		}

        // GET: Shipper/Create
        public IActionResult Create()
        {
			GetData();
			return View(new ShipperViewModel());
		}

        // POST: Shipper/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(ShipperViewModel shipper)
        {
			if (!ModelState.IsValid)
			{
				GetData();
				return View(shipper);
			}
			var result = await _shipperRepo.Create(shipper);
			if (!result.Completed)
			{
				ModelState.AddModelError(String.Empty, result.Message);
				GetData();
				return View(shipper);
			}
			return RedirectToAction(nameof(Index));
        }

        // GET: Shipper/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
			if (id == null)
			{
				return BadRequest();
			}
			var shipper = await _shipperRepo.GetById(id);
			if (shipper == null)
				return NotFound();
			var model = new ShipperViewModel()
			{
				Id = shipper.Id,
				UserId = shipper.UserId,
				Email = shipper.User.Email,
				Ten = shipper.Ten,
				SDT = shipper.User.PhoneNumber,
				CCCD = shipper.CCCD,
				NgaySinh = shipper.NgaySinh,
				GioiTinh = shipper.GioiTinh,
				TinhThanhId = shipper.TinhThanhId,
				PhuongXaId = shipper.PhuongXaId,
				DiaChi = shipper.DiaChi,
				ChiNhanhId = shipper.ChiNhanhId,
				LoaiShipper = shipper.LoaiShipper,
				BienSoXe = shipper.BienSoXe,
				HinhAnh = shipper.HinhAnh,
				IsDeleted = shipper.IsDeleted
			};
			GetData();
			return View(model);
        }

        // POST: Shipper/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(ShipperViewModel shipper)
        {
			if (!ModelState.IsValid)
			{
				GetData();
				return View(shipper);
			}

			var result = await _shipperRepo.Edit(shipper);
			if (!result.Completed)
			{
				ModelState.AddModelError(String.Empty, result.Message);
				GetData();
				return View(shipper);
			}
			return RedirectToAction(nameof(Index));
		}

        // GET: Shipper/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
			if (id == null)
			{
				return BadRequest();
			}

			var result = await _shipperRepo.Delete(id);
			if (!result.Completed)
				Console.WriteLine(result.Message);
			return RedirectToAction(nameof(Index));
		}

		[HttpGet]
		public IActionResult GetPhuongXaByTinhThanh(string TinhThanhId)
		{
			var phuongXaList = _phuongXaRepo.GetAll()
									   .Where(px => px.TinhThanhId == TinhThanhId)
									   .Select(px => new {
										   value = px.Id,
										   text = px.TenPhuongXa
									   })
									   .ToList();

			return Json(phuongXaList);
		}



		[HttpPost]
		public async Task<IActionResult> UpdateState(string viTri, int trangThai)
		{
			var userId = _userManager.GetUserId(User);
			var result = await _shipperRepo.UpdateState(userId, viTri, trangThai);

			return Json(new { success = result.Completed, msg = result.Message });
		}

		[HttpGet]
		[Authorize(Roles = "Admin, NhanVien")]
		public async Task<IActionResult> GetAvailableShippers(string locationId)
		{
			var shippers = await _shipperRepo.GetAvailableShippers(locationId);
			return Json(shippers);
		}

		[Authorize(Roles = "Shipper")] // Chỉ Shipper mới vào được đây
		public async Task<IActionResult> Dashboard()
		{
			var userId = _userManager.GetUserId(User);
			var shipper = await _shipperRepo.GetByUserId(userId);

			if (shipper == null) return RedirectToAction("AccessDenied", "Account", new { area = "Identity" });

			ViewBag.ListTinhThanh = new SelectList(_tinhThanhRepo.GetAll(), "Id", "TenTinhThanh", shipper.ViTri); 

			return View(shipper);
		}

		[HttpGet]
		[Authorize(Roles = "Admin,QuanLy,NhanVien")]
		public async Task<IActionResult> ResolveLocation(int type, string? dataId, string? orderId)
		{
			// type: 3 (Chi nhánh), 4 (Tiếp nối)
			string? locationId = null;

			// Type 3: Chi nhánh
			if (type == 3 && int.TryParse(dataId, out int cnId))
			{
				var chiNhanh = await _chiNhanhRepo.GetById(cnId);
				locationId = chiNhanh?.TinhThanhId;
			}
			// Type 4: Tiếp nối
			else if (type == 4 && !string.IsNullOrEmpty(orderId))
			{
				locationId = await _chuyenHangRepo.GetLastShipmentLocation(orderId);
			}

			return Json(new { locationId = locationId });
		}

	}
}
