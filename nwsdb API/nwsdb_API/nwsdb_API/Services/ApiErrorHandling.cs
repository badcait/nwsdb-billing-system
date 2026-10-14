using nwsdb_API.Model;
namespace nwsdb_API.Services;
public static class ApiErrorHandling
{
    public static void UseApiErrors(this WebApplication app)
    {
app.UseExceptionHandler(error => error.Run(async context =>
{
    var exception = context.Features.Get<Microsoft.AspNetCore.Diagnostics.IExceptionHandlerFeature>()!.Error;
    var status = exception switch { PaymentConflictException => 409, ArgumentException => 400, UnauthorizedAccessException => 403, KeyNotFoundException => 404, InvalidOperationException => 409, _ => 503 };
    if (status == 503) app.Logger.LogError(exception, "Request could not be completed.");
    var extensions = exception is PaymentConflictException conflict ? new Dictionary<string, object?> { ["code"] = conflict.Code } : null;
    await Results.Problem(extensions: extensions, statusCode: status, title: status == 503 ? "Service unavailable. Check the original payment status before retrying." : exception.Message).ExecuteAsync(context);
}));
    }
}
