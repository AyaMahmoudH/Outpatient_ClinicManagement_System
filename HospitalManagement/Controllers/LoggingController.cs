using HospitalManagement.DTOs;
using HospitalManagement.Models;
using HospitalManagement.Services;
using Microsoft.AspNetCore.Mvc;

namespace HospitalManagement.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class LoggingController : ControllerBase
    {
        private readonly ILoggingService _loggingService;

        public LoggingController(ILoggingService loggingService)
        {
            _loggingService = loggingService;
        }

        [HttpPost("log")]
        public async Task<IActionResult> LogActivity([FromBody] ActivityLogRequest request)
        {
            await _loggingService.LogActivityAsync(
                request.UserId,
                request.UserName,
                request.UserRole,
                request.Action,
                request.EntityName,
                request.EntityId,
                request.Details,
                request.IpAddress
            );

            return Ok("Activity logged successfully.");
        }

        [HttpGet("logs")]
        public async Task<IActionResult> GetLogs([FromQuery] DateTime startDate, [FromQuery] DateTime endDate)
        {
            var logs = await _loggingService.GetActivityLogsAsync(startDate, endDate);
            return Ok(logs);
        }

        [HttpGet("logs/user/{userId}")]
        public async Task<IActionResult> GetUserLogs(string userId)
        {
            var logs = await _loggingService.GetUserActivityLogsAsync(userId);
            return Ok(logs);
        }
    }
}
