using DocTrackerEFModels.EFModels;
using DocTrackerService.DTO;
using DocTrackerService.IService;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.Security.Claims;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace DocTrackerWebApi.Controllers
{
    [Authorize(AuthenticationSchemes = CookieAuthenticationDefaults.AuthenticationScheme)]
    [Route("api/[controller]")]
    [ApiController]
    public class ReadingLogController : ControllerBase
    {
        private readonly IReadingLogService _logsService;

        public ReadingLogController(IReadingLogService logsService)
        {
            _logsService = logsService;
        }



        // GET: api/<ReadingLogController>
        [HttpGet]
        public IEnumerable<string> Get()
        {
            return new string[] { "value1", "value2" };
        }

        // GET api/<ReadingLogController>/5
        [HttpGet("{id}")]
        public string Get(int id)
        {
            return "value";
        }


        [HttpPost("/api/readinglog/create")]
        public async Task<IActionResult> CreateLog([FromBody] CreateReadingLogDto dto)
        {
            TimeZoneInfo taipeiZone = TimeZoneInfo.FindSystemTimeZoneById("Taipei Standard Time");
            dto.StartTime = TimeZoneInfo.ConvertTimeFromUtc(dto.StartTime, taipeiZone);
            dto.EndTime = TimeZoneInfo.ConvertTimeFromUtc(dto.EndTime, taipeiZone);

            string claimId = User.FindFirst("UserId")?.Value;
            if (string.IsNullOrEmpty(claimId) || !int.TryParse(claimId, out int userId))
            {
                return Unauthorized(new { isSuccess = false, message = "身份驗證已過期或有誤" });
            }

            //取得 Client IP
            string ip = HttpContext.Connection.RemoteIpAddress?.ToString() ?? "0.0.0.0";
            using (var httpClient = new HttpClient())
            {
                 ip = await httpClient.GetStringAsync("https://api.ipify.org");
            }

            if(await _logsService.CheckAndCreateAsync(userId, ip, dto))
                return Ok(new { isSuccess = true ,message = "紀錄寫入成功" });
            else
                return StatusCode(500, new { isSuccess = false, message = "紀錄寫入失敗" });

        }

      
    }
}
