namespace LMS_BE.Models;
public class Notification { public Guid Id {get;set;}=Guid.NewGuid(); public Guid UserId {get;set;} public ApplicationUser? User {get;set;} public string Title {get;set;}=""; public string Body {get;set;}=""; public bool IsRead {get;set;} public DateTimeOffset CreatedAt {get;set;}=DateTimeOffset.UtcNow; }
