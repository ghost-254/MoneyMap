using System.ComponentModel.DataAnnotations;

namespace MoneyMap.Api.DTOs.Auth;

public sealed class RegisterRequestDto
{
    [Required]
    [MaxLength(100)]
    public string FullName { get; init; } = string.Empty;

    [Required]
    [EmailAddress]
    public string Email { get; init; } = string.Empty;

    [Required]
    [MinLength(8)]
    public string Password { get; init; } = string.Empty;
}
