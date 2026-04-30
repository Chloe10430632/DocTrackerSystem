using DocTrackerEFModels.EFModels;
using DocTrackerService.DTO;
using DocTrackerService.IService;
using Microsoft.AspNetCore.Http;
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
            // 1. 從 DB 找人
            var user = await _dbUsers
                .GetAll()
                .Include(u=>u.Role)
                .FirstOrDefaultAsync(u => u.Account == data.Account);
            if (user == null) return false;

            // 2. 驗證密碼
            if (!BCrypt.Net.BCrypt.Verify(data.Password, user.PasswordHash)) return false;

            // 3. 驗證成功，產生 JWT
            var token = GenerateJwtToken(user);

            // 3. 設定 Cookie 的規則
            var cookieOptions = new CookieOptions
            {
                HttpOnly = true,                  
                Secure = true,                  
                SameSite = SameSiteMode.Strict,   
                Expires = DateTime.UtcNow.AddHours(2) 
            };

            _httpContextAccessor.HttpContext.Response.Cookies.Append("AuthToken", token, cookieOptions);

            return true;
        }


        private string GenerateJwtToken(User user)
        {
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["Jwt:Key"]));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var claims = new[] {
                new Claim(ClaimTypes.Name, user.Account),
                new Claim(ClaimTypes.Role, user.Role.RoleName), 
                new Claim("UserId", user.UserId.ToString())
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
