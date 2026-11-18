namespace LMS_BE.Models;
public class LearningActivity { public Guid Id {get;set;}=Guid.NewGuid(); public Guid StudentId {get;set;} public ApplicationUser? Student {get;set;} public Guid? LessonId {get;set;} public Lesson? Lesson {get;set;} public string ActivityType {get;set;}="view"; public int DurationSeconds {get;set;} public DateTimeOffset OccurredAt {get;set;}=DateTimeOffset.UtcNow; }
