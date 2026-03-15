using FluentValidation;
using Microsoft.AspNetCore.Diagnostics;
using ms_transferencias.Application.Common.Models;

public class GlobalExceptionHandler : IExceptionHandler
{
    public async ValueTask<bool> TryHandleAsync(HttpContext httpContext, Exception exception, CancellationToken ct)
    {
        if (exception is not ValidationException validationException)
        {
            return false;
        }
        var errorResponse = new ErrorResponse
        {
            Message = validationException.Errors.Select(e => e.ErrorMessage).FirstOrDefault() ?? "Error Interno"
        };

        httpContext.Response.StatusCode = StatusCodes.Status400BadRequest;
        await httpContext.Response.WriteAsJsonAsync(errorResponse, ct);

        return true;
    }
}