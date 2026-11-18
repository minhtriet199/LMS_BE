using LMS_BE.Models;using LMS_BE.Services;using Microsoft.AspNetCore.Identity;using Microsoft.EntityFrameworkCore;
namespace LMS_BE.Data;
public static class DemoDataSeeder{
 public static async Task SeedAsync(IServiceProvider services){var db=services.GetRequiredService<ApplicationDbContext>();var users=services.GetRequiredService<UserManager<ApplicationUser>>();var teacher=await users.FindByEmailAsync("teacher@lms.dev");var student=await users.FindByEmailAsync("student@lms.dev");var admin=await users.FindByEmailAsync("admin@lms.dev");if(teacher==null||student==null||admin==null)return;
 var seed=new[]{("CS301","Cấu trúc dữ liệu và giải thuật"),("DB202","Cơ sở dữ liệu"),("SE204","Phân tích và thiết kế phần mềm")};
 foreach(var (code,title) in seed){var c=await db.Courses.FirstOrDefaultAsync(c=>c.CourseCode==code);if(c==null){c=new Course{CourseCode=code,CourseName=title,Description="Demo LMS course",CreatedById=admin.Id};db.Courses.Add(c);await db.SaveChangesAsync();}
 var cls=await db.CourseClasses.FirstOrDefaultAsync(x=>x.CourseId==c.Id&&x.ClassName==code+"-01");if(cls==null){cls=new CourseClass{CourseId=c.Id,InstructorId=teacher.Id,ClassName=code+"-01",Semester="2026-1",StartDate=new DateOnly(2026,9,1),EndDate=new DateOnly(2027,1,15)};db.CourseClasses.Add(cls);await db.SaveChangesAsync();}
 if(!await db.Enrollments.AnyAsync(e=>e.ClassId==cls.Id&&e.StudentId==student.Id)){db.Enrollments.Add(new Enrollment{ClassId=cls.Id,StudentId=student.Id});await db.SaveChangesAsync();}
 if(!await db.Modules.AnyAsync(m=>m.CourseId==c.Id)){var m=new Module{CourseId=c.Id,Title="Kiến thức cơ bản",SequenceNo=1};m.Lessons.Add(new Lesson{Title="Giới thiệu môn học",Content="Nội dung giới thiệu",SequenceNo=1,EstimatedMinutes=30});m.Lessons.Add(new Lesson{Title="Bài học thực hành",Content="Nội dung thực hành",SequenceNo=2,EstimatedMinutes=45});db.Modules.Add(m);await db.SaveChangesAsync();}
 }
 }
}
