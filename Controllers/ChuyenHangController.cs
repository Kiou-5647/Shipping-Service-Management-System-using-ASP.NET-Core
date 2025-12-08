using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Shipping.Data;
using Shipping.Models;
using Shipping.Models.Enums;
using Shipping.Repositories.DonHangService;
using Shipping.Repositories.UserService;
using Shipping.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Shipping.Controllers
{
    public class ChuyenHangController : Controller
    {
		private readonly IChuyenHangService _chuyenHangService;
		private readonly UserManager<IdentityUser> _userManager;
		private readonly IUserService<Shipper, ShipperViewModel> _shipperRepo;

		public ChuyenHangController(
			IChuyenHangService chuyenHangService,
			UserManager<IdentityUser> userManager,
			IUserService<Shipper, ShipperViewModel> shipperRepo)
		{
			_chuyenHangService = chuyenHangService;
			_userManager = userManager;
			_shipperRepo = shipperRepo;
		}

		[Authorize(Roles = "Shipper")]
		public async Task<IActionResult> MyShipments()
		{
			var userId = _userManager.GetUserId(User);
			if (userId == null) return RedirectToPage("/Account/Login", new { area = "Identity" });

			var allShippers = _shipperRepo.GetAll();
			var currentShipper = allShippers.FirstOrDefault(s => s.UserId == userId);

			if (currentShipper == null)
			{
				if (User.IsInRole("Admin") || User.IsInRole("QuanLy"))
				{
					return View("Error", new { message = "Giao diện này dành riêng cho tài khoản Shipper." });
				}
				return RedirectToAction("Index", "Home");
			}

			var tasks = await _chuyenHangService.GetByShipperId(currentShipper.Id);

			return View(tasks);
		}

		[HttpPost]
		[ValidateAntiForgeryToken]
		[Authorize(Roles = "Admin, NhanVien, Shipper")]
		public async Task<IActionResult> UpdateStatus(string donHangId, int thuTu, TrangThaiDonHang trangThai, string ghiChu, IFormFile? evidenceFile, string returnUrl)
		{
			try
			{
				var result = await _chuyenHangService.UpdateStatus(donHangId, thuTu, trangThai, ghiChu, evidenceFile);

				if (result.Completed)
					TempData["Success"] = "Cập nhật thành công!";
				else
					TempData["Error"] = result.Message;
			}
			catch (Exception ex)
			{
				TempData["Error"] = "Lỗi hệ thống: " + ex.Message;
			}

			if (!string.IsNullOrEmpty(returnUrl)) return Redirect(returnUrl);
			return RedirectToAction(nameof(MyShipments));
		}

		[HttpPost]
		[Authorize(Roles = "Admin, NhanVien")]
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> Delete(string donHangId, int thuTu)
		{
			var result = await _chuyenHangService.Delete(donHangId, thuTu);

			if (result.Completed) TempData["Success"] = result.Message;
			else TempData["Error"] = result.Message;

			return RedirectToAction("Details", "DonHang", new { id = donHangId });
		}
	}
}
