using System.Net;

namespace MoneyMap.Api.Exceptions;

public sealed class ConflictException(string message) : AppException(message, (int)HttpStatusCode.Conflict)
{
}
