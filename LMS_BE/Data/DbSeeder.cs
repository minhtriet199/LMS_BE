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

        if (await db.Users.AnyAsync()) return;

        await CreateUserAsync(userManager, "student@lms.dev", "Nguyễn Văn An", Roles.Student);
        await CreateUserAsync(userManager, "teacher@lms.dev", "Trần Thị Bình", Roles.Teacher);
        await CreateUserAsync(userManager, "admin@lms.dev", "Lê Quản Trị", Roles.Admin);
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
