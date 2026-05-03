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

            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, user.UserName),
                new Claim(ClaimTypes.Role, user.Role.RoleName),
                new Claim("UserId", user.UserId.ToString()),
                new Claim("Avatar", user.PictureUrl ?? "/imgs/peach.jpg")
            };

            var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
            var authProperties = new AuthenticationProperties { IsPersistent = true, ExpiresUtc = DateTime.UtcNow.AddMinutes(30) };

            await _httpContextAccessor.HttpContext.SignInAsync(
                CookieAuthenticationDefaults.AuthenticationScheme,
                new ClaimsPrincipal(claimsIdentity),
                authProperties);

            return true;
        }

    }
}
