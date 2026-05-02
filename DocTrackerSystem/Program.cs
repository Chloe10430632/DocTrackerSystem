using DocTrackerEFModels.EFModels;
using DocTrackerService.IService;
using DocTrackerService.Service;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Cors.Infrastructure;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Repository.IRepository;
using Repository.Repository;
using System;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllersWithViews();
builder.Services.AddDbContext<DocTrackerDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DocTrackerDbContext")));

//  Cookie 驗證服務
builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.Cookie.Name = "MySharedCookie";
        options.Cookie.Path = "/";
        options.Cookie.SameSite = SameSiteMode.Lax;
        options.ExpireTimeSpan = TimeSpan.FromMinutes(30);
        options.SlidingExpiration = true;
        options.LoginPath = "/Login/Login"; 
        options.AccessDeniedPath = "/Login/Denied";
        options.Cookie.SecurePolicy = CookieSecurePolicy.Always;
    });

builder.Services.AddDataProtection()
    .SetApplicationName("MySharedApp");

//開啟Http傳輸功能
builder.Services.AddHttpContextAccessor();

//資料存取層
builder.Services.AddScoped(typeof(IGenericRepository<>), typeof(GenericRepository<>));
builder.Services.AddScoped<IGenericRepository<User>, GenericRepository<User>>();
builder.Services.AddScoped<IGenericRepository<Document>, GenericRepository<Document>>();

//商業邏輯層
builder.Services.AddScoped<ILoginService, LoginService>();
builder.Services.AddScoped<IDocumentService, DocumentService>();

var app = builder.Build();

app.UseRouting();
app.UseAuthentication(); 
app.UseAuthorization();

using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    var context = services.GetRequiredService<DocTrackerDbContext>();
    
    if (!context.Users.Any())
    {
        var passwordHashAdmin = BCrypt.Net.BCrypt.HashPassword("Admin123456");
        var passwordHashNormal = BCrypt.Net.BCrypt.HashPassword("Normal123456");

        var adminUser = new User
        {
            UserName = "元大管理員1號",
            Account = "admin@yuanta.test.com",
            PasswordHash = passwordHashAdmin,
            RoleId = 1,
            PictureUrl = "https://res.cloudinary.com/dbyzfq61h/image/upload/v1776671465/Salter/Forum/20260420155102_a18e.jpg"
        };
        var normalUser = new User
        {
            UserName = "Chloe_19283",
            Account = "chloe@yuanta.test.com",
            PasswordHash = passwordHashNormal, 
            RoleId = 2,
            PictureUrl = "https://res.cloudinary.com/dbyzfq61h/image/upload/v1777569415/833e43dc46c4fa04fb547b501a221074_nk35s5.jpg"
        };

        context.Users.Add(adminUser);
        context.Users.Add(normalUser);
        context.SaveChanges();
    }
}

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Login}/{action=Login}/{id?}");
app.Run();
