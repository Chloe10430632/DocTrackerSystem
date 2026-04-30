using DocTrackerEFModels.EFModels;
using DocTrackerService.IService;
using DocTrackerService.Service;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Cors.Infrastructure;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Repository.IRepository;
using Repository.Repository;
using System;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();
builder.Services.AddDbContext<DocTrackerDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DocTrackerDbContext")));

//JWT 驗證服務
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = builder.Configuration["Jwt:Issuer"],
            ValidAudience = builder.Configuration["Jwt:Audience"],
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"]))
        };
    });


//開啟Http傳輸功能
builder.Services.AddHttpContextAccessor();

//資料存取層
builder.Services.AddScoped(typeof(IGenericRepository<>), typeof(GenericRepository<>));
builder.Services.AddScoped<IGenericRepository<User>, GenericRepository<User>>();

//商業邏輯層
builder.Services.AddScoped<ILoginService, LoginService>();


var app = builder.Build();


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

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
