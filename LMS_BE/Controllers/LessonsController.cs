using LMS_BE.Data;using LMS_BE.Models;using Microsoft.AspNetCore.Mvc;using Microsoft.AspNetCore.Authorization;using Microsoft.EntityFrameworkCore;
namespace LMS_BE.Controllers;
[Route("api/lessons")]
public class LessonsController(ApplicationDbContext db):LmsControllerBase(db){
 [HttpGet("{id:guid}")]public async Task<IActionResult> Get(Guid id){var l=await Db.Lessons.AsNoTracking().Where(l=>l.Id==id).Select(l=>new{l.Id,l.Title,l.Content,l.ContentType,l.ContentUrl,l.EstimatedMinutes,CourseId=l.Module!.CourseId}).FirstOrDefaultAsync();if(l==null)return NotFound();if(!IsAdmin && !await Db.CourseClasses.AnyAsync(c=>c.CourseId==l.CourseId&&(c.InstructorId==CurrentUserId||c.Enrollments.Any(e=>e.StudentId==CurrentUserId))))return Forbid();return Ok(l);}
 public record CreateModule(Guid CourseId,string Title,int SequenceNo);
 [HttpPost("modules")][Authorize(Roles="Admin,Teacher")]public async Task<IActionResult> Module(CreateModule x){if(!await CanEdit(x.CourseId))return Forbid();var m=new Module{CourseId=x.CourseId,Title=x.Title,SequenceNo=x.SequenceNo};Db.Modules.Add(m);await Db.SaveChangesAsync();return Ok(new{m.Id});}
 public record CreateLesson(Guid ModuleId,string Title,string Content,string ContentType,string ContentUrl,int SequenceNo,int EstimatedMinutes);
 [HttpPost][Authorize(Roles="Admin,Teacher")]public async Task<IActionResult> Create(CreateLesson x){var m=await Db.Modules.FindAsync(x.ModuleId);if(m==null)return NotFound();if(!await CanEdit(m.CourseId))return Forbid();var l=new Lesson{ModuleId=x.ModuleId,Title=x.Title,Content=x.Content,ContentType=x.ContentType,ContentUrl=x.ContentUrl,SequenceNo=x.SequenceNo,EstimatedMinutes=x.EstimatedMinutes};Db.Lessons.Add(l);await Db.SaveChangesAsync();return Ok(new{l.Id});}
 private async Task<bool> CanEdit(Guid id)=>IsAdmin||await Db.CourseClasses.AnyAsync(c=>c.CourseId==id&&c.InstructorId==CurrentUserId);
}
