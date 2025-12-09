using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Shipping.Data;
using Shipping.Models;
using Shipping.Utilities;
using Shipping.ViewModels;

namespace Shipping.Repositories.UserService
{
	public class NhanVienService : IUserService<NhanVien, NhanVienViewModel>
	{
		private readonly ApplicationDbContext context;
		private readonly UserManager<IdentityUser> userManager;

		public NhanVienService(ApplicationDbContext _context, UserManager<IdentityUser> _userManager)
		{
			context = _context;
			userManager = _userManager;
		}

		public async Task<ReturnState> Delete(int? id)
		{
			var nv = await GetById(id);
			if (nv == null)
			{
				return new ReturnState() { Completed = false, Message = "Không tìm thấy nhân viên!" };
			}
			var user = await userManager.FindByIdAsync(nv.UserId);
			if (user != null)
			{
				user.LockoutEnabled = true;
				user.LockoutEnd = DateTime.MaxValue;
				await userManager.UpdateAsync(user);
			}
			else return new ReturnState() { Completed = false, Message = "Không tìm thấy tài khoản người dùng!" };

			nv.IsDeleted = true;
			context.Update(nv);
			if (Save())
				return new ReturnState() { Completed = true, Message = "Xóa nhân viên thành công!" };
			else
				return new ReturnState() { Completed = false, Message = "Xóa nhân viên thất bại!" };
		}

		public async Task<ReturnState> Create(NhanVienViewModel model)
		{
			if (model == null)
			{
				return new ReturnState()
				{
					Completed = false,
					Message = "Có lỗi xảy ra khi thêm nhân viên!"
				};
			}
			var existingUserByEmail = await userManager.FindByEmailAsync(model.Email);
			if (existingUserByEmail != null)
			{
				return new ReturnState() { Completed = false, Message = "Email đã được sử dụng!" };
			}

			if (await context.NhanViens.AnyAsync(nv => nv.CCCD == model.CCCD))
			{
				return new ReturnState() { Completed = false, Message = "Căn cước công dân đã được sử dụng bởi Nhân Viên!" };
			}
			if (await context.Shippers.AnyAsync(s => s.CCCD == model.CCCD))
			{
				return new ReturnState() { Completed = false, Message = "Căn cước công dân đã được sử dụng bởi Shipper!" };
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
				var result = await userManager.CreateAsync(user, "nhanvien");
				if (!result.Succeeded)
					return new ReturnState() { Completed = false, Message = $"Tạo tài khoản thất bại: {result.Errors.FirstOrDefault()?.Description}" };
			}

			if(model.IsQuanLy)
				await userManager.AddToRoleAsync(user, "Admin");
			else
				await userManager.AddToRoleAsync(user, "NhanVien");

			await userManager.UpdateSecurityStampAsync(user);

			var nv = new NhanVien
			{
				UserId = user.Id,
				Ten = model.Ten,
				CCCD = model.CCCD,
				GioiTinh = model.GioiTinh,
				NgaySinh = model.NgaySinh,
				DiaChi = model.DiaChi,
				TinhThanhId = model.TinhThanhId,
				PhuongXaId = model.PhuongXaId,
				IsQuanLy = model.IsQuanLy,
				ChiNhanhId = model.ChiNhanhId,
				HinhAnh = model.file != null ? Image.Upload(model.file, "avatar\\nhanvien") : null,
				NgayCapNhat = DateTime.Now
			};
			await context.NhanViens.AddAsync(nv);
			if (Save())
				return new ReturnState() { Completed = true, Message = "Thêm nhân viên thành công!" };
			else
			{
				await userManager.DeleteAsync(user);
				return new ReturnState() { Completed = false, Message = "Thêm nhân viên thất bại! (Lỗi lưu cơ sở dữ liệu)" };
			}


		}

		public async Task<ReturnState> Edit(NhanVienViewModel model)
		{
			if (model == null)
				return new ReturnState() { Completed = false, Message = "Dữ liệu không hợp lệ!" };

			var nvToEdit = await context.NhanViens.Include(nv => nv.User).FirstOrDefaultAsync(nv => nv.Id == model.Id);
			if (nvToEdit == null || nvToEdit.User == null)
				return new ReturnState() { Completed = false, Message = "Không tìm thấy nhân viên hoặc tài khoản liên kết!" };

			var user = nvToEdit.User;

			if (user.Email != model.Email)
			{
				var existingUserByEmail = await userManager.FindByEmailAsync(model.Email);
				if (existingUserByEmail != null && existingUserByEmail.Id != user.Id)
				{
					return new ReturnState() { Completed = false, Message = "Email đã được sử dụng!" };
				}
			}

			if (nvToEdit.CCCD != model.CCCD)
			{
				if (await context.NhanViens.AnyAsync(nv => nv.CCCD == model.CCCD && nv.Id != model.Id))
				{
					return new ReturnState() { Completed = false, Message = "Căn cước công dân đã được sử dụng!" };
				}
				if (await context.Shippers.AnyAsync(s => s.CCCD == model.CCCD))
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

			if (model.IsQuanLy && !nvToEdit.IsQuanLy)
			{
				await userManager.RemoveFromRoleAsync(user, "NhanVien");
				await userManager.AddToRoleAsync(user, "Admin");
			}
			else if (!model.IsQuanLy && nvToEdit.IsQuanLy)
			{
				await userManager.RemoveFromRoleAsync(user, "Admin");
				await userManager.AddToRoleAsync(user, "NhanVien");
			}

			await userManager.UpdateSecurityStampAsync(user);

			var updateResult = await userManager.UpdateAsync(user);
			if (!updateResult.Succeeded)
			{
				return new ReturnState() { Completed = false, Message = $"Cập nhật tài khoản thất bại: {updateResult.Errors.FirstOrDefault()?.Description}" };
			}

			nvToEdit.Ten = model.Ten;
			nvToEdit.CCCD = model.CCCD;
			nvToEdit.GioiTinh = model.GioiTinh;
			nvToEdit.NgaySinh = model.NgaySinh;
			nvToEdit.DiaChi = model.DiaChi;
			nvToEdit.TinhThanhId = model.TinhThanhId;
			nvToEdit.PhuongXaId = model.PhuongXaId;
			nvToEdit.IsQuanLy = model.IsQuanLy;
			nvToEdit.ChiNhanhId = model.ChiNhanhId;
			nvToEdit.HinhAnh = model.HinhAnh;

			if (model.file != null)
			{
				nvToEdit.HinhAnh = Image.Upload(model.file, "avatar\\nhanvien");
			}

			context.NhanViens.Update(nvToEdit);
			if (Save())
				return new ReturnState() { Completed = true, Message = "Cập nhật thông tin nhân viên thành công!" };
			else
				return new ReturnState() { Completed = false, Message = "Cập nhật thông tin nhân viên thất bại! (Lỗi lưu cơ sở dữ liệu)" };
		}

		public IEnumerable<NhanVien> GetAll()
		{
			return context.NhanViens.Include(nv => nv.User)
				.Include(nv => nv.ChiNhanh)
				.Include(nv => nv.TinhThanh)
				.Include(nv => nv.PhuongXa)
				.Where(nv => !nv.IsDeleted);
		}

		public IEnumerable<NhanVien> GetAllActive()
		{
			return context.NhanViens.Include(nv => nv.User)
				.Include(nv => nv.TinhThanh)
				.Include(nv => nv.PhuongXa)
				.Include(nv => nv.ChiNhanh).ThenInclude(c => c.TinhThanh)
				.Include(nv => nv.ChiNhanh).ThenInclude(c => c.PhuongXa)
				.Where(nv => !nv.IsDeleted && (nv.User.LockoutEnd == null || nv.User.LockoutEnd < DateTimeOffset.Now));
		}

		public IEnumerable<NhanVien> GetAllInactive()
		{
			return context.NhanViens.Include(nv => nv.User)
				.Include(nv => nv.TinhThanh)
				.Include(nv => nv.PhuongXa)
				.Include(nv => nv.ChiNhanh).ThenInclude(c => c.TinhThanh)
				.Include(nv => nv.ChiNhanh).ThenInclude(c => c.PhuongXa)
				.Where(nv => nv.IsDeleted || (nv.User.LockoutEnd != null || nv.User.LockoutEnd >= DateTimeOffset.Now));
		}

		public async Task<NhanVien?> GetById(int? id)
		{
			if (id == null || id == 0) return null;
			return await context.NhanViens.Include(nv => nv.User)
				.Include(nv => nv.TinhThanh)
				.Include(nv => nv.PhuongXa)
				.Include(nv => nv.ChiNhanh).ThenInclude(c => c.TinhThanh)
				.Include(nv => nv.ChiNhanh).ThenInclude(c => c.PhuongXa)
				.FirstOrDefaultAsync(nv => nv.Id == id && !nv.IsDeleted);
		}

		public bool Save()
		{
			return context.SaveChanges() > 0;
		}
	}
}
