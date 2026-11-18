using LMS_BE.Data;using Microsoft.AspNetCore.Mvc;using Microsoft.AspNetCore.Authorization;using Microsoft.EntityFrameworkCore;
namespace LMS_BE.Controllers;
[Route("api/grades")]
public class GradesController(ApplicationDbContext db):LmsControllerBase(db){
 [HttpGet("me")]public async Task<IActionResult> Mine()=>Ok(await Db.Submissions.Where(s=>s.StudentId==CurrentUserId).Select(s=>new{s.Id,s.AssessmentId,Assessment=s.Assessment!.Title,Course=s.Assessment.Course!.CourseName,s.Score,s.Assessment.MaxScore,s.Status,s.SubmittedAt}).ToListAsync());
 [HttpGet("class/{classId:guid}")][Authorize(Roles="Admin,Teacher")]public async Task<IActionResult> ClassGrades(Guid classId){var c=await Db.CourseClasses.FindAsync(classId);if(c==null)return NotFound();if(!IsAdmin&&c.InstructorId!=CurrentUserId)return Forbid();return Ok(await Db.Submissions.Where(s=>s.Assessment!.CourseId==c.CourseId&&Db.Enrollments.Any(e=>e.ClassId==classId&&e.StudentId==s.StudentId)).Select(s=>new{s.Id,s.StudentId,Student=s.Student!.FullName,s.AssessmentId,Assessment=s.Assessment!.Title,s.Score,s.Status}).ToListAsync());}
 public record GradeRequest(decimal Score);
 [HttpPut("{submissionId:guid}")][Authorize(Roles="Admin,Teacher")]public async Task<IActionResult> Grade(Guid submissionId,GradeRequest x){var s=await Db.Submissions.Include(s=>s.Assessment).FirstOrDefaultAsync(s=>s.Id==submissionId);if(s==null)return NotFound();if(!IsAdmin&&!await Db.CourseClasses.AnyAsync(c=>c.CourseId==s.Assessment!.CourseId&&c.InstructorId==CurrentUserId&&c.Enrollments.Any(e=>e.StudentId==s.StudentId)))return Forbid();if(x.Score<0||x.Score>s.Assessment!.MaxScore)return BadRequest("Score out of range");s.Score=x.Score;s.Status="Graded";await Db.SaveChangesAsync();return NoContent();}
}
