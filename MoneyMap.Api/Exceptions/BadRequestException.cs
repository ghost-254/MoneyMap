using System.Net;

namespace MoneyMap.Api.Exceptions;

public sealed class BadRequestException(string message) : AppException(message, (int)HttpStatusCode.BadRequest)
{
}
