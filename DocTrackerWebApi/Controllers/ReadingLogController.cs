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

        [HttpGet("/api/readinglog/search")]
        public async Task<ActionResult<IEnumerable<SearchReadingLogDto>>> Search([FromQuery] string? keyword)
        {
            string claimRole = User.FindFirst(ClaimTypes.Role)?.Value;
            if (string.IsNullOrEmpty(claimRole) || claimRole != "Admin")
            {
                return Unauthorized(new { isSuccess = false, message = "權限不足無法瀏覽" });
            }
            var result = await _logsService.GetAllLogsAsync(keyword ?? "");
            return Ok(result);
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

            string ip = GetClientIp(HttpContext);

            if(await _logsService.CheckAndCreateAsync(userId, ip, dto))
                return Ok(new { isSuccess = true ,message = "紀錄寫入成功" });
            else
                return StatusCode(500, new { isSuccess = false, message = "紀錄寫入失敗" });

        }

        public string GetClientIp(HttpContext context)
        {
            var cfIp = context.Request.Headers["CF-Connecting-IP"].FirstOrDefault();
            if (!string.IsNullOrEmpty(cfIp)) return cfIp;

            var forwardedIp = context.Request.Headers["X-Forwarded-For"].FirstOrDefault();
            if (!string.IsNullOrEmpty(forwardedIp)) return forwardedIp.Split(',')[0].Trim();

            return context.Connection.RemoteIpAddress?.MapToIPv4().ToString() ?? "0.0.0.0";
        }


    }
}
