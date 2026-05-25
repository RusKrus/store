namespace Application.Common.Errors;

public class UnauthorizedException(string message) : AppException(message, 401)
{
}