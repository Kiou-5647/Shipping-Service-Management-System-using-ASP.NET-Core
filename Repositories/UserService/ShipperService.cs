using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Shipping.Data;
using Shipping.Models;
using Shipping.Models.Enums;
using Shipping.Utilities;
using Shipping.ViewModels;

namespace Shipping.Repositories.UserService
{
	public class ShipperService : IShipperService
	{
		private readonly ApplicationDbContext context;
		private readonly UserManager<IdentityUser> userManager;

		public ShipperService(ApplicationDbContext _context, UserManager<IdentityUser> _userManager)
		{
			context = _context;
			userManager = _userManager;
		}

		public async Task<ReturnState> Delete(int? id)
		{
			var shipper = await GetById(id);
			if (shipper == null)
			{
				return new ReturnState() { Completed = false, Message = "Không tìm thấy shipper!" };
			}
			var user = await userManager.FindByIdAsync(shipper.UserId);
			if (user != null)
			{
				user.LockoutEnd = DateTime.MaxValue;
				await userManager.UpdateAsync(user);
			}
			else return new ReturnState() { Completed = false, Message = "Không tìm thấy tài khoản người dùng!" };

			shipper.IsDeleted = true;
			context.Update(shipper);
			if (Save())
				return new ReturnState() { Completed = true, Message = "Xóa shipper thành công!" };
			else
				return new ReturnState() { Completed = false, Message = "Xóa shipper thất bại!" };
		}

		public async Task<ReturnState> Create(ShipperViewModel model)
		{
			if (model == null)
			{
				return new ReturnState()
				{
					Completed = false,
					Message = "Có lỗi xảy ra khi thêm shipper!"
				};
			}

			var existingUserByEmail = await userManager.FindByEmailAsync(model.Email);
			if (existingUserByEmail != null)
			{
				return new ReturnState() { Completed = false, Message = "Email đã được sử dụng!" };
			}

			if (await context.Shippers.AnyAsync(s => s.CCCD == model.CCCD))
			{
				return new ReturnState() { Completed = false, Message = "Căn cước công dân đã được sử dụng!" };
			}
			if (await context.NhanViens.AnyAsync(nv => nv.CCCD == model.CCCD))
			{
				return new ReturnState() { Completed = false, Message = "Căn cước công dân đã được sử dụng!" };
			}

			if (await context.Shippers.AnyAsync(s => s.BienSoXe == model.BienSoXe))
			{
				return new ReturnState() { Completed = false, Message = "Biển số xe đã được sử dụng!"};
			}

			var user = new IdentityUser
			{
				UserName = model.Email,
				Email = model.Email,
				PhoneNumber = model.SDT,
			};

			if (model.Password != null)
			{
				var result = await userManager.CreateAsync(user, model.Password);
				if (!result.Succeeded)
					return new ReturnState() { Completed = false, Message = $"Tạo tài khoản thất bại: {result.Errors.FirstOrDefault()?.Description}" };
			}
			else
			{
				var result = await userManager.CreateAsync(user, "shipper");
				if (!result.Succeeded)
					return new ReturnState() { Completed = false, Message = $"Tạo tài khoản thất bại: {result.Errors.FirstOrDefault()?.Description}" };
			}

			await userManager.AddToRoleAsync(user, "Shipper");
			var shipper = new Shipper
			{
				UserId = user.Id,
				Ten = model.Ten,
				CCCD = model.CCCD,
				GioiTinh = model.GioiTinh,
				NgaySinh = model.NgaySinh,
				DiaChi = model.DiaChi,
				TinhThanhId = model.TinhThanhId,
				PhuongXaId = model.PhuongXaId,
				LoaiShipper = model.LoaiShipper,
				BienSoXe = model.BienSoXe,
				ChiNhanhId = model.ChiNhanhId,
				HinhAnh = model.file != null ? Image.Upload(model.file, "avatar\\shipper") : null,
				NgayCapNhat = DateTime.Now
			};

			await context.Shippers.AddAsync(shipper);
			if (Save())
				return new ReturnState() { Completed = true, Message = "Thêm shipper thành công!" };
			else
			{
				await userManager.DeleteAsync(user);
				return new ReturnState() { Completed = false, Message = "Thêm shipper thất bại! (Lỗi lưu cơ sở dữ liệu)" };
			}
		}

		public async Task<ReturnState> Edit(ShipperViewModel model)
		{
			if (model == null)
				return new ReturnState() { Completed = false, Message = "Dữ liệu không hợp lệ!" };

			var shipperToEdit = await context.Shippers.Include(s => s.User).FirstOrDefaultAsync(s => s.Id == model.Id);
			if (shipperToEdit == null || shipperToEdit.User == null)
				return new ReturnState() { Completed = false, Message = "Không tìm thấy shipper hoặc tài khoản liên kết!" };

			var user = shipperToEdit.User;

			if (user.Email != model.Email)
			{
				var existingUserByEmail = await userManager.FindByEmailAsync(model.Email);
				if (existingUserByEmail != null && existingUserByEmail.Id != user.Id)
				{
					return new ReturnState() { Completed = false, Message = "Email đã được sử dụng bởi tài khoản khác!" };
				}
			}

			if (shipperToEdit.CCCD != model.CCCD)
			{
				if (await context.Shippers.AnyAsync(s => s.CCCD == model.CCCD && s.Id != model.Id))
				{
					return new ReturnState() { Completed = false, Message = "Căn cước công dân đã được sử dụng!" };
				}
				if (await context.NhanViens.AnyAsync(nv => nv.CCCD == model.CCCD))
				{
					return new ReturnState() { Completed = false, Message = "Căn cước công dân đã được sử dụng!" };
				}
			}

			if (user.Email != model.Email)
			{
				await userManager.SetEmailAsync(user, model.Email);
				await userManager.SetUserNameAsync(user, model.Email);
			}

			if (user.PhoneNumber != model.SDT)
			{
				await userManager.SetPhoneNumberAsync(user, model.SDT);
			}

			var updateResult = await userManager.UpdateAsync(user);
			if (!updateResult.Succeeded)
			{
				return new ReturnState() { Completed = false, Message = $"Cập nhật tài khoản thất bại: {updateResult.Errors.FirstOrDefault()?.Description}" };
			}

			shipperToEdit.Ten = model.Ten;
			shipperToEdit.CCCD = model.CCCD;
			shipperToEdit.GioiTinh = model.GioiTinh;
			shipperToEdit.NgaySinh = model.NgaySinh;
			shipperToEdit.DiaChi = model.DiaChi;
			shipperToEdit.TinhThanhId = model.TinhThanhId;
			shipperToEdit.PhuongXaId = model.PhuongXaId;
			shipperToEdit.LoaiShipper = model.LoaiShipper;
			shipperToEdit.BienSoXe = model.BienSoXe;
			shipperToEdit.ChiNhanhId = model.ChiNhanhId;
			shipperToEdit.HinhAnh = model.HinhAnh;

			if (model.file != null)
			{
				shipperToEdit.HinhAnh = Image.Upload(model.file, "avatar\\shipper");
			}
			shipperToEdit.NgayCapNhat = DateTime.Now;

			context.Shippers.Update(shipperToEdit);
			if (Save())
				return new ReturnState() { Completed = true, Message = "Cập nhật thông tin shipper thành công!" };
			else
				return new ReturnState() { Completed = false, Message = "Cập nhật thông tin shipper thất bại! (Lỗi lưu cơ sở dữ liệu)" };
		}

		public IEnumerable<Shipper> GetAll()
		{
			return context.Shippers.Include(s => s.User)
				.Include(nv => nv.ChiNhanh).ThenInclude(c => c.TinhThanh)
				.Include(nv => nv.ChiNhanh).ThenInclude(c => c.PhuongXa)
				.Where(s => !s.IsDeleted);
		}

		public IEnumerable<Shipper> GetAllActive()
		{
			return context.Shippers.Include(s => s.User)
				.Include(nv => nv.ChiNhanh).ThenInclude(c => c.TinhThanh)
				.Include(nv => nv.ChiNhanh).ThenInclude(c => c.PhuongXa)
				.Where(s => !s.IsDeleted && (s.User.LockoutEnd == null || s.User.LockoutEnd < DateTimeOffset.Now));
		}

		public IEnumerable<Shipper> GetAllInactive()
		{
			return context.Shippers.Include(s => s.User)
				.Include(nv => nv.ChiNhanh).ThenInclude(c => c.TinhThanh)
				.Include(nv => nv.ChiNhanh).ThenInclude(c => c.PhuongXa)
				.Where(s => !s.IsDeleted && (s.User.LockoutEnd != null || s.User.LockoutEnd >= DateTimeOffset.Now));
		}

		public async Task<Shipper?> GetById(int? id)
		{
			if (id == null || id == 0) return null;
			return await context.Shippers.Include(s => s.User)
				.Include(nv => nv.ChiNhanh).ThenInclude(c => c.TinhThanh)
				.Include(nv => nv.ChiNhanh).ThenInclude(c => c.PhuongXa)
				.FirstOrDefaultAsync(s => s.Id == id && !s.IsDeleted);
		}

		public bool Save()
		{
			return context.SaveChanges() > 0;
		}

		public async Task<Shipper?> GetByUserId(string userId)
		{
			return await context.Shippers
				.Include(s => s.User)
				.Include(s => s.ChiNhanh)
				.FirstOrDefaultAsync(s => s.UserId == userId && !s.IsDeleted);
		}

		public async Task<ReturnState> UpdateState(string userId, string viTri, int trangThai)
		{
			var shipper = await context.Shippers.FirstOrDefaultAsync(s => s.UserId == userId);
			if (shipper == null)
			{
				return new ReturnState() { Completed = false, Message = "Không tìm thấy Shipper!" };
			}

			try
			{
				shipper.ViTri = viTri;
				shipper.TrangThai = (TrangThaiShipper)trangThai;
				shipper.NgayCapNhat = DateTime.Now;

				context.Shippers.Update(shipper);

				if (Save())
					return new ReturnState() { Completed = true, Message = "Cập nhật trạng thái thành công!" };
				else
					return new ReturnState() { Completed = false, Message = "Lưu dữ liệu thất bại!" };
			}
			catch (Exception ex)
			{
				return new ReturnState() { Completed = false, Message = "Lỗi: " + ex.Message };
			}
		}

		public async Task<IEnumerable<dynamic>> GetAvailableShippers(string locationId)
		{
			var data = await context.Shippers
				.Where(s => !s.IsDeleted
						 && s.TrangThai == TrangThaiShipper.SanSang
						 && s.ViTri == locationId)
				.Select(s => new {
					id = s.Id,
					ten = $"{s.Ten} - {s.User.PhoneNumber}"
				})
				.ToListAsync();

			return data;
		}
	}
}
