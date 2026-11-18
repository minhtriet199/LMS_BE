namespace LMS_BE.Models;
public class Lesson { public Guid Id {get;set;}=Guid.NewGuid(); public Guid ModuleId {get;set;} public Module? Module {get;set;} public string Title {get;set;}=""; public string ContentType {get;set;}="text"; public string ContentUrl {get;set;}=""; public string Content {get;set;}=""; public int SequenceNo {get;set;} public int EstimatedMinutes {get;set;}=30; }
