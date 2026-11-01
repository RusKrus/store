namespace Application.Common.Errors;

public class NotFoundException(string message = "Not found.") : AppException(message, 404)
{
}