using LMS_BE.Data;using Microsoft.AspNetCore.Mvc;using Microsoft.EntityFrameworkCore;
namespace LMS_BE.Controllers;
[Route("api/notifications")]
public class NotificationsController(ApplicationDbContext db):LmsControllerBase(db){
 [HttpGet]public async Task<IActionResult> List()=>Ok(await Db.Notifications.Where(n=>n.UserId==CurrentUserId).OrderByDescending(n=>n.CreatedAt).Select(n=>new{n.Id,n.Title,n.Body,n.IsRead,n.CreatedAt}).ToListAsync());
 [HttpPut("{id:guid}/read")]public async Task<IActionResult> Read(Guid id){var n=await Db.Notifications.FirstOrDefaultAsync(n=>n.Id==id&&n.UserId==CurrentUserId);if(n==null)return NotFound();n.IsRead=true;await Db.SaveChangesAsync();return NoContent();}
 [HttpPut("read-all")]public async Task<IActionResult> ReadAll(){await Db.Notifications.Where(n=>n.UserId==CurrentUserId&&!n.IsRead).ExecuteUpdateAsync(s=>s.SetProperty(n=>n.IsRead,true));return NoContent();}
}
