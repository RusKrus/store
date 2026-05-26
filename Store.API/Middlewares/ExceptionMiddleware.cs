using System.Text.Json;
using Application.Common.Errors;

namespace Store.Api.Middlewares;

// TODO: add logging
public class ExceptionMiddleware(RequestDelegate next)
{
  public async Task InvokeAsync(HttpContext context)
  {
    try
    {
      await next(context);
    }
    catch (AppException ex)
    {
      await HandleAppException(context, ex);
    } 
    catch (Exception ex)
    {
      context.Response.StatusCode = 500;
      context.Response.ContentType = "application/json";
      var response = new { error = "Internal server error" };
      await context.Response.WriteAsync(JsonSerializer.Serialize(response));
    }
  }

  private async Task HandleAppException(HttpContext context, AppException ex)
  {
    context.Response.StatusCode = ex.StatusCode;
    context.Response.ContentType = "application/json";
    var response = new { error = ex.Message };
    await context.Response.WriteAsync(JsonSerializer.Serialize(response));
  }
}