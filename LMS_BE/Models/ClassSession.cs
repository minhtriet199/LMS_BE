namespace LMS_BE.Models;
public class ClassSession { public Guid Id {get;set;}=Guid.NewGuid(); public Guid ClassId {get;set;} public CourseClass? Class {get;set;} public string Title {get;set;}=""; public DateTimeOffset StartsAt {get;set;} public DateTimeOffset EndsAt {get;set;} public string Room {get;set;}=""; public string SessionType {get;set;}="Lecture"; }
