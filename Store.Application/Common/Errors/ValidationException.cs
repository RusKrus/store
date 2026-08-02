namespace Application.Common.Errors;

public class ValidationException(string message) : AppException(message, 400)
{
}