using Template.Application.Common.Models;
using System.Net;
using System.Text.Json;

namespace Template.Api.Middlewares;

public class ExceptionHandlingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<ExceptionHandlingMiddleware> _logger;
    private readonly IWebHostEnvironment _environment;

    public ExceptionHandlingMiddleware(RequestDelegate next, ILogger<ExceptionHandlingMiddleware> logger, IWebHostEnvironment environment)
    {
        _next = next;
        _logger = logger;
        _environment = environment;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An unhandled exception occurred.");
            await HandleExceptionAsync(context, ex);
        }
    }

    private Task HandleExceptionAsync(HttpContext context, Exception ex)
    {
        context.Response.ContentType = "application/json";
        context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;

        ErrorResponse<string> response;

        if (_environment.IsDevelopment())
        {
            response = new ErrorResponse<string>(
                "Ocorreu um erro interno no servidor. Veja os detalhes abaixo.",
                statusCode: 500,
                data: null,
                erros: new List<NotificationError>
                {
                    new NotificationError("Exception", ex.Message),
                    new NotificationError("StackTrace", ex.StackTrace ?? string.Empty)
                }
            );
        }
        else
        {
            response = new ErrorResponse<string>(
                "Ocorreu um erro interno no servidor. Por favor, tente novamente mais tarde ou consulte o suporte.",
                statusCode: 500,
                data: null,
                erros: new List<NotificationError>
                {
                    new NotificationError("Exception", ex.Message)
                }
            );
        }

        return context.Response.WriteAsync(JsonSerializer.Serialize(response));
    }
}