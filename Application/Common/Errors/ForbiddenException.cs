namespace Application.Common.Errors;

public class ForbiddenException(string message) : AppException(message, 403)
{
}