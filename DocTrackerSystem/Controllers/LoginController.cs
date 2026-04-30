using DocTrackerService.DTO;
using DocTrackerService.IService;
using DocTrackerSystem.Models;
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

            // 檢查模型驗證 (Required 等)
            if (!ModelState.IsValid)
            {
                return Json(new { success = false, message = "請填寫完整資訊" });
            }

            // 驗證帳密
            if (await _loginService.CheckAndLoginAsync(model))
            {
                return Json(new { success = true, redirectUrl = Url.Action("Index", "Document") });
            }

            return Json(new { success = false, message = "帳號或密碼錯誤，請重新輸入。" });
        }

    }


}

