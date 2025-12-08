using Microsoft.EntityFrameworkCore;
using Shipping.Data;
using Shipping.Models;
using Shipping.Utilities;

namespace Shipping.Repositories.CrudService
{
	public class LoaiDichVuService : ICrudService<LoaiDichVu>
	{
		private readonly ApplicationDbContext context;

		public LoaiDichVuService(ApplicationDbContext _context)
		{
			context = _context;
		}

		public async Task<ReturnState> Create(LoaiDichVu model)
		{
			if (model == null)
			{
				return new ReturnState() { Completed = false, Message = "Có lỗi xảy ra trong quá trình thực thi!" };
			}
			var dv = await context.LoaiDichVus.FirstOrDefaultAsync(dv => dv.TenDichVu == model.TenDichVu);
			if (dv != null && dv.Id != model.Id)
				return new ReturnState()
				{
					Completed = false,
					Message = $"{dv.TenDichVu} đã tồn tại!"
				};

			await context.LoaiDichVus.AddAsync(model);
			if (Save())
				return new ReturnState() { Completed = true, Message = "Thêm loại dịch vụ thành công!" };
			else
				return new ReturnState() { Completed = false, Message = "Thêm loại dịch vụ thất bại!" };

		}

		public async Task<ReturnState> Delete(int? id)
		{
			var dv = await GetById(id);
			if (dv == null)
				return new ReturnState() { Completed = false, Message = "Không tìm thấy loại dịch vụ!" };

			try
			{
				context.LoaiDichVus.Remove(dv);
				Save();
				return new ReturnState() { Completed = true, Message = "Xóa loại dịch vụ thành công!" };
			}
			catch
			{
				return new ReturnState() { Completed = false, Message = "Xóa loại dịch vụ thất bại!" };
			}
		}

		public async Task<ReturnState> Edit(LoaiDichVu model)
		{
			if (model == null)
				return new ReturnState() { Completed = false, Message = "Có lỗi xảy ra khi cập nhật thông tin loại dịch vụ!" };

			var dv = await context.LoaiDichVus.AsNoTracking().FirstOrDefaultAsync(c => c.TenDichVu == model.TenDichVu);
			if (dv != null && dv.Id != model.Id)
				return new ReturnState()
				{
					Completed = false,
					Message = $"{dv.TenDichVu} đã tồn tại!"
				};

			context.Update(model);
			if (Save())
				return new ReturnState()
				{
					Completed = true,
					Message = "Cập nhật thông tin loại dịch vụ thành công"
				};
			else
				return new ReturnState()
				{
					Completed = false,
					Message = "Cập nhật thông tin loại dịch vụ thất bại"
				};
		}

		public IEnumerable<LoaiDichVu> GetAll()
		{
			return context.LoaiDichVus.Include(dv => dv.CauTrucGiaCuocs);
		}

		public async Task<LoaiDichVu?> GetById(int? id)
		{
			return await context.LoaiDichVus.Include(dv => dv.CauTrucGiaCuocs).FirstOrDefaultAsync(dv => dv.Id == id);
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
