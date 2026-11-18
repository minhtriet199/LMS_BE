using LMS_BE.Data;using LMS_BE.Models;using Microsoft.AspNetCore.Mvc;using Microsoft.AspNetCore.Authorization;using Microsoft.EntityFrameworkCore;
namespace LMS_BE.Controllers;
[Route("api/assessments")]
public class AssessmentsController(ApplicationDbContext db):LmsControllerBase(db){
 [HttpGet("course/{courseId:guid}")]public async Task<IActionResult> List(Guid courseId){if(!await Allowed(courseId))return Forbid();return Ok(await Db.Assessments.Where(a=>a.CourseId==courseId).Select(a=>new{a.Id,a.Title,a.AssessmentType,a.MaxScore,a.DueAt}).ToListAsync());}
 public record CreateAssessment(Guid CourseId,string Title,string AssessmentType,decimal MaxScore,DateTimeOffset? DueAt);
 [HttpPost][Authorize(Roles="Admin,Teacher")]public async Task<IActionResult> Create(CreateAssessment x){if(x.MaxScore<=0)return BadRequest();if(!IsAdmin&&!await Db.CourseClasses.AnyAsync(c=>c.CourseId==x.CourseId&&c.InstructorId==CurrentUserId))return Forbid();var a=new Assessment{CourseId=x.CourseId,Title=x.Title,AssessmentType=x.AssessmentType,MaxScore=x.MaxScore,DueAt=x.DueAt};Db.Assessments.Add(a);await Db.SaveChangesAsync();return Ok(new{a.Id});}
 public record SubmitRequest();
 [HttpPost("{id:guid}/submit")][Authorize(Roles="Student")]public async Task<IActionResult> Submit(Guid id){var a=await Db.Assessments.FindAsync(id);if(a==null)return NotFound();if(!await Db.Enrollments.AnyAsync(e=>e.StudentId==CurrentUserId&&e.Class!.CourseId==a.CourseId))return Forbid();if(await Db.Submissions.AnyAsync(s=>s.AssessmentId==id&&s.StudentId==CurrentUserId))return Conflict("Already submitted");var s=new Submission{AssessmentId=id,StudentId=CurrentUserId,SubmittedAt=DateTimeOffset.UtcNow,Status="Submitted"};Db.Submissions.Add(s);await Db.SaveChangesAsync();return Ok(new{s.Id});}
 private async Task<bool> Allowed(Guid id)=>IsAdmin||await Db.CourseClasses.AnyAsync(c=>c.CourseId==id&&(c.InstructorId==CurrentUserId||c.Enrollments.Any(e=>e.StudentId==CurrentUserId)));
}
