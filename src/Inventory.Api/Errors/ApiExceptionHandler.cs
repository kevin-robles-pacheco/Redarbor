using Microsoft.AspNetCore.Diagnostics;
namespace Inventory.Api.Errors;
public sealed class ApiExceptionHandler : IExceptionHandler
{
    public async ValueTask<bool> TryHandleAsync(HttpContext httpContext, Exception exception, CancellationToken cancellationToken)
    {
        var statusCode = ApiExceptionMapper.GetStatusCode(exception);
        var title = ApiExceptionMapper.GetTitle(exception, statusCode);
        await Results.Problem(title: title, statusCode: statusCode).ExecuteAsync(httpContext);

        return true;
    }
}
