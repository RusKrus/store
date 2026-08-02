namespace Application.Common.Errors;

public class UnauthorizedException(string message = "User is not authenticated") : AppException(message, 401)
{
}