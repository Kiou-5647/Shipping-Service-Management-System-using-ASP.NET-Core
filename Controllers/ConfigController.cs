using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Shipping.Data;
using Shipping.Models;
using Shipping.Repositories.ConfigService;
using Shipping.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Shipping.Controllers
{
	[Authorize(Roles = "Admin")]
    public class ConfigController : Controller
    {
		private readonly IConfigService _configService;

		public ConfigController(IConfigService configService)
		{
			_configService = configService;
		}

		[HttpGet]
		public IActionResult Index()
		{
			var configs = _configService.GetAll();
			var viewModel = new ConfigListViewModel { Configs = configs.ToList() };
			return View(viewModel);
		}

		[HttpPost]
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> Index(ConfigListViewModel viewModel)
        {
			if (!ModelState.IsValid)
			{
				return View(viewModel);
			}

			var result = await _configService.Edit(viewModel.Configs);

			if (result.Completed)
			{
				TempData["SuccessMessage"] = result.Message;
				return RedirectToAction(nameof(Index));
			}
			else
			{
				ModelState.AddModelError(string.Empty, result.Message);
				return View(viewModel);
			}
		}
    }
}
