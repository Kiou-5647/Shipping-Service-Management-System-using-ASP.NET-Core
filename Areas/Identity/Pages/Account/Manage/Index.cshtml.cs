// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
#nullable disable

using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Shipping.Models;
using Shipping.Models.Enums;
using Shipping.Repositories.GeoService;
using Shipping.Repositories.UserService;
using Shipping.ViewModels;
using System;
using System.ComponentModel.DataAnnotations;
using System.Text.Encodings.Web;
using System.Threading.Tasks;

namespace Shipping.Areas.Identity.Pages.Account.Manage
{
    public class IndexModel : PageModel
    {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly SignInManager<IdentityUser> _signInManager;

		private readonly IUserService<KhachHang, KhachHangViewModel> _khachHangService;
		private readonly IUserService<Shipper, ShipperViewModel> _shipperService;
		private readonly IUserService<NhanVien, NhanVienViewModel> _nhanVienService;
		private readonly IGeoService<TinhThanh> _tinhThanhService;

		public IndexModel(
            UserManager<IdentityUser> userManager,
            SignInManager<IdentityUser> signInManager,
			IUserService<KhachHang, KhachHangViewModel> khachHangService,
			IUserService<Shipper, ShipperViewModel> shipperService,
			IUserService<NhanVien, NhanVienViewModel> nhanVienService,
			 IGeoService<TinhThanh> tinhThanhService)
        {
            _userManager = userManager;
            _signInManager = signInManager;
			_khachHangService = khachHangService;
			_shipperService = shipperService;
			_nhanVienService = nhanVienService;
			_tinhThanhService = tinhThanhService;
		}

        /// <summary>
        ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
        ///     directly from your code. This API may change or be removed in future releases.
        /// </summary>
        public string Username { get; set; }

        /// <summary>
        ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
        ///     directly from your code. This API may change or be removed in future releases.
        /// </summary>
        [TempData]
        public string StatusMessage { get; set; }

        /// <summary>
        ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
        ///     directly from your code. This API may change or be removed in future releases.
        /// </summary>
        [BindProperty]
        public InputModel Input { get; set; }

        /// <summary>
        ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
        ///     directly from your code. This API may change or be removed in future releases.
        /// </summary>
        public class InputModel
        {
			/// <summary>
			///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
			///     directly from your code. This API may change or be removed in future releases.
			/// </summary>
			public int Id { get; set; }

			[Required(ErrorMessage = "Vui lòng nhập Email")]
			[EmailAddress(ErrorMessage = "Email không hợp lệ")]
			[Display(Name = "Email / Tên đăng nhập")]
			[ValidateNever]
			public string Email { get; set; }

			[Required(ErrorMessage = "Vui lòng nhập họ tên")]
			[Display(Name = "Họ và tên")]
			public string Ten { get; set; }

			[Phone]
			[Display(Name = "Số điện thoại")]
			public string PhoneNumber { get; set; }

			[Display(Name = "Căn cước công dân")]
			public string CCCD { get; set; }

			[Display(Name = "Địa chỉ chi tiết")]
			public string DiaChi { get; set; }

			[Display(Name = "Tỉnh/Thành phố")]
			public string TinhThanhId { get; set; }

			[Display(Name = "Phường/Xã")]
			public string PhuongXaId { get; set; }

			[Display(Name = "Mã số thuế")]
			public string? MaSoThue { get; set; } 

			[Display(Name = "Ngày sinh")]
			[DataType(DataType.Date)]
			public DateTime? NgaySinh { get; set; }

			[Display(Name = "Giới tính")]
			public bool? GioiTinh { get; set; } 

			[Display(Name = "Biển số xe")]
			public string? BienSo { get; set; } 

			[Display(Name = "Loại hình hoạt động")]
			public LoaiShipper? LoaiShipper { get; set; }

			[Display(Name = "Ảnh đại diện")]
			public string? AvatarPath { get; set; }

			[Display(Name = "Tải ảnh mới")]
			public IFormFile? AvatarFile { get; set; }
		}

        private async Task LoadAsync(IdentityUser user)
        {
			var userName = await _userManager.GetUserNameAsync(user);
			Username = userName;

			Input = new InputModel
			{
				PhoneNumber = await _userManager.GetPhoneNumberAsync(user)
			};

			// Load dữ liệu chi tiết dựa trên Role
			var userId = user.Id;

			if (User.IsInRole("KhachHang"))
			{
				var kh = _khachHangService.GetAll().FirstOrDefault(x => x.UserId == userId);
				if (kh != null)
				{
					Input.Id = kh.Id;
					Input.Email = kh.User.Email;
					Input.Ten = kh.Ten;
					Input.CCCD = kh.CCCD;
					Input.DiaChi = kh.DiaChi;
					Input.TinhThanhId = kh.TinhThanhId;
					Input.PhuongXaId = kh?.PhuongXaId;
					Input.PhuongXaId = kh.PhuongXaId;
					Input.MaSoThue = kh.MaSoThue;
					Input.PhoneNumber = kh.User.PhoneNumber;
				}
			}
			else if (User.IsInRole("Shipper"))
			{
				var shipper = _shipperService.GetAll().FirstOrDefault(x => x.UserId == userId);
				if (shipper != null)
				{
					Input.Id = shipper.Id;
					Input.Email = shipper.User.Email;
					Input.Ten = shipper.Ten;
					Input.CCCD = shipper.CCCD;
					Input.DiaChi = shipper.DiaChi;
					Input.TinhThanhId = shipper.TinhThanhId;
					Input.PhuongXaId = shipper.PhuongXaId;
					Input.PhoneNumber = shipper.User.PhoneNumber;
					Input.NgaySinh = shipper.NgaySinh;
					Input.GioiTinh = shipper.GioiTinh;
					Input.BienSo = shipper.BienSoXe;
					Input.LoaiShipper = shipper.LoaiShipper;
					Input.AvatarPath = shipper.HinhAnh;
				}
			}
			else if (User.IsInRole("NhanVien") || User.IsInRole("Admin"))
			{
				var nv = _nhanVienService.GetAll().FirstOrDefault(x => x.UserId == userId);
				if (nv != null)
				{
					Input.Id = nv.Id;
					Input.Email = nv.User.Email;
					Input.Ten = nv.Ten;
					Input.CCCD = nv.CCCD;
					Input.DiaChi = nv.DiaChi;
					Input.TinhThanhId = nv.TinhThanhId;
					Input.PhuongXaId = nv.PhuongXaId;
					Input.PhoneNumber = nv.User.PhoneNumber;
					Input.NgaySinh = nv.NgaySinh;
					Input.GioiTinh = nv.GioiTinh;
					Input.AvatarPath = nv.HinhAnh;
				}
			}

			// Load Dropdown
			ViewData["TinhThanhList"] = new SelectList(_tinhThanhService.GetAll(), "Id", "TenTinhThanh", Input.TinhThanhId);
		}

        public async Task<IActionResult> OnGetAsync()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return NotFound($"Unable to load user with ID '{_userManager.GetUserId(User)}'.");
            }

            await LoadAsync(user);
            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
			var user = await _userManager.GetUserAsync(User);
			if (user == null) return NotFound($"Unable to load user with ID '{_userManager.GetUserId(User)}'.");

			if (!ModelState.IsValid)
			{
				await LoadAsync(user);
				return Page();
			}

			string? newAvatarPath = Input.AvatarPath;
			if (Input.AvatarFile != null)
			{
				string folderPath = "avatar";
				if (User.IsInRole("Shipper"))
					folderPath = "avatar\\shipper";
				else if (User.IsInRole("NhanVien") || User.IsInRole("Admin"))
					folderPath = "avatar\\nhanvien";
				newAvatarPath = Shipping.Utilities.Image.Upload(Input.AvatarFile, folderPath);
			}

			var phoneNumber = await _userManager.GetPhoneNumberAsync(user);
			if (Input.PhoneNumber != phoneNumber)
			{
				await _userManager.SetPhoneNumberAsync(user, Input.PhoneNumber);
			}

			Utilities.ReturnState result = new Utilities.ReturnState();

			if (User.IsInRole("KhachHang"))
			{
				var model = new KhachHangViewModel
				{
					Id = Input.Id,
					UserId = user.Id,
					Email = Input.Email,
					Ten = Input.Ten,
					SDT = Input.PhoneNumber,
					CCCD = Input.CCCD,
					MaSoThue = Input.MaSoThue,
					DiaChi = Input.DiaChi,
					TinhThanhId = Input.TinhThanhId,
					PhuongXaId = Input.PhuongXaId
				};
				result = await _khachHangService.Edit(model);
			}
			else if (User.IsInRole("Shipper"))
			{
				var model = new ShipperViewModel
				{
					Id = Input.Id,
					UserId = user.Id,
					Email = Input.Email,
					Ten = Input.Ten,
					SDT = Input.PhoneNumber,
					CCCD = Input.CCCD,
					DiaChi = Input.DiaChi,
					TinhThanhId = Input.TinhThanhId,
					PhuongXaId = Input.PhuongXaId,

					NgaySinh = Input.NgaySinh ?? DateTime.Now,
					GioiTinh = Input.GioiTinh ?? true,
					BienSoXe = Input.BienSo,
					LoaiShipper = Input.LoaiShipper ?? LoaiShipper.GiaoHang,
					HinhAnh = newAvatarPath
				};
				result = await _shipperService.Edit(model);
			}
			else if (User.IsInRole("NhanVien") || User.IsInRole("Admin"))
			{
				var model = new NhanVienViewModel
				{
					Id = Input.Id,
					UserId = user.Id,
					Email = Input.Email,
					Ten = Input.Ten,
					SDT = Input.PhoneNumber,
					CCCD = Input.CCCD,
					DiaChi = Input.DiaChi,
					TinhThanhId = Input.TinhThanhId,
					PhuongXaId = Input.PhuongXaId,
					IsQuanLy = User.IsInRole("Admin") ? true : false,
					NgaySinh = Input.NgaySinh ?? DateTime.Now,
					GioiTinh = Input.GioiTinh ?? true,
					HinhAnh = newAvatarPath
				};
				result = await _nhanVienService.Edit(model);
			}

			if (result.Completed)
			{
				await _signInManager.RefreshSignInAsync(user);
				StatusMessage = "Hồ sơ của bạn đã được cập nhật";
				return RedirectToPage();
			}
			else
			{
				StatusMessage = "Error: " + result.Message;
				await LoadAsync(user);
				return Page();
			}
		}
    }
}
