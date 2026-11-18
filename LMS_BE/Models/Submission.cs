namespace LMS_BE.Models;
public class Submission { public Guid Id {get;set;}=Guid.NewGuid(); public Guid AssessmentId {get;set;} public Assessment? Assessment {get;set;} public Guid StudentId {get;set;} public ApplicationUser? Student {get;set;} public decimal? Score {get;set;} public DateTimeOffset? SubmittedAt {get;set;} public string Status {get;set;}="Pending"; }
