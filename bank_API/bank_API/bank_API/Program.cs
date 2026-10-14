using bank_API.Data;
using bank_API.Services;
using Microsoft.AspNetCore.Authentication;
using Microsoft.EntityFrameworkCore;
using Scalar.AspNetCore;
using System.Text.Json.Serialization;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddControllers().AddJsonOptions(o => o.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter()));
builder.Services.AddOpenApi(o => { o.AddDocumentTransformer<ApiSecurityDocumentation>(); o.AddOperationTransformer<ApiSecurityDocumentation>(); });
builder.Services.AddDbContext<BankDbContext>(o => o.UseSqlServer(builder.Configuration.GetConnectionString("conn")));
builder.Services.AddScoped<BankTransactionRepository>();
builder.Services.AddScoped<BankPaymentService>();
builder.Services.AddAutoMapper(cfg => { }, typeof(Program).Assembly);
builder.Services.AddAuthentication("ServiceKey").AddScheme<AuthenticationSchemeOptions, ServiceAuthenticationHandler>("ServiceKey", null);
builder.Services.AddAuthorization();
var app = builder.Build();
if (app.Environment.IsDevelopment()) app.Services.GetRequiredService<AutoMapper.IConfigurationProvider>().AssertConfigurationIsValid();
if (args.Contains("--migrate"))
{
    using var scope = app.Services.CreateScope();
    await scope.ServiceProvider.GetRequiredService<BankDbContext>().Database.MigrateAsync();
    return;
}
app.UseExceptionHandler(error => error.Run(async context =>
{
    var exception = context.Features.Get<Microsoft.AspNetCore.Diagnostics.IExceptionHandlerFeature>()!.Error;
    var status = exception switch { ArgumentException => 400, UnauthorizedAccessException => 403, KeyNotFoundException => 404, InvalidOperationException => 409, _ => 503 };
    if (status == 503) app.Logger.LogError(exception, "Request could not be completed.");
    await Results.Problem(statusCode: status, title: status == 503 ? "Service unavailable. Check the original payment status before retrying." : exception.Message).ExecuteAsync(context);
}));
if (app.Environment.IsDevelopment()) { app.MapOpenApi(); app.MapScalarApiReference(); }
app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();
app.Run();
