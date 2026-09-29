using LMS_BE.Dtos;
using LMS_BE.Models;
using LMS_BE.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace LMS_BE.Controllers;

[ApiController]
[Route("api/auth")]
public class AuthController(
    UserManager<ApplicationUser> userManager,
    JwtTokenService jwt,
    RefreshTokenService refreshTokens,
    JwtOptions jwtOptions,
    IWebHostEnvironment env) : ControllerBase
{
    private const string RefreshCookieName = "refreshToken";
    private const string CookiePath = "/api/auth";
    private const string CsrfHeader = "X-Requested-With";
    private const string CsrfHeaderValue = "XMLHttpRequest";

    [HttpPost("login")]
    public async Task<ActionResult<AuthResponse>> Login(LoginRequest request)
    {
        if (!HasCsrfHeader()) return BadRequest();

        var user = await userManager.FindByEmailAsync(request.Email);
        if (user is null || await userManager.IsLockedOutAsync(user)) return Unauthorized();

        if (!await userManager.CheckPasswordAsync(user, request.Password))
        {
            await userManager.AccessFailedAsync(user);
            return Unauthorized();
        }
        await userManager.ResetAccessFailedCountAsync(user);

        var response = await CreateAuthResponseAsync(user);
        if (response is null) return Unauthorized();

        SetRefreshCookie(await refreshTokens.IssueAsync(user.Id));
        return response;
    }

    [HttpPost("refresh")]
    public async Task<ActionResult<AuthResponse>> Refresh()
    {
        if (!HasCsrfHeader()) return BadRequest();

        var raw = Request.Cookies[RefreshCookieName];
        if (string.IsNullOrEmpty(raw)) return Unauthorized();

        var result = await refreshTokens.RotateAsync(raw);
        if (result is null)
        {
            DeleteRefreshCookie();
            return Unauthorized();
        }

        var (user, newRaw) = result.Value;
        var response = await CreateAuthResponseAsync(user);
        if (response is null)
        {
            DeleteRefreshCookie();
            return Unauthorized();
        }

        SetRefreshCookie(newRaw);
        return response;
    }

    [HttpPost("logout")]
    public async Task<IActionResult> Logout()
    {
        if (!HasCsrfHeader()) return BadRequest();

        var raw = Request.Cookies[RefreshCookieName];
        if (!string.IsNullOrEmpty(raw))
            await refreshTokens.RevokeAsync(raw);

        DeleteRefreshCookie();
        return NoContent();
    }

    private async Task<AuthResponse?> CreateAuthResponseAsync(ApplicationUser user)
    {
        var role = (await userManager.GetRolesAsync(user)).FirstOrDefault();
        if (role is null) return null;

        var token = jwt.CreateToken(user, role);
        return new AuthResponse(token, new UserDto(user.Id, user.FullName, user.Email!, role));
    }

    private bool HasCsrfHeader() => Request.Headers[CsrfHeader] == CsrfHeaderValue;

    private CookieOptions RefreshCookieOptions() => new()
    {
        HttpOnly = true,
        Secure = !env.IsDevelopment(),
        SameSite = SameSiteMode.Strict,
        Path = CookiePath,
    };

    private void SetRefreshCookie(string token)
    {
        var options = RefreshCookieOptions();
        options.Expires = DateTimeOffset.UtcNow.AddDays(jwtOptions.RefreshTokenExpiresDays);
        Response.Cookies.Append(RefreshCookieName, token, options);
    }

    private void DeleteRefreshCookie() => Response.Cookies.Delete(RefreshCookieName, RefreshCookieOptions());
}
