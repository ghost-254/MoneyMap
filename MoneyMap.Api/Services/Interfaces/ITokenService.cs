using MoneyMap.Api.Models;

namespace MoneyMap.Api.Services.Interfaces;

public interface ITokenService
{
    TokenResult GenerateToken(ApplicationUser user);
}
