using System.Text.Json;
using Application.Common.Errors;
using Store.API.Contracts.Response;

namespace Store.Api.Middlewares;

public class ExceptionMiddleware(RequestDelegate next, ILogger<ExceptionMiddleware> logger, IWebHostEnvironment env)
{
  public async Task InvokeAsync(HttpContext context)
  {
    try
    {
      await next(context);
    }
    catch (AppException ex)
    {
      logger.LogWarning(ex, ex.Message);
      await HandleAppException(context, ex);
    }
    catch (Exception ex)
    {
      logger.LogError(ex, ex.Message);
      await HandleServerErrorException(context, ex);
    }
  }

  private async Task HandleServerErrorException(HttpContext context, Exception exception)
  {
    context.Response.StatusCode = 500;
    context.Response.ContentType = "application/json";
    var response = env.IsDevelopment() ? new ErrorResponse(exception.Message, exception.StackTrace) : new ErrorResponse("Internal server error");
    await context.Response.WriteAsync(JsonSerializer.Serialize(response));
  }

private async Task HandleAppException(HttpContext context, AppException ex)
  {
    context.Response.StatusCode = ex.StatusCode;
    context.Response.ContentType = "application/json";
    var response = new { error = ex.Message };
    await context.Response.WriteAsync(JsonSerializer.Serialize(response));
  }
}