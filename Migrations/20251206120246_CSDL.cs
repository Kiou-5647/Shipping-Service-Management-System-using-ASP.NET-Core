using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Shipping.Migrations
{
    /// <inheritdoc />
    public partial class CSDL : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "AspNetRoles",
                columns: table => new
                {
                    Id = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    NormalizedName = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    ConcurrencyStamp = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetRoles", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUsers",
                columns: table => new
                {
                    Id = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    UserName = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    NormalizedUserName = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    Email = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    NormalizedEmail = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    EmailConfirmed = table.Column<bool>(type: "bit", nullable: false),
                    PasswordHash = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    SecurityStamp = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ConcurrencyStamp = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PhoneNumber = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PhoneNumberConfirmed = table.Column<bool>(type: "bit", nullable: false),
                    TwoFactorEnabled = table.Column<bool>(type: "bit", nullable: false),
                    LockoutEnd = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true),
                    LockoutEnabled = table.Column<bool>(type: "bit", nullable: false),
                    AccessFailedCount = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUsers", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Configs",
                columns: table => new
                {
                    Id = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Ten = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Loai = table.Column<int>(type: "int", nullable: false),
                    LoaiGiaTri = table.Column<int>(type: "int", nullable: false),
                    GiaTri = table.Column<decimal>(type: "decimal(12,2)", nullable: false),
                    MoTa = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Configs", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "LoaiDichVus",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TenDichVu = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    MaDV = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    MoTa = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    DonViTangThem = table.Column<decimal>(type: "decimal(12,2)", nullable: false),
                    MocCanNang = table.Column<decimal>(type: "decimal(4,2)", nullable: false),
                    ThoiGianToiThieu = table.Column<int>(type: "int", nullable: false),
                    ThoiGianToiDa = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LoaiDichVus", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "VungMiens",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TenVungMien = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_VungMiens", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "AspNetRoleClaims",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    RoleId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    ClaimType = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ClaimValue = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetRoleClaims", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AspNetRoleClaims_AspNetRoles_RoleId",
                        column: x => x.RoleId,
                        principalTable: "AspNetRoles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUserClaims",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    ClaimType = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ClaimValue = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUserClaims", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AspNetUserClaims_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUserLogins",
                columns: table => new
                {
                    LoginProvider = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    ProviderKey = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    ProviderDisplayName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UserId = table.Column<string>(type: "nvarchar(450)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUserLogins", x => new { x.LoginProvider, x.ProviderKey });
                    table.ForeignKey(
                        name: "FK_AspNetUserLogins_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUserRoles",
                columns: table => new
                {
                    UserId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    RoleId = table.Column<string>(type: "nvarchar(450)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUserRoles", x => new { x.UserId, x.RoleId });
                    table.ForeignKey(
                        name: "FK_AspNetUserRoles_AspNetRoles_RoleId",
                        column: x => x.RoleId,
                        principalTable: "AspNetRoles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_AspNetUserRoles_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUserTokens",
                columns: table => new
                {
                    UserId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    LoginProvider = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Value = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUserTokens", x => new { x.UserId, x.LoginProvider, x.Name });
                    table.ForeignKey(
                        name: "FK_AspNetUserTokens_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "CauTrucGiaCuocs",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    LoaiVungGia = table.Column<int>(type: "int", nullable: false),
                    GiaCoBan = table.Column<decimal>(type: "decimal(12,0)", nullable: false),
                    GiaTangThem = table.Column<decimal>(type: "decimal(12,0)", nullable: false),
                    GiaVuot = table.Column<decimal>(type: "decimal(12,0)", nullable: false),
                    ThoiGianGiao = table.Column<int>(type: "int", nullable: false),
                    LoaiDichVuId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CauTrucGiaCuocs", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CauTrucGiaCuocs_LoaiDichVus_LoaiDichVuId",
                        column: x => x.LoaiDichVuId,
                        principalTable: "LoaiDichVus",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "TinhThanhs",
                columns: table => new
                {
                    Id = table.Column<string>(type: "nvarchar(10)", nullable: false),
                    TenTinhThanh = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    VungMienId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TinhThanhs", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TinhThanhs_VungMiens_VungMienId",
                        column: x => x.VungMienId,
                        principalTable: "VungMiens",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "PhuongXas",
                columns: table => new
                {
                    Id = table.Column<string>(type: "nvarchar(10)", nullable: false),
                    TenPhuongXa = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    TinhThanhId = table.Column<string>(type: "nvarchar(10)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PhuongXas", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PhuongXas_TinhThanhs_TinhThanhId",
                        column: x => x.TinhThanhId,
                        principalTable: "TinhThanhs",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "ChiNhanhs",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TenChiNhanh = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    DiaChi = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    SDT = table.Column<string>(type: "nvarchar(15)", maxLength: 15, nullable: false),
                    TinhThanhId = table.Column<string>(type: "nvarchar(10)", nullable: false),
                    PhuongXaId = table.Column<string>(type: "nvarchar(10)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ChiNhanhs", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ChiNhanhs_PhuongXas_PhuongXaId",
                        column: x => x.PhuongXaId,
                        principalTable: "PhuongXas",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ChiNhanhs_TinhThanhs_TinhThanhId",
                        column: x => x.TinhThanhId,
                        principalTable: "TinhThanhs",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "KhachHangs",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Ten = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    CCCD = table.Column<string>(type: "nvarchar(20)", nullable: true),
                    MaSoThue = table.Column<string>(type: "nvarchar(20)", nullable: true),
                    LoaiKhachHang = table.Column<int>(type: "int", nullable: false),
                    DiaChi = table.Column<string>(type: "nvarchar(200)", nullable: false),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    NgayCapNhat = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UserId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    TinhThanhId = table.Column<string>(type: "nvarchar(10)", nullable: false),
                    PhuongXaId = table.Column<string>(type: "nvarchar(10)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_KhachHangs", x => x.Id);
                    table.ForeignKey(
                        name: "FK_KhachHangs_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_KhachHangs_PhuongXas_PhuongXaId",
                        column: x => x.PhuongXaId,
                        principalTable: "PhuongXas",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_KhachHangs_TinhThanhs_TinhThanhId",
                        column: x => x.TinhThanhId,
                        principalTable: "TinhThanhs",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "NhanViens",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Ten = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    CCCD = table.Column<string>(type: "nvarchar(20)", nullable: false),
                    GioiTinh = table.Column<bool>(type: "bit", nullable: false),
                    NgaySinh = table.Column<DateTime>(type: "date", nullable: false),
                    DiaChi = table.Column<string>(type: "nvarchar(200)", nullable: false),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    IsQuanLy = table.Column<bool>(type: "bit", nullable: false),
                    HinhAnh = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    NgayCapNhat = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UserId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    ChiNhanhId = table.Column<int>(type: "int", nullable: true),
                    TinhThanhId = table.Column<string>(type: "nvarchar(10)", nullable: false),
                    PhuongXaId = table.Column<string>(type: "nvarchar(10)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_NhanViens", x => x.Id);
                    table.ForeignKey(
                        name: "FK_NhanViens_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_NhanViens_ChiNhanhs_ChiNhanhId",
                        column: x => x.ChiNhanhId,
                        principalTable: "ChiNhanhs",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_NhanViens_PhuongXas_PhuongXaId",
                        column: x => x.PhuongXaId,
                        principalTable: "PhuongXas",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_NhanViens_TinhThanhs_TinhThanhId",
                        column: x => x.TinhThanhId,
                        principalTable: "TinhThanhs",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Shippers",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Ten = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    CCCD = table.Column<string>(type: "nvarchar(20)", nullable: false),
                    GioiTinh = table.Column<bool>(type: "bit", nullable: false),
                    NgaySinh = table.Column<DateTime>(type: "date", nullable: false),
                    DiaChi = table.Column<string>(type: "nvarchar(200)", nullable: false),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    LoaiShipper = table.Column<int>(type: "int", nullable: false),
                    TrangThai = table.Column<int>(type: "int", nullable: false),
                    BienSoXe = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    HinhAnh = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    ViTri = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    NgayCapNhat = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UserId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    ChiNhanhId = table.Column<int>(type: "int", nullable: true),
                    TinhThanhId = table.Column<string>(type: "nvarchar(10)", nullable: false),
                    PhuongXaId = table.Column<string>(type: "nvarchar(10)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Shippers", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Shippers_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Shippers_ChiNhanhs_ChiNhanhId",
                        column: x => x.ChiNhanhId,
                        principalTable: "ChiNhanhs",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Shippers_PhuongXas_PhuongXaId",
                        column: x => x.PhuongXaId,
                        principalTable: "PhuongXas",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Shippers_TinhThanhs_TinhThanhId",
                        column: x => x.TinhThanhId,
                        principalTable: "TinhThanhs",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "DonHangs",
                columns: table => new
                {
                    Id = table.Column<string>(type: "nvarchar(10)", nullable: false),
                    NgayTao = table.Column<DateTime>(type: "datetime2", nullable: false),
                    NgayCapNhat = table.Column<DateTime>(type: "datetime2", nullable: false),
                    TrangThaiDH = table.Column<int>(type: "int", nullable: false),
                    TTNguoiGui = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    DiaChiGui = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    TTNguoiNhan = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    DiaChiNhan = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ChieuDai = table.Column<decimal>(type: "decimal(12,2)", nullable: false),
                    ChieuRong = table.Column<decimal>(type: "decimal(12,2)", nullable: false),
                    ChieuCao = table.Column<decimal>(type: "decimal(12,2)", nullable: false),
                    TrongLuongThuc = table.Column<decimal>(type: "decimal(12,2)", nullable: false),
                    TrongLuongQuyDoi = table.Column<decimal>(type: "decimal(12,2)", nullable: false),
                    COD = table.Column<decimal>(type: "decimal(12,0)", nullable: false),
                    PhiGiaoHang = table.Column<decimal>(type: "decimal(12,0)", nullable: false),
                    TongPhiPhuTro = table.Column<decimal>(type: "decimal(12,0)", nullable: false),
                    ChiTietPhi = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ThanhTien = table.Column<decimal>(type: "decimal(12,0)", nullable: false),
                    PTThanhToan = table.Column<int>(type: "int", nullable: false),
                    TrangThaiTT = table.Column<int>(type: "int", nullable: false),
                    HinhAnhGoiHang = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    HinhAnhXacNhan = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    GhiChu = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ViTri = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    KhachHangId = table.Column<int>(type: "int", nullable: true),
                    NhanVienId = table.Column<int>(type: "int", nullable: true),
                    ShipperId = table.Column<int>(type: "int", nullable: true),
                    LoaiDichVuId = table.Column<int>(type: "int", nullable: false),
                    TinhGuiId = table.Column<string>(type: "nvarchar(10)", nullable: false),
                    PhuongGuiId = table.Column<string>(type: "nvarchar(10)", nullable: false),
                    TinhNhanId = table.Column<string>(type: "nvarchar(10)", nullable: false),
                    PhuongNhanId = table.Column<string>(type: "nvarchar(10)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DonHangs", x => x.Id);
                    table.ForeignKey(
                        name: "FK_DonHangs_KhachHangs_KhachHangId",
                        column: x => x.KhachHangId,
                        principalTable: "KhachHangs",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_DonHangs_LoaiDichVus_LoaiDichVuId",
                        column: x => x.LoaiDichVuId,
                        principalTable: "LoaiDichVus",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_DonHangs_NhanViens_NhanVienId",
                        column: x => x.NhanVienId,
                        principalTable: "NhanViens",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_DonHangs_PhuongXas_PhuongGuiId",
                        column: x => x.PhuongGuiId,
                        principalTable: "PhuongXas",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_DonHangs_PhuongXas_PhuongNhanId",
                        column: x => x.PhuongNhanId,
                        principalTable: "PhuongXas",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_DonHangs_Shippers_ShipperId",
                        column: x => x.ShipperId,
                        principalTable: "Shippers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_DonHangs_TinhThanhs_TinhGuiId",
                        column: x => x.TinhGuiId,
                        principalTable: "TinhThanhs",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_DonHangs_TinhThanhs_TinhNhanId",
                        column: x => x.TinhNhanId,
                        principalTable: "TinhThanhs",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "ChuyenHangs",
                columns: table => new
                {
                    DonHangId = table.Column<string>(type: "nvarchar(10)", nullable: false),
                    ThuTu = table.Column<int>(type: "int", nullable: false),
                    NgayTao = table.Column<DateTime>(type: "datetime2", nullable: true),
                    NgayHoanThanh = table.Column<DateTime>(type: "datetime2", nullable: true),
                    TrangThai = table.Column<int>(type: "int", nullable: false),
                    DiaChiGui = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    DiaChiNhan = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    HinhAnhNhanHang = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    HinhAnhGiaoHang = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ViTri = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ShipperId = table.Column<int>(type: "int", nullable: true),
                    TinhGuiId = table.Column<string>(type: "nvarchar(10)", nullable: false),
                    PhuongGuiId = table.Column<string>(type: "nvarchar(10)", nullable: false),
                    TinhNhanId = table.Column<string>(type: "nvarchar(10)", nullable: false),
                    PhuongNhanId = table.Column<string>(type: "nvarchar(10)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ChuyenHangs", x => new { x.DonHangId, x.ThuTu });
                    table.ForeignKey(
                        name: "FK_ChuyenHangs_DonHangs_DonHangId",
                        column: x => x.DonHangId,
                        principalTable: "DonHangs",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ChuyenHangs_PhuongXas_PhuongGuiId",
                        column: x => x.PhuongGuiId,
                        principalTable: "PhuongXas",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ChuyenHangs_PhuongXas_PhuongNhanId",
                        column: x => x.PhuongNhanId,
                        principalTable: "PhuongXas",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ChuyenHangs_Shippers_ShipperId",
                        column: x => x.ShipperId,
                        principalTable: "Shippers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ChuyenHangs_TinhThanhs_TinhGuiId",
                        column: x => x.TinhGuiId,
                        principalTable: "TinhThanhs",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ChuyenHangs_TinhThanhs_TinhNhanId",
                        column: x => x.TinhNhanId,
                        principalTable: "TinhThanhs",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_AspNetRoleClaims_RoleId",
                table: "AspNetRoleClaims",
                column: "RoleId");

            migrationBuilder.CreateIndex(
                name: "RoleNameIndex",
                table: "AspNetRoles",
                column: "NormalizedName",
                unique: true,
                filter: "[NormalizedName] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUserClaims_UserId",
                table: "AspNetUserClaims",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUserLogins_UserId",
                table: "AspNetUserLogins",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUserRoles_RoleId",
                table: "AspNetUserRoles",
                column: "RoleId");

            migrationBuilder.CreateIndex(
                name: "EmailIndex",
                table: "AspNetUsers",
                column: "NormalizedEmail");

            migrationBuilder.CreateIndex(
                name: "UserNameIndex",
                table: "AspNetUsers",
                column: "NormalizedUserName",
                unique: true,
                filter: "[NormalizedUserName] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_CauTrucGiaCuocs_LoaiDichVuId",
                table: "CauTrucGiaCuocs",
                column: "LoaiDichVuId");

            migrationBuilder.CreateIndex(
                name: "IX_ChiNhanhs_PhuongXaId",
                table: "ChiNhanhs",
                column: "PhuongXaId");

            migrationBuilder.CreateIndex(
                name: "IX_ChiNhanhs_TinhThanhId",
                table: "ChiNhanhs",
                column: "TinhThanhId");

            migrationBuilder.CreateIndex(
                name: "IX_ChuyenHangs_PhuongGuiId",
                table: "ChuyenHangs",
                column: "PhuongGuiId");

            migrationBuilder.CreateIndex(
                name: "IX_ChuyenHangs_PhuongNhanId",
                table: "ChuyenHangs",
                column: "PhuongNhanId");

            migrationBuilder.CreateIndex(
                name: "IX_ChuyenHangs_ShipperId",
                table: "ChuyenHangs",
                column: "ShipperId");

            migrationBuilder.CreateIndex(
                name: "IX_ChuyenHangs_TinhGuiId",
                table: "ChuyenHangs",
                column: "TinhGuiId");

            migrationBuilder.CreateIndex(
                name: "IX_ChuyenHangs_TinhNhanId",
                table: "ChuyenHangs",
                column: "TinhNhanId");

            migrationBuilder.CreateIndex(
                name: "IX_DonHangs_KhachHangId",
                table: "DonHangs",
                column: "KhachHangId");

            migrationBuilder.CreateIndex(
                name: "IX_DonHangs_LoaiDichVuId",
                table: "DonHangs",
                column: "LoaiDichVuId");

            migrationBuilder.CreateIndex(
                name: "IX_DonHangs_NhanVienId",
                table: "DonHangs",
                column: "NhanVienId");

            migrationBuilder.CreateIndex(
                name: "IX_DonHangs_PhuongGuiId",
                table: "DonHangs",
                column: "PhuongGuiId");

            migrationBuilder.CreateIndex(
                name: "IX_DonHangs_PhuongNhanId",
                table: "DonHangs",
                column: "PhuongNhanId");

            migrationBuilder.CreateIndex(
                name: "IX_DonHangs_ShipperId",
                table: "DonHangs",
                column: "ShipperId");

            migrationBuilder.CreateIndex(
                name: "IX_DonHangs_TinhGuiId",
                table: "DonHangs",
                column: "TinhGuiId");

            migrationBuilder.CreateIndex(
                name: "IX_DonHangs_TinhNhanId",
                table: "DonHangs",
                column: "TinhNhanId");

            migrationBuilder.CreateIndex(
                name: "IX_KhachHangs_CCCD",
                table: "KhachHangs",
                column: "CCCD",
                unique: true,
                filter: "[CCCD] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_KhachHangs_PhuongXaId",
                table: "KhachHangs",
                column: "PhuongXaId");

            migrationBuilder.CreateIndex(
                name: "IX_KhachHangs_TinhThanhId",
                table: "KhachHangs",
                column: "TinhThanhId");

            migrationBuilder.CreateIndex(
                name: "IX_KhachHangs_UserId",
                table: "KhachHangs",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_LoaiDichVus_MaDV",
                table: "LoaiDichVus",
                column: "MaDV",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_NhanViens_CCCD",
                table: "NhanViens",
                column: "CCCD",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_NhanViens_ChiNhanhId",
                table: "NhanViens",
                column: "ChiNhanhId");

            migrationBuilder.CreateIndex(
                name: "IX_NhanViens_PhuongXaId",
                table: "NhanViens",
                column: "PhuongXaId");

            migrationBuilder.CreateIndex(
                name: "IX_NhanViens_TinhThanhId",
                table: "NhanViens",
                column: "TinhThanhId");

            migrationBuilder.CreateIndex(
                name: "IX_NhanViens_UserId",
                table: "NhanViens",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_PhuongXas_TinhThanhId",
                table: "PhuongXas",
                column: "TinhThanhId");

            migrationBuilder.CreateIndex(
                name: "IX_Shippers_CCCD",
                table: "Shippers",
                column: "CCCD",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Shippers_ChiNhanhId",
                table: "Shippers",
                column: "ChiNhanhId");

            migrationBuilder.CreateIndex(
                name: "IX_Shippers_PhuongXaId",
                table: "Shippers",
                column: "PhuongXaId");

            migrationBuilder.CreateIndex(
                name: "IX_Shippers_TinhThanhId",
                table: "Shippers",
                column: "TinhThanhId");

            migrationBuilder.CreateIndex(
                name: "IX_Shippers_UserId",
                table: "Shippers",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_TinhThanhs_VungMienId",
                table: "TinhThanhs",
                column: "VungMienId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AspNetRoleClaims");

            migrationBuilder.DropTable(
                name: "AspNetUserClaims");

            migrationBuilder.DropTable(
                name: "AspNetUserLogins");

            migrationBuilder.DropTable(
                name: "AspNetUserRoles");

            migrationBuilder.DropTable(
                name: "AspNetUserTokens");

            migrationBuilder.DropTable(
                name: "CauTrucGiaCuocs");

            migrationBuilder.DropTable(
                name: "ChuyenHangs");

            migrationBuilder.DropTable(
                name: "Configs");

            migrationBuilder.DropTable(
                name: "AspNetRoles");

            migrationBuilder.DropTable(
                name: "DonHangs");

            migrationBuilder.DropTable(
                name: "KhachHangs");

            migrationBuilder.DropTable(
                name: "LoaiDichVus");

            migrationBuilder.DropTable(
                name: "NhanViens");

            migrationBuilder.DropTable(
                name: "Shippers");

            migrationBuilder.DropTable(
                name: "AspNetUsers");

            migrationBuilder.DropTable(
                name: "ChiNhanhs");

            migrationBuilder.DropTable(
                name: "PhuongXas");

            migrationBuilder.DropTable(
                name: "TinhThanhs");

            migrationBuilder.DropTable(
                name: "VungMiens");
        }
    }
}
