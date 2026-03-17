using System.Net;

namespace MoneyMap.Api.Exceptions;

public sealed class UnauthorizedException(string message) : AppException(message, (int)HttpStatusCode.Unauthorized)
{
}
