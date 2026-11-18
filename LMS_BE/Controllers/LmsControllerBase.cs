using System.Security.Claims;
using LMS_BE.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
namespace LMS_BE.Controllers;
[ApiController][Authorize]
public abstract class LmsControllerBase(ApplicationDbContext db) : ControllerBase {
 protected ApplicationDbContext Db => db;
 protected Guid CurrentUserId => Guid.Parse(User.FindFirstValue("sub") ?? throw new UnauthorizedAccessException("Missing subject claim"));
 protected bool IsAdmin => User.IsInRole("Admin"); protected bool IsTeacher => User.IsInRole("Teacher");
}
