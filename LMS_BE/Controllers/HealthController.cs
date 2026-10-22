using LMS_BE.Data;
using Microsoft.AspNetCore.Mvc;

namespace LMS_BE.Controllers;

// GET /api/health — kiểm tra nhanh backend đang chạy và đã kết nối được PostgreSQL chưa.
[ApiController]
[Route("api/health")]
public class HealthController(ApplicationDbContext db) : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> Get() =>
        await db.Database.CanConnectAsync()
            ? Ok(new { api = "ok", database = "ok" })
            : StatusCode(503, new { api = "ok", database = "error" });
}
