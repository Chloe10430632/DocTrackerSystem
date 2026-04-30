using DocTrackerSystem.Models;
using Microsoft.AspNetCore.Mvc;
using System.Net.Http;

namespace DocTrackerSystem.Controllers
{
    public class LoginController : Controller
    {
        

        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }
        //[HttpPost]
        
    }


}

