using Microsoft.AspNetCore.Diagnostics;
using ObiletCase.Core.Exceptions.Base;
using ObiletCase.Web.Models;
using System.Net;

namespace ObiletCase.Web.Middlewares;

public sealed class GlobalExceptionHandler : IExceptionHandler
{
    private readonly ILogger<GlobalExceptionHandler> _logger;

    public GlobalExceptionHandler(ILogger<GlobalExceptionHandler> logger)
    {
        _logger = logger;
    }

    public async ValueTask<bool> TryHandleAsync(
        HttpContext httpContext,
        System.Exception exception,
        CancellationToken cancellationToken)
    {
        _logger.LogError(exception, "Beklenmeyen bir hata oluştu: {Message}", exception.Message);

        var (statusCode, title, detail, props) = exception switch
        {
            CustomBaseException customEx => (
                (int)customEx.StatusCode,
                customEx.Title,
                customEx.Message,
                customEx.MessageProps
            ),
            _ => (
                (int)HttpStatusCode.InternalServerError,
                "Internal Server Error",
                "Sunucu tarafında beklenmeyen bir hata oluştu.",
                null
            )
        };

        httpContext.Response.StatusCode = statusCode;
        httpContext.Response.ContentType = "application/json";

        var response = new ErrorResponse
        {
            Title = title,
            Status = statusCode,
            Detail = detail,
            Props = props
        };

        await httpContext.Response.WriteAsJsonAsync(response, cancellationToken);

        return true;
    }
}