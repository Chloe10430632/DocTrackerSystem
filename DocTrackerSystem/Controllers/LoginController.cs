using DocTrackerService.DTO;
using DocTrackerService.IService;
using DocTrackerSystem.Models;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Net.Http;

namespace DocTrackerSystem.Controllers
{
    public class LoginController : Controller
    {
        private readonly ILoginService _loginService;

        public LoginController(ILoginService loginService)
        {
            _loginService = loginService;
        }

        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        [AutoValidateAntiforgeryToken]
        public async Task<IActionResult> Login(LoginModel model)
        {

           
            if (!ModelState.IsValid)
            {
                return View("Login", model);
            }

            // 驗證帳密
            if (!await _loginService.CheckAndLoginAsync(model))
            {
                ModelState.AddModelError("", "帳號或密碼錯誤，請重新輸入。");
                return View("Login", model);
            }

            return RedirectToAction("Index", "Document");
        }

        [HttpPost]
        public async Task<IActionResult> FastLogin(string type)
        {
           
            var testAccounts = new Dictionary<string, (string Account, string Password)>
    {
        { "admin", ("admin@yuanta.test.com", "Admin123456") },
        { "chloe", ("chloe@yuanta.test.com", "Normal123456") }
    };

            if (!testAccounts.ContainsKey(type)) return BadRequest("無效的測試類型");

            var (account, password) = testAccounts[type];
            var model = new LoginModel { Account = account, Password = password };

            // 直接複用你現有的登入服務
            var success = await _loginService.CheckAndLoginAsync(model);

            if (success)
            {
                return RedirectToAction("Index", "Document");
            }

            ModelState.AddModelError("", "快速登入失敗");
            return View("Login");
        }


        public async Task<IActionResult> Logout()
        {

            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);


            return RedirectToAction("Login");
        }

        [Authorize]
        [ResponseCache(NoStore = true, Location = ResponseCacheLocation.None)]
        public IActionResult Denied()
        {
            return View();
        }

    }


}

