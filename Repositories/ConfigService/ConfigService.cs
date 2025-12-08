using Microsoft.EntityFrameworkCore;
using NuGet.Configuration;
using Shipping.Data;
using Shipping.Models;
using Shipping.Utilities;

namespace Shipping.Repositories.ConfigService
{
	public class ConfigService : IConfigService
	{
		private readonly ApplicationDbContext _context;

		public ConfigService(ApplicationDbContext context)
		{
			_context = context;
		}

		public async Task<ReturnState> Edit(List<Config> configs)
		{
			if (configs == null || !configs.Any())
			{
				return new ReturnState { Completed = false, Message = "Danh sách cấu hình trống." };
			}

			try
			{
				foreach (var config in configs)
				{
					_context.Configs.Update(config);
				}
				await _context.SaveChangesAsync();
				return new ReturnState { Completed = true, Message = "Cập nhật cấu hình thành công." };
			}
			catch (Exception ex)
			{
				return new ReturnState { Completed = false, Message = $"Cập nhật cấu hình thất bại: {ex.Message}" };
			}
		}

		public IEnumerable<Config> GetAll()
		{
			return  _context.Configs;
		}
	}
}
