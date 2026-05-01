using Microsoft.AspNetCore.Mvc;

namespace DocTrackerSystem.Controllers
{
    public class ReadingLogController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
