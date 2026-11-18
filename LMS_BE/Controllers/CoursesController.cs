using LMS_BE.Data; using LMS_BE.Models; using Microsoft.AspNetCore.Mvc; using Microsoft.AspNetCore.Authorization; using Microsoft.EntityFrameworkCore;
namespace LMS_BE.Controllers;
[Route("api/courses")]
public class CoursesController(ApplicationDbContext db) : LmsControllerBase(db) {
 [HttpGet] public async Task<IActionResult> List() {
  var q=Db.Courses.AsNoTracking().AsQueryable();
  if (!IsAdmin && !IsTeacher) q=q.Where(c=>c.Classes.Any(k=>k.Enrollments.Any(e=>e.StudentId==CurrentUserId)));
  else if (IsTeacher && !IsAdmin) q=q.Where(c=>c.Classes.Any(k=>k.InstructorId==CurrentUserId));
  return Ok(await q.Select(c=>new {c.Id,c.CourseCode,c.CourseName,c.Description,ClassCount=c.Classes.Count}).ToListAsync()); }
 [HttpGet("{id:guid}")] public async Task<IActionResult> Detail(Guid id) {
  var c=await Db.Courses.AsNoTracking().Where(c=>c.Id==id).Select(c=>new{c.Id,c.CourseCode,c.CourseName,c.Description,Modules=c.Modules.OrderBy(m=>m.SequenceNo).Select(m=>new{m.Id,m.Title,Lessons=m.Lessons.OrderBy(l=>l.SequenceNo).Select(l=>new{l.Id,l.Title,l.ContentType,l.ContentUrl,l.EstimatedMinutes})})}).FirstOrDefaultAsync();
  if(c==null)return NotFound();
  if(!IsAdmin && !(await Db.CourseClasses.AnyAsync(k=>k.CourseId==id && (k.InstructorId==CurrentUserId || k.Enrollments.Any(e=>e.StudentId==CurrentUserId)))))return Forbid();
  return Ok(c); }
 public record SaveCourse(string CourseCode,string CourseName,string Description);
 [HttpPost][Authorize(Roles="Admin")] public async Task<IActionResult> Create(SaveCourse x) { if(string.IsNullOrWhiteSpace(x.CourseCode)||string.IsNullOrWhiteSpace(x.CourseName))return BadRequest("Required fields"); if(await Db.Courses.AnyAsync(c=>c.CourseCode==x.CourseCode))return Conflict("Course code exists");var c=new Course{CourseCode=x.CourseCode,CourseName=x.CourseName,Description=x.Description,CreatedById=CurrentUserId};Db.Courses.Add(c);await Db.SaveChangesAsync();return CreatedAtAction(nameof(Detail),new{id=c.Id},new{c.Id}); }
 [HttpPut("{id:guid}")][Authorize(Roles="Admin")] public async Task<IActionResult> Update(Guid id,SaveCourse x){var c=await Db.Courses.FindAsync(id);if(c==null)return NotFound();if(await Db.Courses.AnyAsync(t=>t.Id!=id && t.CourseCode==x.CourseCode))return Conflict();c.CourseCode=x.CourseCode;c.CourseName=x.CourseName;c.Description=x.Description;await Db.SaveChangesAsync();return NoContent();}
 [HttpDelete("{id:guid}")][Authorize(Roles="Admin")] public async Task<IActionResult> Delete(Guid id){var c=await Db.Courses.FindAsync(id);if(c==null)return NotFound();Db.Courses.Remove(c);try{await Db.SaveChangesAsync();}catch(DbUpdateException){return Conflict("Course has related data");}return NoContent();}
}
