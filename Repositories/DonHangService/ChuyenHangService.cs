using Microsoft.EntityFrameworkCore;
using Shipping.Data;
using Shipping.Models;
using Shipping.Models.Enums;
using Shipping.Utilities;
using Shipping.ViewModels;

namespace Shipping.Repositories.DonHangService
{
	public class ChuyenHangService : IChuyenHangService
	{
		private readonly ApplicationDbContext _context;

		public ChuyenHangService(ApplicationDbContext context)
		{
			_context = context;
		}

		public async Task<ReturnState> Create(ChuyenHangViewModel model)
		{
			using var transaction = await _context.Database.BeginTransactionAsync();
			try
			{
				var donHang = await _context.DonHangs.FindAsync(model.DonHangId);
				if (donHang == null) return new ReturnState { Completed = false, Message = "Đơn hàng không tồn tại." };

				if (donHang.TrangThaiDH == TrangThaiDonHang.DaGiao || donHang.TrangThaiDH == TrangThaiDonHang.ThatBai)
					return new ReturnState { Completed = false, Message = "Không thể thêm chuyến cho đơn hàng đã kết thúc." };

				var maxThuTu = await _context.ChuyenHangs
					.Where(c => c.DonHangId == model.DonHangId)
					.MaxAsync(c => (int?)c.ThuTu) ?? 0;
				int nextThuTu = maxThuTu + 1;

				string dChiGui = "", tGui = "", pGui = "";
				string dChiNhan = "", tNhan = "", pNhan = "";

				if (model.LoaiDiemDi == LoaiDiaDiem.NguoiGui)
				{
					dChiGui = donHang.DiaChiGui;
					tGui = donHang.TinhGuiId;
					pGui = donHang.PhuongGuiId;
				}
				else if (model.LoaiDiemDi == LoaiDiaDiem.ChiNhanh && model.ChiNhanhDiId.HasValue)
				{
					var cn = await _context.ChiNhanhs.FindAsync(model.ChiNhanhDiId);
					if (cn == null) return new ReturnState { Completed = false, Message = "Chi nhánh đi không tồn tại." };
					dChiGui = cn.DiaChi;
					tGui = cn.TinhThanhId;
					pGui = cn.PhuongXaId;
				}
				else if (model.LoaiDiemDi == LoaiDiaDiem.TiepNoi)
				{
					if (maxThuTu == 0) return new ReturnState { Completed = false, Message = "Đây là chuyến đầu tiên, không thể chọn 'Tiếp nối'." };

					var prevLeg = await _context.ChuyenHangs.AsNoTracking()
						.FirstOrDefaultAsync(c => c.DonHangId == model.DonHangId && c.ThuTu == maxThuTu);

					if (prevLeg == null) return new ReturnState { Completed = false, Message = "Không tìm thấy chuyến trước." };

					dChiGui = prevLeg.DiaChiNhan;
					tGui = prevLeg.TinhNhanId;
					pGui = prevLeg.PhuongNhanId;
				}
				else
				{
					dChiGui = model.DiaChiGui ?? "";
					tGui = model.TinhGuiId ?? "";
					pGui = model.PhuongGuiId ?? "";
				}

				if (model.LoaiDiemDen == LoaiDiaDiem.NguoiNhan)
				{
					dChiNhan = donHang.DiaChiNhan;
					tNhan = donHang.TinhNhanId;
					pNhan = donHang.PhuongNhanId;
				}
				else if (model.LoaiDiemDen == LoaiDiaDiem.ChiNhanh && model.ChiNhanhDenId.HasValue)
				{
					var cn = await _context.ChiNhanhs.FindAsync(model.ChiNhanhDenId);
					if (cn == null) return new ReturnState { Completed = false, Message = "Chi nhánh đến không tồn tại." };
					dChiNhan = cn.DiaChi;
					tNhan = cn.TinhThanhId;
					pNhan = cn.PhuongXaId;
				}
				else
				{
					dChiNhan = model.DiaChiNhan ?? "";
					tNhan = model.TinhNhanId ?? "";
					pNhan = model.PhuongNhanId ?? "";
				}

				if (string.IsNullOrEmpty(tGui) || string.IsNullOrEmpty(pGui) ||
					string.IsNullOrEmpty(tNhan) || string.IsNullOrEmpty(pNhan))
				{
					return new ReturnState { Completed = false, Message = "Thiếu thông tin địa chỉ (Tỉnh/Phường). Vui lòng kiểm tra lại cấu hình Chi nhánh hoặc Đơn hàng." };
				}

				var entity = new ChuyenHang
				{
					DonHangId = model.DonHangId,
					ThuTu = nextThuTu,
					ShipperId = model.ShipperId,

					NgayTao = DateTime.Now,
					TrangThai = TrangThaiDonHang.DangGiao,
					ViTri = model.GhiChu,

					DiaChiGui = dChiGui,
					TinhGuiId = tGui,
					PhuongGuiId = pGui,

					DiaChiNhan = dChiNhan,
					TinhNhanId = tNhan,
					PhuongNhanId = pNhan,

					HinhAnhGiaoHang = null
				};

				await _context.ChuyenHangs.AddAsync(entity);

				if (donHang.TrangThaiDH == TrangThaiDonHang.ChoXuLy)
				{
					donHang.TrangThaiDH = TrangThaiDonHang.DangGiao;
					donHang.NgayCapNhat = DateTime.Now;
					_context.DonHangs.Update(donHang);
				}

				await _context.SaveChangesAsync();
				await transaction.CommitAsync();

				return new ReturnState { Completed = true, Message = "Phân công chuyến hàng thành công!" };
			}
			catch (Exception ex)
			{
				await transaction.RollbackAsync();
				return new ReturnState { Completed = false, Message = "Lỗi hệ thống: " + ex.Message };
			}
		}

		public async Task<ReturnState> Delete(string donHangId, int thuTu)
		{
			var chuyen = await _context.ChuyenHangs.FindAsync(donHangId, thuTu);
			if (chuyen == null) return new ReturnState { Completed = false, Message = "Không tìm thấy chuyến." };

			var maxThuTu = await _context.ChuyenHangs.Where(c => c.DonHangId == donHangId).MaxAsync(c => c.ThuTu);
			if (chuyen.ThuTu != maxThuTu)
			{
				return new ReturnState { Completed = false, Message = "Chỉ được phép xóa chuyến hàng mới nhất (cuối cùng)." };
			}

			_context.ChuyenHangs.Remove(chuyen);

			if (maxThuTu == 1)
			{
				var donHang = await _context.DonHangs.FindAsync(donHangId);
				if (donHang != null) donHang.TrangThaiDH = TrangThaiDonHang.ChoXuLy;
			}

			await _context.SaveChangesAsync();
			return new ReturnState { Completed = true, Message = "Đã xóa chuyến hàng." };
		}

		public async Task<IEnumerable<ChuyenHangViewModel>> GetByDonHangId(string donHangId)
		{
			var list = await _context.ChuyenHangs
				.Include(c => c.Shipper)
				.Include(c => c.TinhThanhGui)
				.Include(c => c.TinhThanhNhan)
				.Where(c => c.DonHangId == donHangId)
				.OrderBy(c => c.ThuTu)
				.Select(c => new ChuyenHangViewModel
				{
					DonHangId = c.DonHangId,
					ThuTu = c.ThuTu,
					ShipperId = c.ShipperId ?? 0,
					TenShipper = c.Shipper != null ? c.Shipper.Ten : "Chưa phân công",
					SDTShipper = c.Shipper != null ? c.Shipper.User.PhoneNumber : "",

					DiaChiGui = c.DiaChiGui,
					TinhGuiId = c.TinhGuiId,
					TenTinhGui = c.TinhThanhGui.TenTinhThanh,
					PhuongGuiId = c.PhuongGuiId,
					TenPhuongGui = c.PhuongXaGui.TenPhuongXa,

					DiaChiNhan = c.DiaChiNhan,
					TinhNhanId = c.TinhNhanId,
					TenTinhNhan = c.TinhThanhNhan.TenTinhThanh,
					PhuongNhanId = c.PhuongNhanId,
					TenPhuongNhan = c.PhuongXaNhan.TenPhuongXa,

					GhiChu = c.ViTri,
					TrangThai = c.TrangThai,
					NgayTao = c.NgayTao ?? DateTime.Now,
					HinhAnhGiaoHang = c.HinhAnhGiaoHang
				})
				.ToListAsync();

			return list;
		}

		public async Task<IEnumerable<ChuyenHangViewModel>> GetByShipperId(int shipperId)
		{
			var list = await _context.ChuyenHangs
			.AsNoTracking()
			.Include(c => c.TinhThanhGui)
			.Include(c => c.TinhThanhNhan)
			.Include(c => c.PhuongXaGui)
			.Include(c => c.PhuongXaNhan)
			.Include(c => c.DonHang)
			.Where(c => c.ShipperId == shipperId)
			.OrderByDescending(c => c.NgayTao)
			.Select(c => new ChuyenHangViewModel
			{
				DonHangId = c.DonHangId,
				ThuTu = c.ThuTu,
				ShipperId = c.ShipperId ?? 0,
				TrangThai = c.TrangThai,
				NgayTao = c.NgayTao ?? DateTime.Now,
				GhiChu = c.ViTri,

				DiaChiGui = c.DiaChiGui,
				TinhGuiId = c.TinhGuiId,
				TenTinhGui = c.TinhThanhGui.TenTinhThanh,
				PhuongGuiId = c.PhuongGuiId,
				TenPhuongGui = c.PhuongXaGui.TenPhuongXa,

				DiaChiNhan = c.DiaChiNhan,
				TinhNhanId = c.TinhNhanId,
				TenTinhNhan = c.TinhThanhNhan.TenTinhThanh,
				PhuongNhanId = c.PhuongNhanId,
				TenPhuongNhan = c.PhuongXaNhan.TenPhuongXa,

				COD = c.DonHang.COD,
				TongTien = c.DonHang.ThanhTien,
				TenNguoiNhanGoc = c.DonHang.TTNguoiNhan,
				GhiChuDonHang = c.DonHang.GhiChu
			})
			.ToListAsync();

			return list;
		}

		public async Task<string?> GetLastShipmentLocation(string donHangId)
		{
			var lastShip = await _context.ChuyenHangs
							.Where(c => c.DonHangId == donHangId)
							.OrderByDescending(c => c.ThuTu)
							.FirstOrDefaultAsync();

			if (lastShip != null) return lastShip.TinhNhanId;

			var dh = await _context.DonHangs.FindAsync(donHangId);
			return dh?.TinhGuiId;
		}

		public async Task<ReturnState> PhanCongChuyenHang(ChuyenHangViewModel model)
		{
			using var transaction = await _context.Database.BeginTransactionAsync();
			try
			{
				var donHang = await _context.DonHangs
					.FirstOrDefaultAsync(d => d.Id == model.DonHangId);

				if (donHang == null) return new ReturnState { Completed = false, Message = "Đơn hàng không tồn tại." };

				// --- 1. XỬ LÝ ĐIỂM ĐI (SOURCE) ---
				string idTinhGui = "", idPhuongGui = "", diaChiGui = "";

				// Chuyển Enum sang int để so sánh
				int valDiemDi = (int)model.LoaiDiemDi;

				switch (valDiemDi)
				{
					case 1: // Đơn gốc
						idTinhGui = donHang.TinhGuiId;
						idPhuongGui = donHang.PhuongGuiId;
						diaChiGui = donHang.DiaChiGui;
						break;

					case 3: // Chi nhánh
						if (model.ChiNhanhDiId.HasValue)
						{
							var cn = await _context.ChiNhanhs.FindAsync(model.ChiNhanhDiId);
							if (cn != null)
							{
								idTinhGui = cn.TinhThanhId;
								idPhuongGui = cn.PhuongXaId;
								diaChiGui = $"{cn.TenChiNhanh} - {cn.DiaChi}";
							}
						}
						break;

					case 4: // Tiếp nối chuyến trước
						var lastShip = await _context.ChuyenHangs
							.Where(c => c.DonHangId == model.DonHangId)
							.OrderByDescending(c => c.ThuTu)
							.FirstOrDefaultAsync();

						if (lastShip != null)
						{
							// Lấy điểm ĐẾN của chuyến trước làm điểm ĐI chuyến này
							idTinhGui = lastShip.TinhNhanId;
							idPhuongGui = lastShip.PhuongNhanId;
							diaChiGui = lastShip.DiaChiNhan;
						}
						else
						{
							// Fallback về đơn gốc
							idTinhGui = donHang.TinhGuiId;
							idPhuongGui = donHang.PhuongGuiId;
							diaChiGui = donHang.DiaChiGui;
						}
						break;

					case 5: // Nhập tay
							// Lưu ý: View phải đảm bảo gửi TinhGuiId và PhuongGuiId
						idTinhGui = model.TinhGuiId ?? "";
						idPhuongGui = model.PhuongGuiId ?? "00001"; // Default phường nếu thiếu
						diaChiGui = model.DiaChiGui ?? "";
						break;
				}

				// --- 2. XỬ LÝ ĐIỂM ĐẾN (DESTINATION) ---
				string idTinhNhan = "", idPhuongNhan = "", diaChiNhan = "";
				int valDiemDen = (int)model.LoaiDiemDen;

				switch (valDiemDen)
				{
					case 2: // Đơn gốc (Người nhận)
						idTinhNhan = donHang.TinhNhanId;
						idPhuongNhan = donHang.PhuongNhanId;
						diaChiNhan = donHang.DiaChiNhan;
						break;

					case 3: // Chi nhánh
						if (model.ChiNhanhDenId.HasValue)
						{
							var cn = await _context.ChiNhanhs.FindAsync(model.ChiNhanhDenId);
							if (cn != null)
							{
								idTinhNhan = cn.TinhThanhId;
								idPhuongNhan = cn.PhuongXaId;
								diaChiNhan = $"{cn.TenChiNhanh} - {cn.DiaChi}";
							}
						}
						break;

					case 5: // Nhập tay
						idTinhNhan = model.TinhNhanId ?? "";
						idPhuongNhan = model.PhuongNhanId ?? "00001";
						diaChiNhan = model.DiaChiNhan ?? "";
						break;
				}

				// Kiểm tra dữ liệu bắt buộc (Constraint khóa ngoại)
				if (string.IsNullOrEmpty(idTinhGui) || string.IsNullOrEmpty(idTinhNhan))
				{
					return new ReturnState { Completed = false, Message = "Thiếu thông tin Tỉnh/Thành phố." };
				}

				// --- 3. TẠO ENTITY ---
				var maxThuTu = await _context.ChuyenHangs
					.Where(c => c.DonHangId == model.DonHangId)
					.MaxAsync(c => (int?)c.ThuTu) ?? 0;

				var chuyenHang = new ChuyenHang
				{
					DonHangId = model.DonHangId,
					ShipperId = model.ShipperId,
					ThuTu = maxThuTu + 1,
					TrangThai = TrangThaiDonHang.DangGiao,
					NgayTao = DateTime.Now,
					GhiChu = model.GhiChu,

					// Lưu địa chỉ (Text)
					DiaChiGui = diaChiGui,
					DiaChiNhan = diaChiNhan,

					// Lưu khóa ngoại (IDs)
					TinhGuiId = idTinhGui,
					PhuongGuiId = idPhuongGui,
					TinhNhanId = idTinhNhan,
					PhuongNhanId = idPhuongNhan
				};

				_context.ChuyenHangs.Add(chuyenHang);

				// Cập nhật trạng thái đơn hàng chính
				if (donHang.TrangThaiDH == TrangThaiDonHang.ChoXuLy)
				{
					donHang.TrangThaiDH = TrangThaiDonHang.DangGiao;
					_context.DonHangs.Update(donHang);
				}

				await _context.SaveChangesAsync();
				await transaction.CommitAsync();

				return new ReturnState { Completed = true, Message = "Phân công thành công!" };
			}
			catch (Exception ex)
			{
				await transaction.RollbackAsync();
				// Log ex here
				return new ReturnState { Completed = false, Message = "Lỗi hệ thống: " + ex.Message };
			}
		}

		public async Task<ReturnState> UpdateStatus(string donHangId, int thuTu, TrangThaiDonHang trangThai, string? ghiChu, IFormFile? imageFile)
		{
			var chuyen = await _context.ChuyenHangs.FindAsync(donHangId, thuTu);
			if (chuyen == null) return new ReturnState { Completed = false, Message = "Không tìm thấy chuyến." };

			chuyen.TrangThai = trangThai;
			if (!string.IsNullOrEmpty(ghiChu)) chuyen.ViTri = ghiChu;

			if (imageFile != null)
			{
				string folderPath = Path.Combine("donhang", donHangId);
				string fileName = Image.Upload(imageFile, folderPath);
				chuyen.HinhAnhGiaoHang = fileName;
			}

			if (trangThai == TrangThaiDonHang.DaGiao)
			{
				chuyen.NgayHoanThanh = DateTime.Now;

				var maxThuTu = await _context.ChuyenHangs.Where(c => c.DonHangId == donHangId).MaxAsync(c => c.ThuTu);
				if (chuyen.ThuTu == maxThuTu)
				{
					var donHang = await _context.DonHangs.FindAsync(donHangId);
					if (donHang != null)
					{
						donHang.TrangThaiDH = TrangThaiDonHang.DaGiao;
						donHang.NgayCapNhat = DateTime.Now;
					}
				}
			}
			else if (trangThai == TrangThaiDonHang.ThatBai)
			{
				var donHang = await _context.DonHangs.FindAsync(donHangId);
				if (donHang != null)
				{
					donHang.TrangThaiDH = TrangThaiDonHang.ThatBai;
					donHang.NgayCapNhat = DateTime.Now;
					var index = thuTu + 1;
					while (true)
					{
						var chuyenToCancel = await _context.ChuyenHangs.FindAsync(donHangId, index);
						if (chuyenToCancel != null)
						{
							chuyenToCancel.TrangThai = TrangThaiDonHang.ThatBai;
							index++;
						}
						else
							break;
					}
				}
			}
			else if (trangThai == TrangThaiDonHang.DangGiao)
			{
				var donHang = await _context.DonHangs.FindAsync(donHangId);
				if (donHang != null)
				{
					donHang.TrangThaiDH = TrangThaiDonHang.DangGiao;
					donHang.NgayCapNhat = DateTime.Now;
					var index = thuTu + 1;
					while (true)
					{
						var chuyenToCancel = await _context.ChuyenHangs.FindAsync(donHangId, index);
						if (chuyenToCancel != null)
						{
							chuyenToCancel.TrangThai = TrangThaiDonHang.DangGiao;
							index++;
						}
						else
							break;
					}
				}
			}

			await _context.SaveChangesAsync();
			return new ReturnState { Completed = true, Message = "Cập nhật thành công." };
		}
	}
}
