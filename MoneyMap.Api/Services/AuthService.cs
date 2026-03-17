using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using MoneyMap.Api.Data;
using MoneyMap.Api.DTOs.Auth;
using MoneyMap.Api.Exceptions;
using MoneyMap.Api.Models;
using MoneyMap.Api.Services.Interfaces;

namespace MoneyMap.Api.Services;

public sealed class AuthService(
    AppDbContext dbContext,
    IPasswordHasher<ApplicationUser> passwordHasher,
    ITokenService tokenService) : IAuthService
{
    public async Task<AuthResponseDto> RegisterAsync(RegisterRequestDto request, CancellationToken cancellationToken = default)
    {
        var normalizedEmail = NormalizeEmail(request.Email);

        var emailExists = await dbContext.Users
            .AsNoTracking()
            .AnyAsync(user => user.Email == normalizedEmail, cancellationToken);

        if (emailExists)
        {
            throw new ConflictException("An account with this email already exists.");
        }

        var user = new ApplicationUser
        {
            FullName = request.FullName.Trim(),
            Email = normalizedEmail
        };

        user.PasswordHash = passwordHasher.HashPassword(user, request.Password);

        dbContext.Users.Add(user);
        await dbContext.SaveChangesAsync(cancellationToken);

        return BuildAuthResponse(user);
    }

    public async Task<AuthResponseDto> LoginAsync(LoginRequestDto request, CancellationToken cancellationToken = default)
    {
        var normalizedEmail = NormalizeEmail(request.Email);

        var user = await dbContext.Users
            .SingleOrDefaultAsync(candidate => candidate.Email == normalizedEmail, cancellationToken)
            ?? throw new UnauthorizedException("Invalid email or password.");

        var verificationResult = passwordHasher.VerifyHashedPassword(user, user.PasswordHash, request.Password);
        if (verificationResult == PasswordVerificationResult.Failed)
        {
            throw new UnauthorizedException("Invalid email or password.");
        }

        return BuildAuthResponse(user);
    }

    private AuthResponseDto BuildAuthResponse(ApplicationUser user)
    {
        var token = tokenService.GenerateToken(user);

        return new AuthResponseDto
        {
            AccessToken = token.AccessToken,
            ExpiresAtUtc = token.ExpiresAtUtc,
            User = new AuthenticatedUserDto
            {
                Id = user.Id,
                FullName = user.FullName,
                Email = user.Email
            }
        };
    }

    private static string NormalizeEmail(string email) => email.Trim().ToLowerInvariant();
}
