using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Shipping.Models;
using Shipping.Repositories.CrudService;
using Shipping.Repositories.GeoService;
using Shipping.Repositories.UserService;
using Shipping.ViewModels;


namespace Shipping.Controllers
{
    public class NhanVienController : Controller
    {
		private readonly IUserService<NhanVien, NhanVienViewModel> _nhanVienRepo;
		private readonly ICrudService<ChiNhanh> _chiNhanhRepo;
		private readonly IGeoService<TinhThanh> _tinhThanhRepo;
		private readonly IGeoService<PhuongXa> _phuongXaRepo;

		public NhanVienController(
			IUserService<NhanVien, NhanVienViewModel> nhanVienRepo,
			ICrudService<ChiNhanh> chiNhanhRepo,
			IGeoService<TinhThanh> tinhThanhRepo,
			IGeoService<PhuongXa> phuongXaRepo)
		{
			_nhanVienRepo = nhanVienRepo;
			_chiNhanhRepo = chiNhanhRepo;
			_tinhThanhRepo = tinhThanhRepo;
			_phuongXaRepo = phuongXaRepo;
		}

        public void GetData(NhanVienViewModel? model = null)
        {
			ViewBag.TinhThanh = _tinhThanhRepo.GetAll();
			ViewBag.PhuongXa = _phuongXaRepo.GetAll();
			ViewBag.ChiNhanh = _chiNhanhRepo.GetAll();
		}

		// GET: NhanVien
		public IActionResult Index()
        {
			var nhanViens = _nhanVienRepo.GetAll();
			return View(nhanViens);
		}

        // GET: NhanVien/Details/5
        public async Task<IActionResult> Details(int? id)
        {
			if (id == null)
			{
				return NotFound();
			}

			var nhanVien = await _nhanVienRepo.GetById(id);
			if (nhanVien == null)
			{
				return NotFound();
			}
			GetData();
			return View(nhanVien);
		}

        // GET: NhanVien/Create
        public IActionResult Create()
        {
			GetData();
			return View(new NhanVienViewModel());
		}

        // POST: NhanVien/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(NhanVienViewModel nhanVienVM)
        {
			if (ModelState.IsValid)
			{
				var result = await _nhanVienRepo.Create(nhanVienVM);
				if (result.Completed)
				{
					return RedirectToAction(nameof(Index));
				}
				ModelState.AddModelError(string.Empty, result.Message);
			}
			GetData(nhanVienVM);
			return View(nhanVienVM);
		}

        // GET: NhanVien/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
			if (id == null)
			{
				return NotFound();
			}

			var nhanVien = await _nhanVienRepo.GetById(id);
			if (nhanVien == null)
			{
				return NotFound();
			}
			var nhanVienVM = new NhanVienViewModel
			{
				Id = nhanVien.Id,
				UserId = nhanVien.UserId,
				Email = nhanVien.User.Email,
				Ten = nhanVien.Ten,
				CCCD = nhanVien.CCCD,
				SDT = nhanVien.User.PhoneNumber,
				GioiTinh = nhanVien.GioiTinh,
				NgaySinh = nhanVien.NgaySinh,
				DiaChi = nhanVien.DiaChi,
				TinhThanhId = nhanVien.TinhThanhId,
				PhuongXaId = nhanVien.PhuongXaId,
				IsQuanLy = nhanVien.IsQuanLy,
				ChiNhanhId = nhanVien.ChiNhanhId,
				HinhAnh = nhanVien.HinhAnh,
				IsDeleted = nhanVien.IsDeleted
			};
			GetData(nhanVienVM);
            return View(nhanVienVM);
        }

        // POST: NhanVien/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(NhanVienViewModel nhanVienVM)
        {
			if (ModelState.IsValid)
			{
				var result = await _nhanVienRepo.Edit(nhanVienVM);
				if (result.Completed)
				{
					return RedirectToAction(nameof(Index));
				}
				ModelState.AddModelError(string.Empty, result.Message);
			}

			GetData(nhanVienVM);
			return View(nhanVienVM);
		}

        // POST: NhanVien/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
			var result = await _nhanVienRepo.Delete(id);
			if (!result.Completed)
			{
				Console.WriteLine($"Lỗi xóa nhân viên: {result.Message}");
			}
			return RedirectToAction(nameof(Index));
		}

		[HttpGet]
		public IActionResult GetPhuongXaByTinhThanh(string TinhThanhId)
		{
			if (string.IsNullOrEmpty(TinhThanhId))
			{
				return Json(new List<SelectListItem>());
			}

			var phuongXaList = _phuongXaRepo.GetAll()
											.Where(p => p.TinhThanhId == TinhThanhId)
											.Select(p => new
											{
												Value = p.Id,
												Text = p.TenPhuongXa
											})
											.ToList();

			return Json(phuongXaList);
		}
	}
}
