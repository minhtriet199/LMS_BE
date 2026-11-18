namespace LMS_BE.Models;
public class Module { public Guid Id {get;set;}=Guid.NewGuid(); public Guid CourseId {get;set;} public Course? Course {get;set;} public string Title {get;set;}=""; public int SequenceNo {get;set;} public ICollection<Lesson> Lessons {get;set;}=new List<Lesson>(); }
