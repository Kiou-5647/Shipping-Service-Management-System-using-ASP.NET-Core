using Microsoft.EntityFrameworkCore;
using Shipping.Data;
using Shipping.Models;
using Shipping.Models.Enums;
using Shipping.Utilities;
using Shipping.ViewModels;
using System.Text;

namespace Shipping.Repositories.DonHangService
{
	public class DonHangService : IDonHangService
	{

		private readonly ApplicationDbContext _context;
		private readonly Random _random = new Random();

		public DonHangService(ApplicationDbContext context)
		{
			_context = context;
		}


		public async Task<DonHangViewModel> CalculateShippingFee(DonHangViewModel model)
		{
			model.PhiGiaoHang = 0;
			model.TongPhiPhuTro = 0;
			var details = new List<string>();

			var configs = await _context.Configs.ToDictionaryAsync(c => c.Id, c => c.GiaTri);

			decimal L = model.ChieuDai < 10 ? 10 : model.ChieuDai;
			decimal W = model.ChieuRong < 10 ? 10 : model.ChieuRong;
			decimal H = model.ChieuCao < 10 ? 10 : model.ChieuCao;

			decimal dimenDivider = configs.ContainsKey("DIMEN_DIVIDER") ? configs["DIMEN_DIVIDER"] : 5000;
			decimal volWeight = (L * W * H) / dimenDivider;
			model.TrongLuongQD = volWeight;
			decimal chargeWeight = Math.Max(model.TrongLuongThuc, volWeight);
			Console.WriteLine("chargeW: " + chargeWeight);

			if (volWeight > model.TrongLuongThuc)
			{
				details.Add($"Quy đổi ({L}x{W}x{H}/{dimenDivider}): {volWeight:N2}kg");
			}

			var tinhGui = await _context.TinhThanhs.FindAsync(model.TinhGuiId);
			var tinhNhan = await _context.TinhThanhs.FindAsync(model.TinhNhanId);

			if (tinhGui != null && tinhNhan != null)
			{
				LoaiVungGia vungGia = LoaiVungGia.LienMien;
				if (model.TinhGuiId == model.TinhNhanId) vungGia = LoaiVungGia.NoiTinh;
				else if (tinhGui.VungMienId == tinhNhan.VungMienId) vungGia = LoaiVungGia.NoiMien;

				var cauTruc = await _context.CauTrucGiaCuocs
					.FirstOrDefaultAsync(c => c.LoaiDichVuId == model.LoaiDichVuId && c.LoaiVungGia == vungGia);
				var dichVu = await _context.LoaiDichVus.FindAsync(model.LoaiDichVuId);

				// --- TÍNH CƯỚC VẬN CHUYỂN CHÍNH ---
				if (cauTruc != null && dichVu != null)
				{
					decimal stepUnit = dichVu.DonViTangThem;
					decimal thresholdWeight = dichVu.MocCanNang;
					Console.WriteLine("thresholdW: " + thresholdWeight);

					// Tính số nấc (Luôn làm tròn lên)
					int totalSteps = (int)Math.Ceiling(chargeWeight / stepUnit);
					int thresholdSteps = (int)Math.Ceiling(thresholdWeight / stepUnit) - 1;

					if (chargeWeight < thresholdWeight)
					{
						// Dưới Mốc: Base + (Số nấc * Giá tăng thêm)
						decimal phiTangThem = totalSteps * cauTruc.GiaTangThem;
						model.PhiGiaoHang = cauTruc.GiaCoBan + phiTangThem;
						details.Add($"Cước chính: {cauTruc.GiaCoBan:N0} + ({totalSteps}x{cauTruc.GiaTangThem:N0})");
					}
					else
					{
						// Vượt Mốc: Giá Max đoạn dưới + (Số nấc vượt * Giá vượt)
						decimal giaDoanDau = cauTruc.GiaCoBan + (thresholdSteps * cauTruc.GiaTangThem);

						int stepsOver = totalSteps - thresholdSteps;
						decimal giaDoanVuot = stepsOver * cauTruc.GiaVuot;

						model.PhiGiaoHang = giaDoanDau + giaDoanVuot;
						details.Add($"Cước chính: {giaDoanDau:N0} (Mốc {thresholdWeight}kg) + {giaDoanVuot:N0} (Vượt {stepsOver} nấc)");
					}
				}
			}

			if (configs.TryGetValue("OVERSIZE_THRESHOLD", out decimal oversizeLimit))
			{
				decimal maxSide = Math.Max(L, Math.Max(W, H));
				decimal totalDim = L + W + H;

				if (maxSide > oversizeLimit && totalDim >= 100)
				{
					decimal oversizeFee = configs.ContainsKey("OVERSIZE_FEE") ? configs["OVERSIZE_FEE"] : 0;

					model.TongPhiPhuTro += oversizeFee;
					details.Add($"Phí quá khổ (Max {maxSide}cm > {oversizeLimit}cm): {oversizeFee:N0}");
				}
				else if (maxSide > oversizeLimit && totalDim < 100)
				{
					details.Add($"Miễn phí quá khổ: Tổng {totalDim}cm < {100}cm");
				}
			}


			if (configs.TryGetValue("INSURANCE_THRESHOLD", out decimal insThreshold))
			{
				if (model.COD >= insThreshold)
				{
					if (model.SuDungBaoHiem)
					{
						decimal insFee = configs.ContainsKey("INSURANCE_FEE") ? configs["INSURANCE_FEE"] : 0;

						model.TongPhiPhuTro += insFee;
						details.Add($"Phí bảo hiểm: {insFee:N0}");
					}
				}
			}

			if (model.PTThanhToan == PhuongThucThanhToan.ThuHo && model.COD > 0)
			{
				decimal codPercent = configs.ContainsKey("COD_FEE") ? configs["COD_FEE"] : 0;
				decimal codFee = model.COD * (codPercent / 100);

				model.TongPhiPhuTro += codFee;
				details.Add($"Phí COD ({codPercent}%): {codFee:N0}");
			}

			model.ChiTietPhi = string.Join(" | ", details);
			model.ThanhTien = model.PhiGiaoHang + model.TongPhiPhuTro + model.COD;

			return model;


		}

		public async Task<ReturnState> Cancel(string id, string lyDoHuy)
		{
			try
			{
				var order = await _context.DonHangs.FindAsync(id);
				if (order == null)
					return new ReturnState { Completed = false, Message = "Không tìm thấy đơn hàng." };

				if (order.TrangThaiDH == TrangThaiDonHang.DaGiao)
				{
					return new ReturnState { Completed = false, Message = "Không thể hủy đơn hàng đã giao thành công." };
				}

				order.TrangThaiDH = TrangThaiDonHang.ThatBai;
				order.GhiChu += $" | Đã hủy lúc {DateTime.Now:dd/MM/yyyy HH:mm}. Lý do: {lyDoHuy}";

				var shipments = _context.ChuyenHangs.Where(c => c.DonHangId == id);
				foreach (var s in shipments) s.TrangThai = TrangThaiDonHang.ThatBai;

				await _context.SaveChangesAsync();

				return new ReturnState { Completed = true, Message = "Đã hủy đơn hàng thành công." };
			}
			catch (Exception ex)
			{
				return new ReturnState { Completed = false, Message = "Lỗi khi hủy đơn: " + ex.Message };
			}
		}

		public async Task<ReturnState> Create(DonHangViewModel model)
		{
			try
			{
				var validationResult = await ValidateOrderConstraints(model);
				if (!validationResult.Completed)
				{
					return validationResult;
				}

				var calculatedModel = await CalculateShippingFee(model);

				string newId = await GenerateUniqueOrderId();

				string? imagePath = null;
				if (model.HinhAnhGoiHangFile != null)
				{
					string folderName = Path.Combine("donhang", newId);
					imagePath = Image.Upload(model.HinhAnhGoiHangFile, folderName);
				}

				var entity = new DonHang
				{
					Id = newId,
					NgayTao = DateTime.Now,
					NgayCapNhat = DateTime.Now,
					TrangThaiDH = TrangThaiDonHang.ChoXuLy,
					LoaiDichVuId = model.LoaiDichVuId,

					TTNguoiGui = model.TTNguoiGui,
					DiaChiGui = model.DiaChiGui,
					TinhGuiId = model.TinhGuiId,
					PhuongGuiId = model.PhuongGuiId,

					TTNguoiNhan = model.TTNguoiNhan,
					DiaChiNhan = model.DiaChiNhan,
					TinhNhanId = model.TinhNhanId,
					PhuongNhanId = model.PhuongNhanId,

					ChieuDai = model.ChieuDai,
					ChieuRong = model.ChieuRong,
					ChieuCao = model.ChieuCao,
					TrongLuongThuc = model.TrongLuongThuc,
					TrongLuongQuyDoi = model.TrongLuongQD,
					GhiChu = model.GhiChu,
					HinhAnhGoiHang = imagePath,

					COD = model.COD,
					PhiGiaoHang = calculatedModel.PhiGiaoHang,
					TongPhiPhuTro = calculatedModel.TongPhiPhuTro,
					ChiTietPhi = calculatedModel.ChiTietPhi,
					ThanhTien = calculatedModel.ThanhTien,

					PTThanhToan = model.PTThanhToan,
					TrangThaiTT = TrangThaiThanhToan.ChoThanhToan,
					KhachHangId = model.KhachHangId
				};

				await _context.DonHangs.AddAsync(entity);
				await _context.SaveChangesAsync();

				return new ReturnState
				{
					Completed = true,
					Message = "Tạo đơn hàng thành công!"
				};
			}
			catch (Exception ex)
			{
				return new ReturnState { Completed = false, Message = "Lỗi hệ thống: " + ex.Message };
			}
		}

		public async Task<ReturnState> Delete(string id)
		{
			try
			{
				var order = await _context.DonHangs.FindAsync(id);
				if (order == null)
					return new ReturnState { Completed = false, Message = "Không tìm thấy đơn hàng." };

				if (order.TrangThaiDH != TrangThaiDonHang.ChoXuLy)
				{
					return new ReturnState
					{
						Completed = false,
						Message = "Không thể xóa đơn hàng đã đi vào quy trình vận chuyển. Vui lòng sử dụng chức năng Hủy."
					};
				}

				bool hasShipment = await _context.ChuyenHangs.AnyAsync(c => c.DonHangId == id);
				if (hasShipment)
				{
					return new ReturnState
					{
						Completed = false,
						Message = "Đơn hàng đã được điều phối chuyến hàng. Không thể xóa, chỉ có thể Hủy."
					};
				}

				_context.DonHangs.Remove(order);

				await _context.SaveChangesAsync();
				return new ReturnState { Completed = true, Message = "Đã xóa đơn hàng thành công." };
			}
			catch (Exception ex)
			{
				return new ReturnState { Completed = false, Message = "Lỗi khi xóa: " + ex.Message };
			}
		}

		public IEnumerable<DonHang> GetAll()
		{
			return _context.DonHangs
				.Include(d => d.LoaiDichVu)
				.Include(d => d.TinhThanhGui)
				.Include(d => d.TinhThanhNhan)
				.Include(d => d.PhuongXaGui)
				.Include(d => d.PhuongXaNhan)
				.Include(d => d.KhachHang)
				.Include(d => d.Shipper)
				.Include(d => d.NhanVien)
				.OrderByDescending(d => d.NgayTao);
		}

		public async Task<DonHang?> GetById(string id)
		{
			return await _context.DonHangs.Include(d => d.LoaiDichVu)
				.Include(d => d.TinhThanhGui)
				.Include(d => d.TinhThanhNhan)
				.Include(d => d.PhuongXaGui)
				.Include(d => d.PhuongXaNhan)
				.Include(d => d.KhachHang)
				.Include(d => d.Shipper)
				.Include(d => d.NhanVien)
				.OrderByDescending(d => d.NgayTao)
				.FirstOrDefaultAsync(d => d.Id == id);
		}

		public async Task<IEnumerable<DonHang>> GetByKhachHangId(int khachHangId)
		{
			return await _context.DonHangs
				.Include(d => d.LoaiDichVu)
				.Include(d => d.TinhThanhGui)
				.Include(d => d.TinhThanhNhan)
				.Where(d => d.KhachHangId == khachHangId)
				.OrderByDescending(d => d.NgayTao).ToListAsync();
		}

		public async Task<ReturnState> UpdatePaymentStatus(string id, bool daThanhToan, string? paymentImage = null, int? nhanVienId = null, int? shipperId = null)
		{
			try
			{
				var order = await _context.DonHangs.FindAsync(id);
				if (order == null)
					return new ReturnState { Completed = false, Message = "Không tìm thấy đơn hàng." };

				order.TrangThaiTT = daThanhToan ? TrangThaiThanhToan.DaThanhToan : TrangThaiThanhToan.ChoThanhToan;
				order.NgayCapNhat = DateTime.Now;

				if (daThanhToan)
				{
					if (!string.IsNullOrEmpty(paymentImage))
					{
						order.HinhAnhXacNhan = paymentImage;
					}

					if (nhanVienId.HasValue)
					{
						order.NhanVienId = nhanVienId.Value;
						order.ShipperId = null;
					}
					else if (shipperId.HasValue)
					{
						order.ShipperId = shipperId.Value;
						order.NhanVienId = null;
					}
				}
				else
				{
					order.NhanVienId = null;
					order.ShipperId = null;
					order.HinhAnhXacNhan = null;
				}

				await _context.SaveChangesAsync();
				return new ReturnState { Completed = true, Message = "Cập nhật trạng thái thanh toán thành công." };
			}
			catch (Exception ex)
			{
				return new ReturnState { Completed = false, Message = "Lỗi cập nhật thanh toán: " + ex.Message };
			}
		}

		public async Task<ReturnState> ValidateOrderConstraints(DonHangViewModel model)
		{
			var configs = await _context.Configs.ToDictionaryAsync(c => c.Id, c => c.GiaTri);
			decimal L = model.ChieuDai < 10 ? 10 : model.ChieuDai;
			decimal W = model.ChieuRong < 10 ? 10 : model.ChieuRong;
			decimal H = model.ChieuCao < 10 ? 10 : model.ChieuCao;

			decimal dimenDivider = configs.ContainsKey("DIMEN_DIVIDER") ? configs["DIMEN_DIVIDER"] : 5000;
			decimal volWeight = (L * W * H) / dimenDivider;

			decimal checkWeight = Math.Max(model.TrongLuongThuc, volWeight);


			if (configs.TryGetValue("SIZE_LIMIT", out decimal sizeLimit))
			{

				if (L >= sizeLimit || W >= sizeLimit || H >= sizeLimit)
				{
					return new ReturnState
					{
						Completed = false,
						Message = $"Kích thước kiện hàng vượt quá giới hạn ({sizeLimit}cm)."
					};
				}
			}

			if (configs.TryGetValue("WEIGHT_LIMIT", out decimal weightLimit))
			{
				if (checkWeight > weightLimit)
				{
					return new ReturnState
					{
						Completed = false,
						Message = $"Trọng lượng vượt quá giới hạn cho phép ({weightLimit}kg)."
					};
				}
			}

			if (configs.TryGetValue("HEAVY_THRESHOLD", out decimal heavyLimit))
			{
				if (checkWeight >= heavyLimit)
				{
					var dichVu = await _context.LoaiDichVus.FindAsync(model.LoaiDichVuId);

					if (dichVu != null)
					{
						if (!dichVu.MaDV.ToUpper().Contains("HEAVY"))
						{
							string reason = checkWeight == volWeight
								? $"Hàng cồng kềnh (Quy đổi {checkWeight:N2}kg)"
								: $"Hàng nặng ({checkWeight:N2}kg)";

							return new ReturnState
							{
								Completed = false,
								Message = $"{reason} đạt ngưỡng {heavyLimit}kg. Vui lòng chọn dịch vụ vận chuyển Hàng Nặng (Mã HEAVY)."
							};
						}
					}
				}
			}

			return new ReturnState { Completed = true };
		}

		private async Task<string> GenerateUniqueOrderId()
		{
			const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
			string orderId;
			bool exists;

			do
			{
				StringBuilder sb = new StringBuilder();
				for (int i = 0; i < 10; i++)
				{
					sb.Append(chars[_random.Next(chars.Length)]);
				}
				orderId = sb.ToString();
				exists = await _context.DonHangs.AnyAsync(d => d.Id == orderId);
			} while (exists);

			return orderId;
		}
	}
}
