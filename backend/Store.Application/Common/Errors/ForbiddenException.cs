namespace Application.Common.Errors;

public class ForbiddenException(string message = "This action is not allowed.") : AppException(message, 403)
{
}