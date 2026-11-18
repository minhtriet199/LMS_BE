using LMS_BE.Data;using LMS_BE.Models;using Microsoft.AspNetCore.Mvc;using Microsoft.AspNetCore.Authorization;using Microsoft.EntityFrameworkCore;
namespace LMS_BE.Controllers;
[Route("api/schedule")]
public class ScheduleController(ApplicationDbContext db):LmsControllerBase(db){
 [HttpGet]public async Task<IActionResult> List(DateTimeOffset? from,DateTimeOffset? to){var q=Db.ClassSessions.AsNoTracking().AsQueryable();if(from.HasValue)q=q.Where(s=>s.StartsAt>=from);if(to.HasValue)q=q.Where(s=>s.StartsAt<=to);if(!IsAdmin)q=q.Where(s=>IsTeacher?s.Class!.InstructorId==CurrentUserId:s.Class!.Enrollments.Any(e=>e.StudentId==CurrentUserId));return Ok(await q.OrderBy(s=>s.StartsAt).Select(s=>new{s.Id,s.Title,s.StartsAt,s.EndsAt,s.Room,s.SessionType,s.ClassId,Course=s.Class!.Course!.CourseName}).ToListAsync());}
 public record CreateSession(Guid ClassId,string Title,DateTimeOffset StartsAt,DateTimeOffset EndsAt,string Room,string SessionType);
 [HttpPost][Authorize(Roles="Admin,Teacher")]public async Task<IActionResult> Create(CreateSession x){var c=await Db.CourseClasses.FindAsync(x.ClassId);if(c==null)return NotFound();if(!IsAdmin&&c.InstructorId!=CurrentUserId)return Forbid();if(x.EndsAt<=x.StartsAt)return BadRequest("Invalid time range");var s=new ClassSession{ClassId=x.ClassId,Title=x.Title,StartsAt=x.StartsAt,EndsAt=x.EndsAt,Room=x.Room,SessionType=x.SessionType};Db.ClassSessions.Add(s);await Db.SaveChangesAsync();return Ok(new{s.Id});}
}
