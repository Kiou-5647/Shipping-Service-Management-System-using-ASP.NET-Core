using Microsoft.EntityFrameworkCore;
using Shipping.Data;
using Shipping.Models;

namespace Shipping.Repositories.GeoService
{
	public class PhuongXaService : IGeoService<PhuongXa>
	{
		private readonly ApplicationDbContext context;

		public PhuongXaService(ApplicationDbContext _context)
		{
			context = _context;
		}

		public IEnumerable<PhuongXa> GetAll()
		{
			return context.PhuongXas.Include(p => p.TinhThanh).ThenInclude(t => t.VungMien);
		}

		public async Task<PhuongXa?> GetById(string id)
		{
			return await context.PhuongXas.Include(p => p.TinhThanh).ThenInclude(t => t.VungMien).FirstOrDefaultAsync(p => p.Id == id);
		}
	}
}
