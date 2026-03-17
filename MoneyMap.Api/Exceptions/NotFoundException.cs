using System.Net;

namespace MoneyMap.Api.Exceptions;

public sealed class NotFoundException(string message) : AppException(message, (int)HttpStatusCode.NotFound)
{
}
