namespace Application.Common.Errors;

public class NotFoundException(string message) : AppException(message, 404)
{
}