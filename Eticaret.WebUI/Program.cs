using Eticaret.Core.Entities;
using Eticaret.Data;
using Eticaret.Service;
using Eticaret.Service.Abstract;
using Eticaret.Service.Concrete;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.Extensions.Options;
using System.Security.Claims;


var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();
builder.Services.AddScoped<IEmailService,EmailService>();

// Session yap�land�rmas�
builder.Services.AddDistributedMemoryCache(); // Cache kullanmak i�in
builder.Services.AddSession(options =>
{
    options.Cookie.Name = "Eticaret.Session"; // Session �erezi ad�
    options.Cookie.HttpOnly = true; // Taray�c� taraf�ndan eri�ilebilirlik
    options.IdleTimeout = TimeSpan.FromDays(1); // Session s�resi (opsiyonel)
    options.Cookie.IsEssential = true; // Session �erezi i�in gerekli ayar (opsiyonel)
    options.IOTimeout= TimeSpan.FromMinutes(10);
});


//database tan�t
builder.Services.AddDbContext<DatabaseContext>();

builder.Services.AddScoped(typeof(IService<>), typeof(Service<>));


builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(x=>
    {
        x.LoginPath = "/Account/SignIn";
        x.AccessDeniedPath = "/AccessDenied"; //yetkisiz eri�im
        x.Cookie.Name = "Account";
        x.Cookie.MaxAge = TimeSpan.FromDays(7);
        x.Cookie.IsEssential = true; //kal�c� cokkie
    });
builder.Services.AddAuthorization(x =>
{
    x.AddPolicy("AdminPolicy", policy => policy.RequireClaim(ClaimTypes.Role, "Admin"));
    x.AddPolicy("UserPolicy", policy => policy.RequireClaim(ClaimTypes.Role, "Admin","User","Customer"));
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
app.UseSession(); // Session kullan�m�
app.UseAuthentication();// Autuhorization dan �nce gelmeli �nce outurm a�ma
app.UseAuthorization();//sonra yetkilendirme yap�lmal�

app.MapControllerRoute(
  name: "admin",
  pattern: "{area:exists}/{controller=Main}/{action=Index}/{id?}");

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
