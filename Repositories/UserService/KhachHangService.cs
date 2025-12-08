using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Shipping.Data;
using Shipping.Models;
using Shipping.Utilities;
using Shipping.ViewModels;

namespace Shipping.Repositories.UserService
{
	public class KhachHangService : IUserService<KhachHang, KhachHangViewModel>
	{
		private readonly ApplicationDbContext context;
		private readonly UserManager<IdentityUser> userManager;

		public KhachHangService(ApplicationDbContext _context, UserManager<IdentityUser> _userManager)
		{
			context = _context;
			userManager = _userManager;
		}

		public async Task<ReturnState> Create(KhachHangViewModel model)
		{
			if (model == null)
			{
				return new ReturnState() { Completed = false, Message = "Có lỗi xảy ra khi thêm khách hàng!" };
			}

			var existingUserByEmail = await userManager.FindByEmailAsync(model.Email);
			if (existingUserByEmail != null)
			{
				return new ReturnState() { Completed = false, Message = "Email đã được sử dụng!" };
			}

			if (await context.KhachHangs.AnyAsync(kh => kh.CCCD == model.CCCD))
			{
				return new ReturnState() { Completed = false, Message = "Căn cước công dân đã được sử dụng!" };
			}

			if (await context.KhachHangs.AnyAsync(k => k.MaSoThue == model.MaSoThue))
			{
				return new ReturnState() { Completed = false, Message = "Mã số thuế đã được sử dụng!" };
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
				var result = await userManager.CreateAsync(user, "khachhang");
				if (!result.Succeeded)
					return new ReturnState() { Completed = false, Message = $"Tạo tài khoản thất bại: {result.Errors.FirstOrDefault()?.Description}" };
			}

			await userManager.AddToRoleAsync(user, "KhachHang");

			var khachHang = new KhachHang
			{
				UserId = user.Id,
				Ten = model.Ten,
				CCCD = model.CCCD,
				MaSoThue = model.MaSoThue,
				LoaiKhachHang = model.LoaiKhachHang,
				DiaChi = model.DiaChi,
				TinhThanhId = model.TinhThanhId,
				PhuongXaId = model.PhuongXaId,
				NgayCapNhat = DateTime.Now
			};

			await context.KhachHangs.AddAsync(khachHang);
			if (Save())
				return new ReturnState() { Completed = true, Message = "Thêm khách hàng thành công!" };
			else
			{
				await userManager.DeleteAsync(user);
				return new ReturnState() { Completed = false, Message = "Thêm khách hàng thất bại! (Lỗi lưu cơ sở dữ liệu)" };
			}
		}

		public async Task<ReturnState> Delete(int? id)
		{
			var kh = await GetById(id);
			if (kh == null)
			{
				return new ReturnState() { Completed = false, Message = "Không tìm thấy khách hàng!" };
			}
			var user = await userManager.FindByIdAsync(kh.UserId);
			if (user != null)
			{
				user.LockoutEnabled = true;
				user.LockoutEnd = DateTime.MaxValue;
				await userManager.UpdateAsync(user);
			}
			else return new ReturnState() { Completed = false, Message = "Không tìm thấy tài khoản người dùng!" };

			kh.IsDeleted = true;
			context.Update(kh);
			if (Save())
				return new ReturnState() { Completed = true, Message = "Xóa khách hàng thành công!" };
			else
				return new ReturnState() { Completed = false, Message = "Xóa khách hàng thất bại!" };
		}

		public async Task<ReturnState> Edit(KhachHangViewModel model)
		{
			if (model == null)
				return new ReturnState() { Completed = false, Message = "Dữ liệu không hợp lệ!" };

			var khToEdit = await context.KhachHangs.Include(kh => kh.User).FirstOrDefaultAsync(kh => kh.Id == model.Id);
			if (khToEdit == null || khToEdit.User == null)
				return new ReturnState() { Completed = false, Message = "Không tìm thấy khách hàng hoặc tài khoản liên kết!" };

			var user = khToEdit.User;

			if (user.Email != model.Email)
			{
				var existingUserByEmail = await userManager.FindByEmailAsync(model.Email);
				if (existingUserByEmail != null && existingUserByEmail.Id != user.Id)
				{
					return new ReturnState() { Completed = false, Message = "Email đã được sử dụng bởi tài khoản khác!" };
				}
			}

			if (khToEdit.CCCD != model.CCCD && await context.KhachHangs.AnyAsync(kh => kh.CCCD == model.CCCD && kh.Id != model.Id))
			{
				return new ReturnState() { Completed = false, Message = "Căn cước công dân đã được sử dụng bởi Khách Hàng khác!" };
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

			khToEdit.Ten = model.Ten;
			khToEdit.CCCD = model.CCCD;
			khToEdit.MaSoThue = model.MaSoThue;
			khToEdit.LoaiKhachHang = model.LoaiKhachHang;
			khToEdit.DiaChi = model.DiaChi;
			khToEdit.TinhThanhId = model.TinhThanhId;
			khToEdit.PhuongXaId = model.PhuongXaId;
			khToEdit.NgayCapNhat = DateTime.Now;

			context.KhachHangs.Update(khToEdit);
			if (Save())
				return new ReturnState() { Completed = true, Message = "Cập nhật thông tin khách hàng thành công!" };
			else
				return new ReturnState() { Completed = false, Message = "Cập nhật thông tin khách hàng thất bại! (Lỗi lưu cơ sở dữ liệu)" };

		}

		public IEnumerable<KhachHang> GetAll()
		{
			return context.KhachHangs.Include(k => k.User)
				.Include(k => k.TinhThanh)
				.Include(k => k.PhuongXa)
				.Where(k => !k.IsDeleted);
		}

		public IEnumerable<KhachHang> GetAllActive()
		{
			return context.KhachHangs.Include(k => k.User)
				.Include(k => k.TinhThanh).Include(k => k.PhuongXa)
				.Where(k => !k.IsDeleted && (k.User.LockoutEnd == null || k.User.LockoutEnd < DateTimeOffset.Now));
		}

		public IEnumerable<KhachHang> GetAllInactive()
		{
			return context.KhachHangs.Include(k => k.User)
				.Include(k => k.TinhThanh).Include(k => k.PhuongXa)
				.Where(k => !k.IsDeleted && (k.User.LockoutEnd == null || k.User.LockoutEnd >= DateTimeOffset.Now));
		}

		public async Task<KhachHang?> GetById(int? id)
		{
			if (id == null || id == 0) return null;
			return await context.KhachHangs.Include(k => k.User)
				.Include(k => k.TinhThanh).Include(k => k.PhuongXa)
				.FirstOrDefaultAsync(k => k.Id == id && !k.IsDeleted);
		}

		public bool Save()
		{
			return context.SaveChanges() > 0;
		}
	}
}
