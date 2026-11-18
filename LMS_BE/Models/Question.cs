namespace LMS_BE.Models;
public class Question { public Guid Id {get;set;}=Guid.NewGuid(); public Guid AssessmentId {get;set;} public Assessment? Assessment {get;set;} public string QuestionText {get;set;}=""; public string QuestionType {get;set;}="text"; public decimal Points {get;set;}=1; }
