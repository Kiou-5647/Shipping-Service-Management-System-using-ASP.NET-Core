using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Shipping.Data;
using Shipping.Data.Seed;
using Shipping.Models;
using Shipping.Repositories.ConfigService;
using Shipping.Repositories.CrudService;
using Shipping.Repositories.Dashboard;
using Shipping.Repositories.DonHangService;
using Shipping.Repositories.GeoService;
using Shipping.Repositories.UserService;
using Shipping.ViewModels;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddScoped<IGeoService<TinhThanh>, TinhThanhService>();
builder.Services.AddScoped<IGeoService<PhuongXa>, PhuongXaService>();

builder.Services.AddScoped<ICrudService<ChiNhanh>, ChiNhanhService>();
builder.Services.AddScoped<ICrudService<LoaiDichVu>, LoaiDichVuService>();
builder.Services.AddScoped<ICrudService<CauTrucGiaCuoc>, GiaCuocService>();

builder.Services.AddScoped<IUserService<NhanVien, NhanVienViewModel>, NhanVienService>();
builder.Services.AddScoped<IUserService<Shipper, ShipperViewModel>, ShipperService>();
builder.Services.AddScoped<IShipperService, ShipperService>();
builder.Services.AddScoped<IUserService<KhachHang, KhachHangViewModel>, KhachHangService>();

builder.Services.AddScoped<IDonHangService,DonHangService>();
builder.Services.AddScoped<IChuyenHangService, ChuyenHangService>();

builder.Services.AddScoped<IConfigService, ConfigService>();

builder.Services.AddScoped<IDashboardService, DashboardService>();

var connectionString = builder.Configuration.GetConnectionString("DefaultConnection") ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");

builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(connectionString));

builder.Services.AddDatabaseDeveloperPageExceptionFilter();

builder.Services.AddIdentity<IdentityUser, IdentityRole>(options =>
	{
	options.SignIn.RequireConfirmedAccount = false;
	options.Password.RequireDigit = false;
	options.Password.RequireLowercase = false;
	options.Password.RequireUppercase = false;
	options.Password.RequireNonAlphanumeric = false;
	options.Password.RequiredLength = 6;
	})
	.AddEntityFrameworkStores<ApplicationDbContext>()
	.AddDefaultTokenProviders().AddRoles<IdentityRole>();

builder.Services.ConfigureApplicationCookie(options => {
	options.Cookie.SecurePolicy = CookieSecurePolicy.SameAsRequest;
	options.Cookie.SameSite = SameSiteMode.Lax;
	options.ExpireTimeSpan = TimeSpan.FromDays(7);

	options.LoginPath = "/Identity/Account/Login";
	options.LogoutPath = "/Identity/Account/Logout";
	options.AccessDeniedPath = "/Identity/Account/AccessDenied";
});

builder.Services.Configure<SecurityStampValidatorOptions>(options =>
{
	options.ValidationInterval = TimeSpan.FromMinutes(0);
});


builder.Services.AddControllersWithViews();
builder.Services.AddRazorPages();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseMigrationsEndPoint();
}
else
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

//app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

using (var scope = app.Services.CreateScope())
{
	var services = scope.ServiceProvider;
	try
	{
		await DbInitializer.SeedData(services);
	}
	catch (Exception ex)
	{
		var logger = services.GetRequiredService<ILogger<Program>>();
		logger.LogError(ex, "An error occurred while seeding the database.");
	}
}

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");
app.MapRazorPages();

app.Run();
