using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Shipping.Data;
using Shipping.Models;
using Shipping.Repositories.GeoService;
using Shipping.Repositories.UserService;
using Shipping.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Shipping.Controllers
{
	[Authorize(Roles = "Admin, NhanVien")]
    public class KhachHangController : Controller
    {
		private readonly IUserService<KhachHang, KhachHangViewModel> _khachHangRepo;
		private readonly IGeoService<TinhThanh> _tinhThanhRepo;
		private readonly IGeoService<PhuongXa> _phuongXaRepo;

		public KhachHangController(IUserService<KhachHang, KhachHangViewModel> khachHangRepo,
								   IGeoService<TinhThanh> tinhThanhRepo,
								   IGeoService<PhuongXa> phuongXaRepo)
		{
			_khachHangRepo = khachHangRepo;
			_tinhThanhRepo = tinhThanhRepo;
			_phuongXaRepo = phuongXaRepo;
		}

		public void GetData()
		{
			ViewBag.TinhThanh = _tinhThanhRepo.GetAll();
			ViewBag.PhuongXa = _phuongXaRepo.GetAll();
		}

		// GET: KhachHang
		public IActionResult Index()
		{
			GetData();
			var khachHang = _khachHangRepo.GetAll();
			return View(khachHang);
		}

		// GET: KhachHang/Details/5
		public async Task<IActionResult> Details(int? id)
		{
			if (id == null)
			{
				return BadRequest();
			}
			var khachHang = await _khachHangRepo.GetById(id);
			if (khachHang == null)
				return NotFound();

			var model = new KhachHangViewModel()
			{
				Id = khachHang.Id,
				UserId = khachHang.UserId,
				Email = khachHang.User.Email,
				Ten = khachHang.Ten,
				SDT = khachHang.User.PhoneNumber,
				CCCD = khachHang.CCCD,
				TinhThanhId = khachHang.TinhThanhId,
				PhuongXaId = khachHang.PhuongXaId,
				DiaChi = khachHang.DiaChi,
				MaSoThue = khachHang.MaSoThue,
				LoaiKhachHang = khachHang.LoaiKhachHang,
				IsDeleted = khachHang.IsDeleted,
			};

			GetData();
			return View(model);
		}
	}
}
