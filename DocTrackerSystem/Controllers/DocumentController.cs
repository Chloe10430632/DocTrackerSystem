using DocTrackerService.DTO;
using DocTrackerService.IService;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace DocTrackerSystem.Controllers
{
    [Authorize]
    [ApiController]
    public class DocumentController : Controller
    {

        private readonly IDocumentService _docService;

        public DocumentController(IDocumentService docService)
        {
            _docService = docService;
        }


        [HttpGet("/Document/Index")]
        public IActionResult Index()
        {
            return View();
        }



        [HttpGet("/api/document/{id}")]
        public async Task<ActionResult<DocumentModel>> GetDocument(int id)
        {

            var doc = await _docService.GetDocumentByID(id);

            if (doc == null) return NotFound();

            return Ok(doc);
        }


    }
}
