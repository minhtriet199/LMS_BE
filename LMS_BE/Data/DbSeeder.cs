using LMS_BE.Models;
using LMS_BE.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace LMS_BE.Data;

public static class DbSeeder
{
    public const string DefaultPassword = "Lms@12345";

    public static async Task SeedAsync(IServiceProvider services)
    {
        var db = services.GetRequiredService<ApplicationDbContext>();
        var userManager = services.GetRequiredService<UserManager<ApplicationUser>>();
        var roleManager = services.GetRequiredService<RoleManager<ApplicationRole>>();

        await db.Database.MigrateAsync();

        foreach (var role in Roles.All)
        {
            if (!await roleManager.RoleExistsAsync(role))
                await roleManager.CreateAsync(new ApplicationRole { Name = role });
        }

        // Seed từng tài khoản độc lập, không bỏ qua nếu database đã có user khác.

        await EnsureUserAsync(userManager, "student@lms.dev", "Nguyễn Văn An", Roles.Student);
        await EnsureUserAsync(userManager, "teacher@lms.dev", "Trần Thị Bình", Roles.Teacher);
        await EnsureUserAsync(userManager, "admin@lms.dev", "Lê Quản Trị", Roles.Admin);
    }

    private static async Task<ApplicationUser> EnsureUserAsync(
        UserManager<ApplicationUser> manager, string email, string fullName, string role)
    {
        var existing = await manager.FindByEmailAsync(email);
        if (existing != null) {
            if (!await manager.IsInRoleAsync(existing, role)) await manager.AddToRoleAsync(existing, role);
            return existing; // Không tự ý reset mật khẩu của tài khoản đã tồn tại.
        }
        return await CreateUserAsync(manager,email,fullName,role);
    }
    private static async Task<ApplicationUser> CreateUserAsync(
        UserManager<ApplicationUser> userManager, string email, string fullName, string role)
    {
        var user = new ApplicationUser { UserName = email, Email = email, EmailConfirmed = true, FullName = fullName };
        var result = await userManager.CreateAsync(user, DefaultPassword);
        if (!result.Succeeded)
            throw new InvalidOperationException(string.Join("; ", result.Errors.Select(e => e.Description)));
        await userManager.AddToRoleAsync(user, role);
        return user;
    }
}
