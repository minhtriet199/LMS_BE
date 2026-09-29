using System.Security.Cryptography;
using System.Text;
using LMS_BE.Data;
using LMS_BE.Models;
using Microsoft.EntityFrameworkCore;

namespace LMS_BE.Services;

public class RefreshTokenService(ApplicationDbContext db, JwtOptions options)
{
    public async Task<string> IssueAsync(Guid userId)
    {
        var now = DateTimeOffset.UtcNow;
        await db.RefreshTokens
            .Where(t => t.UserId == userId && t.ExpiresAt < now)
            .ExecuteDeleteAsync();

        var raw = GenerateRawToken();
        db.RefreshTokens.Add(new RefreshToken
        {
            UserId = userId,
            TokenHash = Hash(raw),
            ExpiresAt = now.AddDays(options.RefreshTokenExpiresDays),
        });
        await db.SaveChangesAsync();
        return raw;
    }

    public async Task<(ApplicationUser User, string NewRawToken)?> RotateAsync(string rawToken)
    {
        var now = DateTimeOffset.UtcNow;
        var hash = Hash(rawToken);
        var existing = await db.RefreshTokens
            .Include(t => t.User)
            .FirstOrDefaultAsync(t => t.TokenHash == hash);

        if (existing is null || existing.ExpiresAt <= now) return null;

        if (existing.RevokedAt is not null)
        {
            await RevokeAllForUserAsync(existing.UserId);
            return null;
        }

        var revoked = await db.RefreshTokens
            .Where(t => t.Id == existing.Id && t.RevokedAt == null)
            .ExecuteUpdateAsync(s => s.SetProperty(t => t.RevokedAt, now));
        if (revoked == 0)
        {
            await RevokeAllForUserAsync(existing.UserId);
            return null;
        }

        var raw = await IssueAsync(existing.UserId);
        return (existing.User, raw);
    }

    public async Task RevokeAsync(string rawToken)
    {
        var hash = Hash(rawToken);
        await db.RefreshTokens
            .Where(t => t.TokenHash == hash && t.RevokedAt == null)
            .ExecuteUpdateAsync(s => s.SetProperty(t => t.RevokedAt, DateTimeOffset.UtcNow));
    }

    private async Task RevokeAllForUserAsync(Guid userId) =>
        await db.RefreshTokens
            .Where(t => t.UserId == userId && t.RevokedAt == null)
            .ExecuteUpdateAsync(s => s.SetProperty(t => t.RevokedAt, DateTimeOffset.UtcNow));

    private static string GenerateRawToken() => Convert.ToBase64String(RandomNumberGenerator.GetBytes(64));

    private static string Hash(string raw) => Convert.ToBase64String(SHA256.HashData(Encoding.UTF8.GetBytes(raw)));
}
