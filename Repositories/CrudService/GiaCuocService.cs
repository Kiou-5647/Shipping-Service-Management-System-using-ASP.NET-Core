using Microsoft.EntityFrameworkCore;
using Shipping.Data;
using Shipping.Models;
using Shipping.Utilities;

namespace Shipping.Repositories.CrudService
{
	public class GiaCuocService : ICrudService<CauTrucGiaCuoc>
	{
		private readonly ApplicationDbContext context;

		public GiaCuocService(ApplicationDbContext _context)
		{
			context = _context;
		}

		public async Task<ReturnState> Create(CauTrucGiaCuoc model)
		{
			if (model == null)
			{
				return new ReturnState() { Completed = false, Message = "Có lỗi xảy ra trong quá trình thực thi!" };
			}
			var gc = await context.CauTrucGiaCuocs.FirstOrDefaultAsync(gc => gc.LoaiDichVuId == model.LoaiDichVuId &&
																	   gc.LoaiVungGia == model.LoaiVungGia);
			if (gc != null && gc.Id != model.Id)
				return new ReturnState()
				{
					Completed = false,
					Message = $"Cấu trúc giá cước {gc.LoaiVungGia} cho {gc.LoaiDichVu.TenDichVu} đã tồn tại!"
				};

			await context.CauTrucGiaCuocs.AddAsync(model);
			if (Save())
				return new ReturnState() { Completed = true, Message = "Thêm cấu trúc giá cước thành công!" };
			else
				return new ReturnState() { Completed = false, Message = "Thêm cấu trúc giá cước thất bại!" };
		}

		public async Task<ReturnState> Delete(int? id)
		{
			var gc = await GetById(id);
			if (gc == null)
				return new ReturnState() { Completed = false, Message = "Không tìm thấy cấu trúc giá cước!" };

			try
			{
				context.CauTrucGiaCuocs.Remove(gc);
				Save();
				return new ReturnState() { Completed = true, Message = "Xóa cấu trúc giá cước thành công!" };
			}
			catch
			{
				return new ReturnState() { Completed = false, Message = "Xóa cấu trúc giá cước thất bại!" };
			}
		}

		public async Task<ReturnState> Edit(CauTrucGiaCuoc model)
		{
			if (model == null)
				return new ReturnState() { Completed = false, Message = "Có lỗi xảy ra khi cập nhật thông tin cấu trúc giá cước!" };

			var gc = await context.CauTrucGiaCuocs.Include(c => c.LoaiDichVu).AsNoTracking().FirstOrDefaultAsync(gc => gc.LoaiDichVuId == model.LoaiDichVuId &&
																	   gc.LoaiVungGia == model.LoaiVungGia);
			if (gc != null && gc.Id != model.Id)
				return new ReturnState()
				{
					Completed = false,
					Message = $"Cấu trúc giá cước {gc.LoaiVungGia} cho {gc.LoaiDichVu.TenDichVu} đã tồn tại!"
				};

			context.Update(model);
			if (Save())
				return new ReturnState()
				{
					Completed = true,
					Message = "Cập nhật thông tin cấu trúc giá cước thành công!"
				};
			else
				return new ReturnState()
				{
					Completed = false,
					Message = "Cập nhật thông tin cấu trúc giá cước thất bại!"
				};
		}

		public IEnumerable<CauTrucGiaCuoc> GetAll()
		{
			return context.CauTrucGiaCuocs;
		}

		public async Task<CauTrucGiaCuoc?> GetById(int? id)
		{
			return await context.CauTrucGiaCuocs.FirstOrDefaultAsync(gc => gc.Id == id);
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
