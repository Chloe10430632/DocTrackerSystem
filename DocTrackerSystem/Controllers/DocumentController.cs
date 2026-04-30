using Microsoft.AspNetCore.Mvc;

namespace DocTrackerSystem.Controllers
{
    public class DocumentController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
