using LMS_BE.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
namespace LMS_BE.Data;
public class ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : IdentityDbContext<ApplicationUser,ApplicationRole,Guid>(options)
{
 public DbSet<RefreshToken> RefreshTokens => Set<RefreshToken>();
 public DbSet<Course> Courses => Set<Course>(); public DbSet<CourseClass> CourseClasses => Set<CourseClass>();
 public DbSet<Enrollment> Enrollments => Set<Enrollment>(); public DbSet<Module> Modules => Set<Module>();
 public DbSet<Lesson> Lessons => Set<Lesson>(); public DbSet<Assessment> Assessments => Set<Assessment>();
 public DbSet<Question> Questions => Set<Question>(); public DbSet<Submission> Submissions => Set<Submission>();
 public DbSet<LearningActivity> LearningActivities => Set<LearningActivity>();
 public DbSet<LearningPath> LearningPaths => Set<LearningPath>(); public DbSet<LearningPathItem> LearningPathItems => Set<LearningPathItem>();
 public DbSet<RiskPrediction> RiskPredictions => Set<RiskPrediction>(); public DbSet<ClassSession> ClassSessions => Set<ClassSession>();
 public DbSet<Notification> Notifications => Set<Notification>();
 protected override void OnModelCreating(ModelBuilder b) {
  base.OnModelCreating(b);
  b.Entity<ApplicationUser>().ToTable("Users"); b.Entity<ApplicationRole>().ToTable("Roles");
  b.Entity<IdentityUserRole<Guid>>().ToTable("UserRoles"); b.Entity<IdentityUserClaim<Guid>>().ToTable("UserClaims");
  b.Entity<IdentityUserLogin<Guid>>().ToTable("UserLogins"); b.Entity<IdentityUserToken<Guid>>().ToTable("UserTokens");
  b.Entity<IdentityRoleClaim<Guid>>().ToTable("RoleClaims");
  b.Entity<RefreshToken>(e=>{e.HasIndex(x=>x.TokenHash).IsUnique();e.HasOne(x=>x.User).WithMany().HasForeignKey(x=>x.UserId).OnDelete(DeleteBehavior.Cascade);});
  b.Entity<Course>(e=>{e.HasIndex(x=>x.CourseCode).IsUnique();e.HasOne(x=>x.CreatedBy).WithMany().HasForeignKey(x=>x.CreatedById).OnDelete(DeleteBehavior.Restrict);});
  b.Entity<CourseClass>(e=>e.HasOne(x=>x.Instructor).WithMany().HasForeignKey(x=>x.InstructorId).OnDelete(DeleteBehavior.Restrict));
  b.Entity<Enrollment>(e=>{e.HasIndex(x=>new{x.ClassId,x.StudentId}).IsUnique();e.HasOne(x=>x.Student).WithMany().HasForeignKey(x=>x.StudentId).OnDelete(DeleteBehavior.Restrict);});
  b.Entity<Submission>(e=>{e.HasIndex(x=>new{x.AssessmentId,x.StudentId}).IsUnique();e.HasOne(x=>x.Student).WithMany().HasForeignKey(x=>x.StudentId).OnDelete(DeleteBehavior.Restrict);e.Property(x=>x.Score).HasPrecision(6,2);});
  b.Entity<Assessment>().Property(x=>x.MaxScore).HasPrecision(6,2); b.Entity<Question>().Property(x=>x.Points).HasPrecision(6,2);
  b.Entity<LearningPathItem>().Property(x=>x.PriorityScore).HasPrecision(7,4);
  b.Entity<RiskPrediction>().Property(x=>x.RiskScore).HasPrecision(5,2);
  b.Entity<LearningActivity>().HasIndex(x=>new{x.StudentId,x.OccurredAt});
  b.Entity<Notification>().HasIndex(x=>new{x.UserId,x.CreatedAt});
 }
}
