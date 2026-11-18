namespace LMS_BE.Models;
public class LearningPathItem { public Guid Id {get;set;}=Guid.NewGuid(); public Guid PathId {get;set;} public LearningPath? Path {get;set;} public Guid LessonId {get;set;} public Lesson? Lesson {get;set;} public int SequenceNo {get;set;} public decimal PriorityScore {get;set;} public string Status {get;set;}="Recommended"; }
