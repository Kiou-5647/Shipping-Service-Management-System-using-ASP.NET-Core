using Microsoft.AspNetCore.Identity;
using Shipping.Models;
using Shipping.Models.Enums;
using Shipping.Seed;
using System.Text.Json;

namespace Shipping.Data.Seed
{
	public class DbInitializer
	{
		public static async Task SeedData(IServiceProvider serviceProvider)
		{
			var userManager = serviceProvider.GetRequiredService<UserManager<IdentityUser>>();
			var roleManager = serviceProvider.GetRequiredService<RoleManager<IdentityRole>>();
			var context = serviceProvider.GetRequiredService<ApplicationDbContext>();

			//Tạo Roles
			string[] roleNames = { "Admin", "NhanVien", "KhachHang", "Shipper" };

			foreach (var roleName in roleNames)
			{
				if (await roleManager.FindByNameAsync(roleName) == null)
				{
					await roleManager.CreateAsync(new IdentityRole(roleName));
				}
			}
			await context.SaveChangesAsync();

			//Tạo data hành chính
			var mienBacCode = new List<string> {
				"01", "04", "11", "42", "12", "20", "40", "22", "14", "38", "24", "31", "33", "15", "19", "08", "37", "25"
			};
			var mienTrungCode = new List<string>{
				"46", "48", "52", "56", "51", "44"
			};

			if (!context.VungMiens.Any())
			{
				context.VungMiens.AddRange(
					new VungMien { TenVungMien = "Miền Bắc" },
					new VungMien { TenVungMien = "Miền Trung" },
					new VungMien { TenVungMien = "Miền Nam" }
				);
				await context.SaveChangesAsync();
			}

			var mienBacId = context.VungMiens.First(vm => vm.TenVungMien == "Miền Bắc").Id;
			var mienTrungId = context.VungMiens.First(vm => vm.TenVungMien == "Miền Trung").Id;
			var mienNamId = context.VungMiens.First(vm => vm.TenVungMien == "Miền Nam").Id;

			if (!context.TinhThanhs.Any())
			{
				string jsonFilePath = "vn_only_simplified_json_generated_data_vn_units.json";

				if (!File.Exists(jsonFilePath))
				{
					throw new FileNotFoundException($"Không tìm thấy file JSON tại: {jsonFilePath}");
				}

				string jsonString = await File.ReadAllTextAsync(jsonFilePath);

				var provinces = JsonSerializer.Deserialize<List<TinhJson>>(jsonString);

				if (provinces == null)
				{
					throw new InvalidOperationException("Không thể giải mã dữ liệu JSON tỉnh thành.");
				}

				var newTinhThanhs = new List<TinhThanh>();
				var newPhuongXas = new List<PhuongXa>();

				foreach (var p in provinces)
				{
					int vungMienId;
					if (mienBacCode.Contains(p.Code))
					{
						vungMienId = mienBacId;
					}
					else if (mienTrungCode.Contains(p.Code))
					{
						vungMienId = mienTrungId;
					}
					else
					{
						vungMienId = mienNamId;
					}

					var tinhThanh = new TinhThanh
					{
						Id = p.Code,
						TenTinhThanh = p.FullName,
						VungMienId = vungMienId
					};
					newTinhThanhs.Add(tinhThanh);

					foreach (var w in p.Wards)
					{
						var phuongXa = new PhuongXa
						{
							Id = w.Code,
							TenPhuongXa = w.FullName,
							TinhThanhId = p.Code
						};
						newPhuongXas.Add(phuongXa);
					}
				}

				context.TinhThanhs.AddRange(newTinhThanhs);
				context.PhuongXas.AddRange(newPhuongXas);
				await context.SaveChangesAsync();
			}
			//Tạo chi nhánh
			if (!context.ChiNhanhs.Any())
			{
				var HaNoi = context.TinhThanhs.FirstOrDefault(x => x.TenTinhThanh == "Thành phố Hà Nội");
				var BaDinh = context.PhuongXas.FirstOrDefault(x => x.TenPhuongXa == "Phường Ba Đình");

				if (HaNoi != null && BaDinh != null)
				{
					context.Add(
						new ChiNhanh
						{
							TenChiNhanh = "Chi nhánh Ba Đình - Hà Nội",
							DiaChi = "Số 30/31 ngõ 135 Đội Cấn",
							TinhThanhId = HaNoi.Id,
							PhuongXaId = BaDinh.Id,
							SDT = "02463272299"
						}
					);
				}

				var DaNang = context.TinhThanhs.FirstOrDefault(x => x.TenTinhThanh == "Thành phố Đà Nẵng");
				var HaiChau = context.PhuongXas.FirstOrDefault(x => x.TenPhuongXa == "Phường Hải Châu");

				if (DaNang != null && HaiChau != null)
				{
					context.Add(
						new ChiNhanh
						{
							TenChiNhanh = "Chi nhánh Cầu Rồng - Đà Nẵng",
							DiaChi = "Số 129, Lê Đình Dương",
							TinhThanhId = DaNang.Id,
							PhuongXaId = HaiChau.Id,
							SDT = "02366526003"
						}
					);
				}

				var HoChiMinh = context.TinhThanhs.FirstOrDefault(x => x.TenTinhThanh == "Thành phố Hồ Chí Minh");
				var OngLanh = context.PhuongXas.FirstOrDefault(x => x.TenPhuongXa == "Phường Cầu Ông Lãnh");

				if (HoChiMinh != null && OngLanh != null)
				{
					context.Add(
						new ChiNhanh
						{
							TenChiNhanh = "Chi nhánh Hưng Phú - Hồ Chí Minh",
							DiaChi = "Số 24/2, Nguyễn Cảnh Chân",
							TinhThanhId = HoChiMinh.Id,
							PhuongXaId = OngLanh.Id,
							SDT = "0335917111"
						}
					);
				}
				await context.SaveChangesAsync();
			}

			// --- 1. Tạo người dùng Quản trị viên (Admin/NhanVien) ---
			if (await userManager.FindByNameAsync("adminhcm@gmail.com") == null)
			{
				string adminPassword = "Admin123";

				var adminUser = new IdentityUser
				{
					UserName = "adminhcm@gmail.com",
					Email = "adminhcm@gmail.com",
					PhoneNumber = "0911111111", // Sẽ dùng PhoneNumber này
					EmailConfirmed = true,
				};

				var result = await userManager.CreateAsync(adminUser, adminPassword);
				if (result.Succeeded)
				{
					await userManager.AddToRoleAsync(adminUser, "Admin");

					context.NhanViens.Add(new NhanVien
					{
						UserId = adminUser.Id,
						Ten = "Quản trị viên",
						CCCD = "080000000000",
						TinhThanhId = "79",
						PhuongXaId = "26758",
						DiaChi = "123 Admin",
						

						IsQuanLy = true,
						NgaySinh = DateTime.Now.AddYears(-40),
						GioiTinh = true
					});
				}
				await context.SaveChangesAsync();
			}

			// --- 2. Tạo người dùng Khách hàng Cá nhân (KhachHang) ---
			if (await userManager.FindByNameAsync("khcn@gmail.com") == null)
			{
				string khPassword = "Khachhang123";

				var khUser = new IdentityUser
				{
					UserName = "khcn@gmail.com",
					Email = "khcn@gmail.com",
					PhoneNumber = "0922222222",
					EmailConfirmed = true,
				};

				var result = await userManager.CreateAsync(khUser, khPassword);
				if (result.Succeeded)
				{
					await userManager.AddToRoleAsync(khUser, "KhachHang");

					context.KhachHangs.Add(new KhachHang
					{
						UserId = khUser.Id,
						Ten = "Khách hàng cá nhân",
						CCCD = "083333333333",
						TinhThanhId = "79",
						PhuongXaId = "26758",
						DiaChi = "123 Khách Hàng",
						LoaiKhachHang = LoaiKhachHang.CaNhan,
						MaSoThue = null
					});
				}
				await context.SaveChangesAsync();
			}

			// --- 3. Tạo người dùng Khách hàng Doanh nghiệp (KhachHang) ---
			if (await userManager.FindByNameAsync("khdn@gmail.com") == null)
			{
				string khPassword = "Khachhang123";

				var khUser = new IdentityUser
				{
					UserName = "khdn@gmail.com",
					Email = "khdn@gmail.com",
					PhoneNumber = "0933333333",
					EmailConfirmed = true,
				};

				var result = await userManager.CreateAsync(khUser, khPassword);
				if (result.Succeeded)
				{
					await userManager.AddToRoleAsync(khUser, "KhachHang");

					context.KhachHangs.Add(new KhachHang
					{
						UserId = khUser.Id,
						Ten = "Khách hàng doanh nghiệp",
						CCCD = null,
						TinhThanhId = "79",
						PhuongXaId = "26758",
						DiaChi = "123 Khách Hàng",
						

						LoaiKhachHang = LoaiKhachHang.DoanhNghiep,
						MaSoThue = "1234567890"
					});
				}
				await context.SaveChangesAsync();
			}

			// --- 4. Tạo người dùng Nhân viên (NhanVien) ---
			if (await userManager.FindByNameAsync("nhanvien@gmail.com") == null)
			{
				string nvPassword = "Nhanvien123";

				var nvUser = new IdentityUser
				{
					UserName = "nhanvien@gmail.com",
					Email = "nhanvien@gmail.com",
					PhoneNumber = "0944444444",
					EmailConfirmed = true,
				};
				var result = await userManager.CreateAsync(nvUser, nvPassword);
				if (result.Succeeded)
				{
					var cnId = context.ChiNhanhs.FirstOrDefault(c => c.TenChiNhanh == "Chi nhánh Hưng Phú - Hồ Chí Minh")?.Id;

					await userManager.AddToRoleAsync(nvUser, "NhanVien");

					context.NhanViens.Add(new NhanVien
					{
						UserId = nvUser.Id,
						Ten = "Nhân viên",
						CCCD = "081111111111",
						TinhThanhId = "79",
						PhuongXaId = "26758",
						DiaChi = "123 Nhân Viên",
						

						IsQuanLy = false,
						ChiNhanhId = (cnId == null) ? null : cnId,
						NgaySinh = DateTime.Now.AddYears(-28),
						GioiTinh = true
					});
				}
				await context.SaveChangesAsync();
			}

			// --- 5. Tạo người dùng Shipper Giao Hàng (Shipper) ---
			if (await userManager.FindByNameAsync("shippergh@gmail.com") == null)
			{
				string spPassword = "Shipper123";

				var spUser = new IdentityUser
				{
					UserName = "shippergh@gmail.com",
					Email = "shippergh@gmail.com",
					PhoneNumber = "0955555555",
					EmailConfirmed = true,
				};

				var result = await userManager.CreateAsync(spUser, spPassword);
				if (result.Succeeded)
				{
					var cnId = context.ChiNhanhs.FirstOrDefault(c => c.TenChiNhanh == "Chi nhánh Hưng Phú - Hồ Chí Minh")?.Id;

					await userManager.AddToRoleAsync(spUser, "Shipper");

					context.Shippers.Add(new Shipper
					{
						UserId = spUser.Id,
						Ten = "Shipper giao hàng",
						CCCD = "082222222222",
						TinhThanhId = "79",
						PhuongXaId = "26758",
						DiaChi = "123 Shipper",

						ChiNhanhId = (cnId == null) ? null : cnId,
						NgaySinh = DateTime.Now.AddYears(-28),
						GioiTinh = true,
						LoaiShipper = LoaiShipper.GiaoHang,
						BienSoXe = "67AA-11111",
						TrangThai = TrangThaiShipper.Nghi
					});
				}
				await context.SaveChangesAsync();
			}

			// --- 6. Tạo người dùng Shipper Vận Chuyển (Shipper) ---
			if (await userManager.FindByNameAsync("shippervc@gmail.com") == null)
			{
				string spPassword = "Shipper123";

				var spUser = new IdentityUser
				{
					UserName = "shippervc@gmail.com",
					Email = "shippervc@gmail.com",
					PhoneNumber = "0966666666",
					EmailConfirmed = true,
				};

				var result = await userManager.CreateAsync(spUser, spPassword);
				if (result.Succeeded)
				{
					var cnId = context.ChiNhanhs.FirstOrDefault(c => c.TenChiNhanh == "Chi nhánh Hưng Phú - Hồ Chí Minh")?.Id;

					await userManager.AddToRoleAsync(spUser, "Shipper");

					context.Shippers.Add(new Shipper
					{
						UserId = spUser.Id,
						Ten = "Shipper vận chuyển",
						CCCD = "084444444444",
						TinhThanhId = "79",
						PhuongXaId = "26758",
						DiaChi = "123 Shipper",
						

						ChiNhanhId = (cnId == null) ? null : cnId,
						NgaySinh = DateTime.Now.AddYears(-25),
						GioiTinh = true,
						LoaiShipper = LoaiShipper.VanChuyen,
						BienSoXe = "67C-11138",
						TrangThai = TrangThaiShipper.Nghi
					});
				}
				await context.SaveChangesAsync();
			}

			if (!context.LoaiDichVus.Any())
			{
				context.LoaiDichVus.AddRange(
					// 1. Giao hàng Tiêu Chuẩn (Standard)
					new LoaiDichVu
					{
						TenDichVu = "Giao hàng Tiêu Chuẩn",
						MaDV = "STD",
						DonViTangThem = 0.5M,
						MocCanNang = 3,
						ThoiGianToiThieu = 2,
						ThoiGianToiDa = 5,
						MoTa = "Dịch vụ kinh tế, áp dụng cho mọi khu vực."
					},
					// 2. Giao hàng Nhanh (Fast)
					new LoaiDichVu
					{
						TenDichVu = "Giao hàng Nhanh",
						MaDV = "FAST",
						DonViTangThem = 0.5M,
						MocCanNang = 2,
						ThoiGianToiThieu = 1,
						ThoiGianToiDa = 3,
						MoTa = "Dịch vụ ưu tiên, giao hàng trong vòng 72 giờ."
					},
					// 3. Giao hàng Hỏa Tốc (Express)
					new LoaiDichVu
					{
						TenDichVu = "Giao hàng Hỏa Tốc",
						MaDV = "EXPRESS",
						DonViTangThem = 0.5M,
						MocCanNang = 2,
						ThoiGianToiThieu = 1,
						ThoiGianToiDa = 2,
						MoTa = "Dịch vụ giao hàng cấp tốc dưới 48 giờ."
					},
					// 4. Giao hàng Hàng Nặng (Heavy)
					new LoaiDichVu
					{
						TenDichVu = "Vận chuyển Hàng Nặng/Cồng Kềnh",
						MaDV = "HEAVY",
						DonViTangThem = 1M,
						MocCanNang = 15,
						ThoiGianToiThieu = 2,
						ThoiGianToiDa = 7,
						MoTa = "Dịch vụ đặc biệt dành cho hàng hóa nặng."
					}
				);
				await context.SaveChangesAsync();
			}

			//Tạo giá dịch vụ
			var standard = context.LoaiDichVus.First(ldv => ldv.MaDV == "STD");

			if (!context.CauTrucGiaCuocs.Any(gc => gc.LoaiDichVuId == standard.Id))
			{
				var giaCuocs = new List<CauTrucGiaCuoc>();

				//Noi tinh
				giaCuocs.Add(new CauTrucGiaCuoc
				{
					LoaiDichVuId = standard.Id,
					LoaiVungGia = LoaiVungGia.NoiTinh,
					GiaCoBan = 16500,
					GiaTangThem = 0,
					GiaVuot = 2500,
					ThoiGianGiao = 1
				});

				//Noi mien
				giaCuocs.Add(new CauTrucGiaCuoc
				{
					LoaiDichVuId = standard.Id,
					LoaiVungGia = LoaiVungGia.NoiMien,
					GiaCoBan = 30000,
					GiaTangThem = 3000,
					GiaVuot = 3000,
					ThoiGianGiao = 3
				});

				//Lien mien
				giaCuocs.Add(new CauTrucGiaCuoc
				{
					LoaiDichVuId = standard.Id,
					LoaiVungGia = LoaiVungGia.LienMien,
					GiaCoBan = 32000,
					GiaTangThem = 5000,
					GiaVuot = 5000,
					ThoiGianGiao = 5
				});

				context.CauTrucGiaCuocs.AddRange(giaCuocs);
				await context.SaveChangesAsync();
			}

			var fast = context.LoaiDichVus.First(ldv => ldv.MaDV == "FAST");

			if (!context.CauTrucGiaCuocs.Any(gc => gc.LoaiDichVuId == fast.Id))
			{
				var giaCuocs = new List<CauTrucGiaCuoc>();

				//Noi tinh
				giaCuocs.Add(new CauTrucGiaCuoc
				{
					LoaiDichVuId = fast.Id,
					LoaiVungGia = LoaiVungGia.NoiTinh,
					GiaCoBan = 17000,
					GiaTangThem = 4000,
					GiaVuot = 4000,
					ThoiGianGiao = 1
				});

				//Noi mien
				giaCuocs.Add(new CauTrucGiaCuoc
				{
					LoaiDichVuId = fast.Id,
					LoaiVungGia = LoaiVungGia.NoiMien,
					GiaCoBan = 31000,
					GiaTangThem = 11000,
					GiaVuot = 5000,
					ThoiGianGiao = 2
				});

				//Lien mien
				giaCuocs.Add(new CauTrucGiaCuoc
				{
					LoaiDichVuId = fast.Id,
					LoaiVungGia = LoaiVungGia.LienMien,
					GiaCoBan = 38000,
					GiaTangThem = 16000,
					GiaVuot = 12000,
					ThoiGianGiao = 3
				});

				context.CauTrucGiaCuocs.AddRange(giaCuocs);
				await context.SaveChangesAsync();
			}

			var express = context.LoaiDichVus.First(ldv => ldv.MaDV == "EXPRESS");

			if (!context.CauTrucGiaCuocs.Any(gc => gc.LoaiDichVuId == express.Id))
			{
				var giaCuocs = new List<CauTrucGiaCuoc>();

				//Noi tinh
				giaCuocs.Add(new CauTrucGiaCuoc
				{
					LoaiDichVuId = express.Id,
					LoaiVungGia = LoaiVungGia.NoiTinh,
					GiaCoBan = 38000,
					GiaTangThem = 0,
					GiaVuot = 4000,
					ThoiGianGiao = 1
				});

				//Noi mien
				giaCuocs.Add(new CauTrucGiaCuoc
				{
					LoaiDichVuId = express.Id,
					LoaiVungGia = LoaiVungGia.NoiMien,
					GiaCoBan = 127000,
					GiaTangThem = 0,
					GiaVuot = 11000,
					ThoiGianGiao = 1
				});

				//Lien mien
				giaCuocs.Add(new CauTrucGiaCuoc
				{
					LoaiDichVuId = express.Id,
					LoaiVungGia = LoaiVungGia.LienMien,
					GiaCoBan = 216000,
					GiaTangThem = 0,
					GiaVuot = 17000,
					ThoiGianGiao = 2
				});

				context.CauTrucGiaCuocs.AddRange(giaCuocs);
				await context.SaveChangesAsync();
			}

			var heavy = context.LoaiDichVus.First(ldv => ldv.MaDV == "HEAVY");

			if (!context.CauTrucGiaCuocs.Any(gc => gc.LoaiDichVuId == heavy.Id))
			{
				var giaCuocs = new List<CauTrucGiaCuoc>();

				//Noi tinh
				giaCuocs.Add(new CauTrucGiaCuoc
				{
					LoaiDichVuId = heavy.Id,
					LoaiVungGia = LoaiVungGia.NoiTinh,
					GiaCoBan = 76000,
					GiaTangThem = 0,
					GiaVuot = 5000,
					ThoiGianGiao = 2
				});

				//Noi mien
				giaCuocs.Add(new CauTrucGiaCuoc
				{
					LoaiDichVuId = heavy.Id,
					LoaiVungGia = LoaiVungGia.NoiMien,
					GiaCoBan = 117000,
					GiaTangThem = 0,
					GiaVuot = 7000,
					ThoiGianGiao = 5
				});

				//Lien mien
				giaCuocs.Add(new CauTrucGiaCuoc
				{
					LoaiDichVuId = heavy.Id,
					LoaiVungGia = LoaiVungGia.LienMien,
					GiaCoBan = 177000,
					GiaTangThem = 0,
					GiaVuot = 9000,
					ThoiGianGiao = 7
				});

				context.CauTrucGiaCuocs.AddRange(giaCuocs);
				await context.SaveChangesAsync();
			}

			if (!context.Configs.Any(c => c.Loai == LoaiConfig.LuaChon))
			{
				context.Configs.AddRange(
					new Config
					{
						Id = "COD_FEE",
						Ten = "Phí Thu Hộ (COD)",
						Loai = LoaiConfig.TuDong,
						LoaiGiaTri = LoaiGiaTri.Percent,
						GiaTri = 1,
						MoTa = "Phí thu hộ tiền hàng, tính 1% trên tổng giá trị COD.",
					},

					new Config
					{
						Id = "INSURANCE_FEE",
						Ten = "Phí Bảo Hiểm Hàng Hóa",
						Loai = LoaiConfig.LuaChon,
						LoaiGiaTri = LoaiGiaTri.Fixed,
						GiaTri = 50000,
						MoTa = "Phí bảo hiểm áp dụng cho giá trị hàng hóa giá trị cao.",
					}, 
					
					new Config
					{
						Id = "INSURANCE_THRESHOLD",
						Ten = "Ngưỡng bảo hiểm hàng hóa",
						Loai = LoaiConfig.LuaChon,
						LoaiGiaTri = LoaiGiaTri.Fixed,
						GiaTri = 5000000,
						MoTa = "Ngưỡng áp dụng bảo hiểm hàng hóa",
					},

					new Config
					{
						Id = "OVERSIZE_FEE",
						Ten = "Phụ phí Hàng Quá Khổ",
						Loai = LoaiConfig.TuDong,
						LoaiGiaTri = LoaiGiaTri.Fixed,
						GiaTri = 30000,
						MoTa = "Phí xử lý bổ sung cho các bưu kiện có kích thước lớn hơn quy định thông thường.",
					},

					new Config
					{
						Id = "OVERSIZE_THRESHOLD",
						Ten = "Ngưỡng Hàng Quá Khổ",
						Loai = LoaiConfig.TuDong,
						LoaiGiaTri = LoaiGiaTri.Length,
						GiaTri = 100,
						MoTa = "Ngưỡng kích cỡ cho tổng kích thước 3 chiều của một đơn hàng!",
					},

					new Config
					{
						Id = "HEAVY_THRESHOLD",
						Ten = "Ngưỡng trọng lượng hàng nặng",
						Loai = LoaiConfig.TuDong,
						LoaiGiaTri = LoaiGiaTri.Mass,
						GiaTri = 15,
						MoTa = "Ngưỡng cân nặng (kg) được xem là hàng nặng.",
					},

					new Config
					{
						Id = "WEIGHT_LIMIT",
						Ten = "Giới hạn cân nặng",
						Loai = LoaiConfig.TuDong,
						LoaiGiaTri = LoaiGiaTri.Mass,
						GiaTri = 300,
						MoTa = "Giới hạn cân nặng hợp lệ (kg) của đơn hàng.",
					},

					new Config
					{
						Id = "DIMEN_DIVIDER",
						Ten = "Hệ số không gian cho trọng lượng",
						Loai = LoaiConfig.TuDong,
						LoaiGiaTri = LoaiGiaTri.None,
						GiaTri = 5000,
						MoTa = "Hệ số không gian để tính trọng lượng quy đổi cho đơn hàng",
					},

					new Config
					{
						Id = "SIZE_LIMIT",
						Ten = "Kích thước giới hạn",
						Loai = LoaiConfig.TuDong,
						LoaiGiaTri = LoaiGiaTri.Length,
						GiaTri = 300,
						MoTa = "Kích thước giới hạn hợp lệ của một đơn hàng (cm)",
					}
				);
				await context.SaveChangesAsync();
			}
		}
	}
}
