// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
#nullable disable

using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.Logging;
using Shipping.Models;
using Shipping.Repositories.GeoService;
using Shipping.Repositories.UserService;
using Shipping.ViewModels;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Text.Encodings.Web;
using System.Threading;
using System.Threading.Tasks;

namespace Shipping.Areas.Identity.Pages.Account
{
    public class RegisterModel : PageModel
    {
        private readonly SignInManager<IdentityUser> _signInManager;
        private readonly UserManager<IdentityUser> _userManager;
        private readonly IUserStore<IdentityUser> _userStore;
        private readonly ILogger<RegisterModel> _logger;

		private readonly IUserService<KhachHang, KhachHangViewModel> _khachHangService;
		private readonly IGeoService<TinhThanh> _tinhThanhService;

		public RegisterModel(
			UserManager<IdentityUser> userManager,
			IUserStore<IdentityUser> userStore,
			SignInManager<IdentityUser> signInManager,
			ILogger<RegisterModel> logger,
			IUserService<KhachHang, KhachHangViewModel> khachHangService,
			IGeoService<TinhThanh> tinhThanhService)
		{
            _userManager = userManager;
            _userStore = userStore;
			_signInManager = signInManager;
            _logger = logger;
            _khachHangService = khachHangService;
            _tinhThanhService = tinhThanhService;
		}

		/// <summary>
		///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
		///     directly from your code. This API may change or be removed in future releases.
		/// </summary>
		/// 

		[BindProperty]
		public KhachHangViewModel Input { get; set; } = new KhachHangViewModel();

		/// <summary>
		///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
		///     directly from your code. This API may change or be removed in future releases.
		/// </summary>
		public string ReturnUrl { get; set; }

        /// <summary>
        ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
        ///     directly from your code. This API may change or be removed in future releases.
        /// </summary>
        public IList<AuthenticationScheme> ExternalLogins { get; set; }

        /// <summary>
        ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
        ///     directly from your code. This API may change or be removed in future releases.
        /// </summary>

        public async Task OnGetAsync(string returnUrl = null)
        {
			ReturnUrl = returnUrl;
			ExternalLogins = (await _signInManager.GetExternalAuthenticationSchemesAsync()).ToList();
			ViewData["TinhThanhList"] = new SelectList(_tinhThanhService.GetAll(), "Id", "TenTinhThanh");

			Input.LoaiKhachHang = Models.Enums.LoaiKhachHang.CaNhan;
		}

        public async Task<IActionResult> OnPostAsync(string returnUrl = null)
        {
            returnUrl ??= Url.Content("~/");
            ExternalLogins = (await _signInManager.GetExternalAuthenticationSchemesAsync()).ToList();

			if (ModelState.IsValid)
			{
				var result = await _khachHangService.Create(Input);

				if (result.Completed)
				{
					_logger.LogInformation("Đã đăng ký tài khoản thành công.");
					var user = await _userManager.FindByEmailAsync(Input.Email);

					if (user != null)
					{
						// Tự động đăng nhập
						await _signInManager.SignInAsync(user, isPersistent: false);
						return LocalRedirect(returnUrl);
					}
					else
					{
						_logger.LogError("Không thể tìm thấy người dùng.");
						ModelState.AddModelError(string.Empty, "Đăng ký thành công nhưng không thể đăng nhập tự động. Vui lòng đăng nhập thủ công.");
						return RedirectToPage("./Login", new { returnUrl = returnUrl });
					}
				}
				else
				{
					ModelState.AddModelError(string.Empty, result.Message ?? "Đăng ký thất bại!");

					ViewData["TinhThanhList"] = new SelectList(_tinhThanhService.GetAll(), "Id", "TenTinhThanh", Input.TinhThanhId);
					return Page();
				}
			}

			// Nếu thất bại (validation hoặc lỗi Identity), load lại danh sách Tỉnh/Thành
			var tinhThanhsFailed = _tinhThanhService.GetAll();
			ViewData["TinhThanhList"] = new SelectList(tinhThanhsFailed, "Id", "TenTinhThanh", Input.TinhThanhId);
			return Page();
		}
    }
}
