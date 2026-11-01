namespace Application.Common.Errors;

public class ConflictException(string message) : AppException(message, 409)
{
}