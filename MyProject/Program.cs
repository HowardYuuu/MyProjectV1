using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using System.Security.Claims;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using MyProject.Data;
using MyProject.Models;
using System.Web;
using Microsoft.AspNetCore.DataProtection;
using MyProject.Controllers.LinePay.Service;

var builder = WebApplication.CreateBuilder(args);


// Add services to the container.
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection") ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(connectionString));
builder.Services.AddDbContext<MyProjectContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("MyProjectConnection")));
builder.Services.AddDatabaseDeveloperPageExceptionFilter();

builder.Services.AddDefaultIdentity<IdentityUser>(options => options.SignIn.RequireConfirmedAccount = true)
    .AddEntityFrameworkStores<ApplicationDbContext>();

builder.Services.AddHttpContextAccessor();

builder.Services.AddControllersWithViews();

builder.Services.AddHttpClient();

builder.Services.AddCors(option =>
{
	option.AddDefaultPolicy(policy =>
	{
		policy.AllowAnyOrigin().AllowAnyHeader().AllowAnyMethod();
	});
});

//cookie
builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
	.AddCookie(options =>
	{
		options.ExpireTimeSpan = TimeSpan.FromMinutes(100);
		options.LoginPath = "/Member/Login";
		options.SlidingExpiration = true;
		options.LogoutPath = "/Member/LogOut";
		// 將 Secure 屬性設置為 CookieSecurePolicy.None
		options.Cookie.SecurePolicy = CookieSecurePolicy.None;

		// 可選：設置 SameSite 策略
		options.Cookie.SameSite = SameSiteMode.Strict;
		//options.AccessDeniedPath = "/Forbidden/";
	});
//builder.Services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();

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


app.UseHttpsRedirection();
app.UseStaticFiles();
//app.UseCookiePolicy();

app.UseRouting();

//cookie啟用
//app.UseCookiePolicy();
app.UseAuthentication();
app.UseAuthorization();

// 添加 NonceMiddleware，必須在添加 Razor Pages 或 MVC 之前
app.UseMiddleware<NonceMiddleWare>();

//app.MapControllerRoute(
//    name: "admin",
//    pattern: "{controller=admin}/{action=Index}/{id?}");
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");
app.MapRazorPages();
//app.MapDefaultControllerRoute();

app.Run();
