using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Shipping.Data;
using Shipping.Models;
using Shipping.Repositories.CrudService;
using Shipping.Repositories.GeoService;
using Shipping.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Shipping.Controllers
{
    public class ChiNhanhController : Controller
    {
		private readonly ICrudService<ChiNhanh> chiNhanhRepo;
		private readonly IGeoService<TinhThanh> tinhThanhRepo;
		private readonly IGeoService<PhuongXa> phuongXaRepo;

		public ChiNhanhController(ICrudService<ChiNhanh> _chiNhanhRepo,
								  IGeoService<TinhThanh> _tinhThanhRepo,
								  IGeoService<PhuongXa> _phuongXaRepo)
		{
			chiNhanhRepo = _chiNhanhRepo;
			tinhThanhRepo = _tinhThanhRepo;
			phuongXaRepo = _phuongXaRepo;
		}

		public void GetData()
		{
			ViewBag.TinhThanh = tinhThanhRepo.GetAll();
			ViewBag.PhuongXa = phuongXaRepo.GetAll();
		}

		// GET: ChiNhanh
		public IActionResult Index()
		{
			GetData();
			var chiNhanh = chiNhanhRepo.GetAll();
			return View(chiNhanh);
		}

		// GET: ChiNhanh/Details/5
		public async Task<IActionResult> Details(int? id)
		{
			if (id == null)
			{
				return NotFound();
			}

			var chiNhanh = await chiNhanhRepo.GetById(id);
			if (chiNhanh == null)
			{
				return NotFound();
			}

			return View(chiNhanh);
		}

		// GET: ChiNhanh/Create
		public IActionResult Create()
		{
			GetData();
			return View(new ChiNhanh());
		}

		// POST: ChiNhanh/Create
		// To protect from overposting attacks, enable the specific properties you want to bind to.
		// For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
		[HttpPost]
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> Create([Bind("Id,TenChiNhanh,DiaChi,SDT,TinhThanhId,PhuongXaId")] ChiNhanh chiNhanh)
		{
			if (!ModelState.IsValid)
			{
				GetData();
				return View(chiNhanh);
			}
			ReturnState result = await chiNhanhRepo.Create(chiNhanh);
			if (!result.Completed)
			{
				ModelState.AddModelError(String.Empty, result.Message);
				GetData();
				return View(chiNhanh);
			}
			return RedirectToAction(nameof(Index));
		}

		// GET: ChiNhanh/Edit/5
		public async Task<IActionResult> Edit(int? id)
		{
			if (id == null)
			{
				return NotFound();
			}

			var chiNhanh = await chiNhanhRepo.GetById(id);
			if (chiNhanh == null)
			{
				return NotFound();
			}
			GetData();
			return View(chiNhanh);
		}

		// POST: ChiNhanh/Edit/5
		// To protect from overposting attacks, enable the specific properties you want to bind to.
		// For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
		[HttpPost]
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> Edit(int id, [Bind("Id,TenChiNhanh,DiaChi,SDT,TinhThanhId,PhuongXaId")] ChiNhanh chiNhanh)
		{
			if (id != chiNhanh.Id)
			{
				return NotFound();
			}

			if (!ModelState.IsValid)
			{
				GetData();
				return View(chiNhanh);
			}
			ReturnState result = await chiNhanhRepo.Edit(chiNhanh);
			if (!result.Completed)
			{
				ModelState.AddModelError(String.Empty, result.Message);
				GetData();
				return View(chiNhanh);
			}
			return RedirectToAction(nameof(Index));
		}

		// GET: ChiNhanh/Delete/5
		public async Task<IActionResult> Delete(int? id)
		{
			if (id == null)
			{
				return BadRequest();
			}


			ReturnState result = await chiNhanhRepo.Delete(id);
			if (!result.Completed)
			{
				return NotFound();
			}
			return RedirectToAction(nameof(Index));
		}

		[HttpGet]
		public IActionResult GetPhuongXaByTinhThanh(string TinhThanhId)
		{
			var phuongXaList = phuongXaRepo.GetAll()
									   .Where(px => px.TinhThanhId == TinhThanhId)
									   .Select(px => new {
										   value = px.Id,
										   text = px.TenPhuongXa
									   })
									   .ToList();

			return Json(phuongXaList);
		}
	}
}
