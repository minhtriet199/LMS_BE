using LMS_BE.Data;
using Microsoft.AspNetCore.Mvc;

namespace LMS_BE.Controllers;

// GET /api/health — kiểm tra nhanh backend đang chạy và đã kết nối được PostgreSQL chưa.
[ApiController]
[Route("api/health")]
public class HealthController(ApplicationDbContext db) : ControllerBase
{
    [HttpGet]
    [EndpointSummary("Kiểm tra API đang chạy và kết nối được database")]
    public async Task<IActionResult> Get() =>
        await db.Database.CanConnectAsync()
            ? Ok(new { api = "ok", database = "ok" })
            : StatusCode(503, new { api = "ok", database = "error" });
}
