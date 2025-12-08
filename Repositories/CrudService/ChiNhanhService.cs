using Microsoft.EntityFrameworkCore;
using Shipping.Data;
using Shipping.Models;
using Shipping.Utilities;

namespace Shipping.Repositories.CrudService
{
	public class ChiNhanhService : ICrudService<ChiNhanh>
	{
		private readonly ApplicationDbContext context;

		public ChiNhanhService(ApplicationDbContext _context)
		{
			context = _context;
		}

		public IEnumerable<ChiNhanh> GetAll()
		{
			return context.ChiNhanhs.Include(c => c.TinhThanh).Include(c => c.PhuongXa);
		}

		public async Task<ReturnState> Delete(int? id)
		{
			var cn = await GetById(id);
			if (cn == null)
				return new ReturnState() { Completed = false, Message = "Không tìm thấy chi nhánh!" };
			try
			{
				context.ChiNhanhs.Remove(cn);
				Save();
				return new ReturnState() { Completed = true, Message = "Xóa chi nhánh thành công!" };
			}
			catch
			{
				return new ReturnState() { Completed = false, Message = "Xóa chi nhánh thất bại!" };
			}
		}

		public async Task<ChiNhanh?> GetById(int? id)
		{
			return await context.ChiNhanhs.Include(c => c.TinhThanh).Include(c => c.PhuongXa).FirstOrDefaultAsync(cn => cn.Id == id);
		}

		public async Task<ReturnState> Create(ChiNhanh model)
		{
			if (model == null)
			{
				return new ReturnState() { Completed = false, Message = "Có lỗi xảy ra trong quá trình thực thi!" };
			}
			var cn = await context.ChiNhanhs.FirstOrDefaultAsync(c => c.TenChiNhanh == model.TenChiNhanh);
			if (cn != null && cn.Id != model.Id)
				return new ReturnState()
				{
					Completed = false,
					Message = $"{cn.TenChiNhanh} đã tồn tại!"
				};
			await context.ChiNhanhs.AddAsync(model);
			if (Save())
				return new ReturnState() { Completed = true, Message = "Thêm chi nhánh thành công!" };
			else
				return new ReturnState() { Completed = false, Message = "Thêm chi nhánh thất bại!" };
		}

		public async Task<ReturnState> Edit(ChiNhanh model)
		{
			if (model == null)
				return new ReturnState() { Completed = false, Message = "Có lỗi xảy ra khi cập nhật thông tin chi nhánh!" };

			var cn = await context.ChiNhanhs.AsNoTracking().FirstOrDefaultAsync(c => c.TenChiNhanh == model.TenChiNhanh);
			if (cn != null && cn.Id != model.Id)
				return new ReturnState()
				{
					Completed = false,
					Message = $"{cn.TenChiNhanh} đã tồn tại!"
				};

			context.Update(model);
			if (Save())
				return new ReturnState()
				{
					Completed = true,
					Message = "Cập nhật thông tin chi nhánh thành công"
				};
			else
				return new ReturnState()
				{
					Completed = false,
					Message = "Cập nhật thông tin chi nhánh thất bại"
				};
		}

		public bool Save()
		{
			try
			{
				context.SaveChanges();
				return true;
			}
			catch
			{
				return false;
			}
		}
	}
}
