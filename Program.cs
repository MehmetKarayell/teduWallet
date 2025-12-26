using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.EntityFrameworkCore;
using teduWallet.Models;
using teduWallet.Services;

var builder = WebApplication.CreateBuilder(args);

// veritabanı bağlantısı
builder.Services.AddDbContext<CampuscoinContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("CampusCoinDb")));

// Register Wallet Service
builder.Services.AddScoped<IWalletService, WalletService>();

// Register Activity Service
builder.Services.AddScoped<IActivityService, ActivityService>();

// Cookie Authentication Servis Kaydı
builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
      options.LoginPath = "/Account/Login";
      options.AccessDeniedPath = "/Account/AccessDenied";
      options.ExpireTimeSpan = TimeSpan.FromMinutes(60);
    });

// yetkilendirme servisleri
builder.Services.AddAuthorization();
builder.Services.AddControllersWithViews();
builder.Services.AddSession(options =>
{
  options.IdleTimeout = TimeSpan.FromMinutes(60);
  options.Cookie.HttpOnly = true;
  options.Cookie.IsEssential = true;
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
  app.UseExceptionHandler("/Home/Error");
  // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
  app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

//kimlik doğrulama ve yetkilendirme middleware'leri 
app.UseAuthentication();
app.UseAuthorization();
app.UseSession();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Account}/{action=Register}/{id?}");

app.Run();
