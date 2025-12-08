using Microsoft.EntityFrameworkCore;
using Shipping.Data;
using Shipping.Models;

namespace Shipping.Repositories.GeoService
{
	public class TinhThanhService : IGeoService<TinhThanh>
	{
		private readonly ApplicationDbContext context;

		public TinhThanhService(ApplicationDbContext _context)
		{
			context = _context;
		}

		public IEnumerable<TinhThanh> GetAll()
		{
			return context.TinhThanhs.Include(t => t.VungMien);
		}

		public async Task<TinhThanh?> GetById(string id)
		{
			return await context.TinhThanhs.Include(t => t.VungMien).FirstOrDefaultAsync(x => x.Id == id);
		}
	}
}
