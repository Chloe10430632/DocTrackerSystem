using DocTrackerEFModels.EFModels;
using DocTrackerService.DTO;
using DocTrackerService.IService;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Authentication;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using Repository.IRepository;
using Repository.Repository;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using AuthenticationProperties = Microsoft.AspNetCore.Authentication.AuthenticationProperties;



namespace DocTrackerService.Service
{
    public class LoginService : ILoginService
    {
        private readonly IGenericRepository<User> _dbUsers;
        private readonly IConfiguration _config;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public LoginService(
            IGenericRepository<User> dbUsers, 
            IConfiguration config,
            IHttpContextAccessor httpContextAccessor)
        {
            _dbUsers = dbUsers;
            _config = config;
            _httpContextAccessor = httpContextAccessor;
        }


        public async Task<bool> CheckAndLoginAsync(LoginModel data)
        {
            var user = await _dbUsers
                .GetAll()
                .Include(u=>u.Role)
                .FirstOrDefaultAsync(u => u.Account == data.Account);
            if (user == null) return false;

            if (!BCrypt.Net.BCrypt.Verify(data.Password, user.PasswordHash)) return false;

            // --- A. 處理 JWT (給 API 用的) ---
            //var token = GenerateJwtToken(user);
            // 把 JWT 存入 Session，之後呼叫 API 就從這裡拿
            //_httpContextAccessor.HttpContext.Session.SetString("ApiToken", token);

            // --- B. 處理 MVC Cookie 驗證 (給頁面顯示使用者資訊用的) ---
            var claims = new List<Claim> {
        new Claim(ClaimTypes.Name, user.UserName),
        new Claim(ClaimTypes.Role, user.Role.RoleName),
        new Claim("UserId", user.UserId.ToString()),
        new Claim("Avatar", user.PictureUrl ?? "/imgs/peach.jpg")
    };

            var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
            var authProperties = new AuthenticationProperties { IsPersistent = true, ExpiresUtc = DateTime.UtcNow.AddMinutes(30) };

            // 這行執行後，你的 User.FindFirst 就有資料了
            await _httpContextAccessor.HttpContext.SignInAsync(
                CookieAuthenticationDefaults.AuthenticationScheme,
                new ClaimsPrincipal(claimsIdentity),
                authProperties);

            return true;
        }


        private string GenerateJwtToken(User user)
        {
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["Jwt:Key"]));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var claims = new[] {
                new Claim(ClaimTypes.Name, user.UserName),
                new Claim(ClaimTypes.Role, user.Role.RoleName), 
                new Claim("UserId", user.UserId.ToString()),
                new Claim("Avatar",user.PictureUrl)
            };
          
            var token = new JwtSecurityToken(
                issuer: _config["Jwt:Issuer"],
                audience: _config["Jwt:Audience"],
                claims: claims,
                expires: DateTime.Now.AddHours(2),
                signingCredentials: creds
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}
