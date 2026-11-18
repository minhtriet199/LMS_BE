using LMS_BE.Data;using Microsoft.AspNetCore.Mvc;using Microsoft.EntityFrameworkCore;using Microsoft.AspNetCore.Identity;using LMS_BE.Models;
namespace LMS_BE.Controllers;
[Route("api/profile")]
public class ProfileController(ApplicationDbContext db,UserManager<ApplicationUser> manager):LmsControllerBase(db){
 [HttpGet]public async Task<IActionResult> Me(){var u=await Db.Users.Where(u=>u.Id==CurrentUserId).Select(u=>new{u.Id,u.Email,u.FullName,u.CreatedAt}).FirstOrDefaultAsync();return u==null?NotFound():Ok(u);}
 public record UpdateProfile(string FullName);
 [HttpPut]public async Task<IActionResult> Update(UpdateProfile x){if(string.IsNullOrWhiteSpace(x.FullName))return BadRequest();var u=await manager.FindByIdAsync(CurrentUserId.ToString());if(u==null)return NotFound();u.FullName=x.FullName.Trim();var r=await manager.UpdateAsync(u);return r.Succeeded?NoContent():BadRequest(r.Errors);}
 public record ChangePassword(string CurrentPassword,string NewPassword);
 [HttpPut("password")]public async Task<IActionResult> Password(ChangePassword x){var u=await manager.FindByIdAsync(CurrentUserId.ToString());if(u==null)return NotFound();var r=await manager.ChangePasswordAsync(u,x.CurrentPassword,x.NewPassword);return r.Succeeded?NoContent():BadRequest(r.Errors);}
}
