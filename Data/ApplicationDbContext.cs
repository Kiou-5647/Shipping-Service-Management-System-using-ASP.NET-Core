using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Shipping.Models;
using System.Reflection.Emit;

namespace Shipping.Data
{
	public class ApplicationDbContext : IdentityDbContext
	{
		public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
			: base(options)
		{
		}

		// DbSets
		public DbSet<VungMien> VungMiens { get; set; } = null!;
		public DbSet<TinhThanh> TinhThanhs { get; set; } = null!;
		public DbSet<PhuongXa> PhuongXas { get; set; } = null!;
		public DbSet<ChiNhanh> ChiNhanhs { get; set; } = null!;
		public DbSet<NhanVien> NhanViens { get; set; } = null!;
		public DbSet<Shipper> Shippers { get; set; } = null!;
		public DbSet<KhachHang> KhachHangs { get; set; } = null!;
		public DbSet<LoaiDichVu> LoaiDichVus { get; set; } = null!;
		public DbSet<CauTrucGiaCuoc> CauTrucGiaCuocs { get; set; } = null!;
		public DbSet<DonHang> DonHangs { get; set; } = null!;
		public DbSet<ChuyenHang> ChuyenHangs { get; set; } = null!;
		public DbSet<Config> Configs { get; set; } = null!;


		protected override void OnModelCreating(ModelBuilder modelBuilder)
		{
			base.OnModelCreating(modelBuilder);

			modelBuilder.Entity<ChuyenHang>()
				.HasKey(c => new { c.DonHangId, c.ThuTu });

			modelBuilder.Entity<KhachHang>()
				.HasIndex(x => x.CCCD)
				.IsUnique();

			modelBuilder.Entity<NhanVien>()
				.HasIndex(x => x.CCCD)
				.IsUnique();

			modelBuilder.Entity<Shipper>()
				.HasIndex(x => x.CCCD)
				.IsUnique();

			modelBuilder.Entity<LoaiDichVu>()
				.HasIndex(n => n.MaDV)
				.IsUnique();

			modelBuilder.Entity<TinhThanh>()
				.HasOne(t => t.VungMien)
				.WithMany(v => v.TinhThanhs)
				.HasForeignKey(t => t.VungMienId)
				.OnDelete(DeleteBehavior.Restrict);

			modelBuilder.Entity<PhuongXa>()
				.HasOne(p => p.TinhThanh)
				.WithMany(t => t.PhuongXas)
				.HasForeignKey(p => p.TinhThanhId)
				.OnDelete(DeleteBehavior.Restrict);

			modelBuilder.Entity<ChiNhanh>()
				.HasOne(c => c.TinhThanh)
				.WithMany(t => t.ChiNhanhs)
				.HasForeignKey(c => c.TinhThanhId)
				.OnDelete(DeleteBehavior.Restrict);
			modelBuilder.Entity<ChiNhanh>()
				.HasOne(c => c.PhuongXa)
				.WithMany(p => p.ChiNhanhs)
				.HasForeignKey(c => c.PhuongXaId)
				.OnDelete(DeleteBehavior.Restrict);

			modelBuilder.Entity<KhachHang>()
				.HasOne(k => k.TinhThanh)
				.WithMany(t => t.KhachHangs)
				.HasForeignKey(k => k.TinhThanhId)
				.OnDelete(DeleteBehavior.Restrict);
			modelBuilder.Entity<KhachHang>()
				.HasOne(k => k.PhuongXa)
				.WithMany(p => p.KhachHangs)
				.HasForeignKey(k => k.PhuongXaId)
				.OnDelete(DeleteBehavior.Restrict);

			modelBuilder.Entity<NhanVien>()
				.HasOne(n => n.TinhThanh)
				.WithMany(t => t.NhanViens)
				.HasForeignKey(n => n.TinhThanhId)
				.OnDelete(DeleteBehavior.Restrict);
			modelBuilder.Entity<NhanVien>()
				.HasOne(n => n.PhuongXa)
				.WithMany(p => p.NhanViens)
				.HasForeignKey(n => n.PhuongXaId)
				.OnDelete(DeleteBehavior.Restrict);

			modelBuilder.Entity<Shipper>()
				.HasOne(s => s.TinhThanh)
				.WithMany(t => t.Shippers)
				.HasForeignKey(s => s.TinhThanhId)
				.OnDelete(DeleteBehavior.Restrict);
			modelBuilder.Entity<Shipper>()
				.HasOne(s => s.PhuongXa)
				.WithMany(p => p.Shippers)
				.HasForeignKey(s => s.PhuongXaId)
				.OnDelete(DeleteBehavior.Restrict);

			modelBuilder.Entity<DonHang>()
				.HasOne(d => d.TinhThanhGui)
				.WithMany(t => t.GuiDonHangs)
				.HasForeignKey(d => d.TinhGuiId)
				.OnDelete(DeleteBehavior.Restrict);

			modelBuilder.Entity<DonHang>()
				.HasOne(d => d.TinhThanhNhan)
				.WithMany(t => t.NhanDonHangs)
				.HasForeignKey(d => d.TinhNhanId)
				.OnDelete(DeleteBehavior.Restrict);

			modelBuilder.Entity<DonHang>()
				.HasOne(d => d.PhuongXaGui)
				.WithMany(p => p.GuiDonHangs)
				.HasForeignKey(d => d.PhuongGuiId)
				.OnDelete(DeleteBehavior.Restrict);

			modelBuilder.Entity<DonHang>()
				.HasOne(d => d.PhuongXaNhan)
				.WithMany(p => p.NhanDonHangs)
				.HasForeignKey(d => d.PhuongNhanId)
				.OnDelete(DeleteBehavior.Restrict);

			modelBuilder.Entity<ChuyenHang>()
				.HasOne(c => c.TinhThanhGui)
				.WithMany(t => t.GuiChuyenHangs)
				.HasForeignKey(c => c.TinhGuiId)
				.OnDelete(DeleteBehavior.Restrict);

			modelBuilder.Entity<ChuyenHang>()
				.HasOne(c => c.TinhThanhNhan)
				.WithMany(t => t.NhanChuyenHangs)
				.HasForeignKey(c => c.TinhNhanId)
				.OnDelete(DeleteBehavior.Restrict);

			modelBuilder.Entity<ChuyenHang>()
				.HasOne(c => c.PhuongXaGui)
				.WithMany(p => p.GuiChuyenHangs)
				.HasForeignKey(c => c.PhuongGuiId)
				.OnDelete(DeleteBehavior.Restrict);

			modelBuilder.Entity<ChuyenHang>()
				.HasOne(c => c.PhuongXaNhan)
				.WithMany(p => p.NhanChuyenHangs)
				.HasForeignKey(c => c.PhuongNhanId)
				.OnDelete(DeleteBehavior.Restrict);

			modelBuilder.Entity<CauTrucGiaCuoc>()
				.HasOne(c => c.LoaiDichVu)
				.WithMany(l => l.CauTrucGiaCuocs)
				.HasForeignKey(c => c.LoaiDichVuId)
				.OnDelete(DeleteBehavior.Restrict);

			modelBuilder.Entity<DonHang>()
				.HasOne(d => d.LoaiDichVu)
				.WithMany(l => l.DonHangs)
				.HasForeignKey(d => d.LoaiDichVuId)
				.OnDelete(DeleteBehavior.Restrict);

			modelBuilder.Entity<DonHang>()
				.HasOne(d => d.KhachHang)
				.WithMany(k => k.DonHangs)
				.HasForeignKey(d => d.KhachHangId)
				.IsRequired(false)
				.OnDelete(DeleteBehavior.Restrict);

			modelBuilder.Entity<DonHang>()
				.HasOne(d => d.NhanVien)
				.WithMany(n => n.DonHangs)
				.HasForeignKey(d => d.NhanVienId)
				.IsRequired(false)
				.OnDelete(DeleteBehavior.Restrict);

			modelBuilder.Entity<DonHang>()
				.HasOne(d => d.Shipper)
				.WithMany(n => n.DonHangs)
				.HasForeignKey(d => d.ShipperId)
				.IsRequired(false)
				.OnDelete(DeleteBehavior.Restrict);

			modelBuilder.Entity<ChuyenHang>()
				.HasOne(c => c.DonHang)
				.WithMany(d => d.ChuyenHangs)
				.HasForeignKey(c => c.DonHangId)
				.OnDelete(DeleteBehavior.Restrict);

			modelBuilder.Entity<ChuyenHang>()
				.HasOne(c => c.Shipper)
				.WithMany(s => s.ChuyenHangs)
				.HasForeignKey(c => c.ShipperId)
				.IsRequired(false)
				.OnDelete(DeleteBehavior.Restrict);

			modelBuilder.Entity<NhanVien>()
				.HasOne(n => n.ChiNhanh)
				.WithMany(c => c.NhanViens)
				.HasForeignKey(n => n.ChiNhanhId)
				.IsRequired(false)
				.OnDelete(DeleteBehavior.Restrict);

			modelBuilder.Entity<Shipper>()
				.HasOne(s => s.ChiNhanh)
				.WithMany(c => c.Shippers)
				.HasForeignKey(s => s.ChiNhanhId)
				.IsRequired(false)
				.OnDelete(DeleteBehavior.Restrict);
		}
	}
}
