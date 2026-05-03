using DocTrackerEFModels.EFModels;
using DocTrackerService.IService;
using DocTrackerService.Service;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Repository.IRepository;
using Repository.Repository;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddDbContext<DocTrackerDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DocTrackerDbContext")));

//  Cookie 驗證服務
builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.Events = new CookieAuthenticationEvents
        {
            OnRedirectToLogin = context =>
            {
                if (context.Request.Path.StartsWithSegments("/api"))
                {
                    context.Response.StatusCode = 401;
                    return Task.CompletedTask;
                }
                context.Response.Redirect(context.RedirectUri);
                return Task.CompletedTask;
            }
        };
        options.Cookie.Name = "MySharedCookie";
        //options.Cookie.Domain = ".salter-ocean.online";
        options.Cookie.Path = "/";
        options.Cookie.SameSite = SameSiteMode.Lax;
        options.ExpireTimeSpan = TimeSpan.FromMinutes(30);
        options.SlidingExpiration = true;
        options.LoginPath = "/Login/Login";
        options.AccessDeniedPath = "/Login/Denied";
        options.Cookie.SecurePolicy = CookieSecurePolicy.Always;
    });


//var keysDirectory = new DirectoryInfo("/app/DataKeys");

//if (!keysDirectory.Exists)
//{
//    keysDirectory.Create();
//}

builder.Services.AddDataProtection()
    .SetApplicationName("MySharedApp");
    //.PersistKeysToFileSystem(keysDirectory);

//資料存取層
builder.Services.AddScoped(typeof(IGenericRepository<>), typeof(GenericRepository<>));
builder.Services.AddScoped<IGenericRepository<ReadingLog>, GenericRepository<ReadingLog>>();

//商業邏輯層
builder.Services.AddScoped<IReadingLogService, ReadingLogService>();

//CORS
var allowedOrigins = builder.Configuration.GetSection("AllowedOrigins").Get<string[]>();
builder.Services.AddCors(options =>
{
    options.AddPolicy("CorsPolicy", policy =>
    {
        policy.WithOrigins(allowedOrigins)
              .AllowAnyHeader()
              .AllowAnyMethod()
              .AllowCredentials()
              .WithExposedHeaders("Location");
    });
});

var app = builder.Build();

app.UseRouting();
app.UseCors("CorsPolicy");
app.UseAuthentication();
app.UseAuthorization();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.MapControllers();
app.Run();
