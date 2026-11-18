namespace LMS_BE.Models;
public class Enrollment { public Guid Id {get;set;}=Guid.NewGuid(); public Guid ClassId {get;set;} public CourseClass? Class {get;set;} public Guid StudentId {get;set;} public ApplicationUser? Student {get;set;} public DateTimeOffset EnrolledAt {get;set;}=DateTimeOffset.UtcNow; public string Status {get;set;}="Active"; }
