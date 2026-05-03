using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DocTrackerSystem.Controllers
{
    [Authorize]
    public class ReadingLogController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
