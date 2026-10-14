using System.Text;
using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using nwsdb_API.Data;
using nwsdb_API.Services;
using Scalar.AspNetCore;
using System.Threading.RateLimiting;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddControllers().AddJsonOptions(o => o.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter()));
builder.Services.AddOpenApi(o => { o.AddDocumentTransformer<ApiSecurityDocumentation>(); o.AddOperationTransformer<ApiSecurityDocumentation>(); });
builder.Services.AddDbContextFactory<AppDbContext>(o => o.UseSqlServer(builder.Configuration.GetConnectionString("conn")));
builder.Services.AddScoped<UserRepository>();
builder.Services.AddScoped<AccountRepository>();
builder.Services.AddScoped<BillRepository>();
builder.Services.AddScoped<PaymentRepository>();
builder.Services.AddScoped<AuthService>();
builder.Services.AddScoped<AccountService>();
builder.Services.AddScoped<BillService>();
builder.Services.AddScoped<PaymentService>();
builder.Services.AddAutoMapper(cfg => { }, typeof(Program).Assembly);
builder.Services.AddHttpClient<BankGatewayClient>(http =>
{
    http.BaseAddress = new Uri(builder.Configuration["BankApi:BaseUrl"]!);
    http.Timeout = TimeSpan.FromSeconds(15);
    http.DefaultRequestHeaders.Add("X-Service-Key", builder.Configuration["BankApi:ServiceKey"]);
});
var key = builder.Configuration["Jwt:Key"];
if (string.IsNullOrWhiteSpace(key) || key.Length < 32) throw new InvalidOperationException("Set Jwt:Key to at least 32 characters in local configuration.");
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme).AddJwtBearer(o =>
{
    o.TokenValidationParameters = new TokenValidationParameters { ValidateIssuer = true, ValidateAudience = true,
        ValidateLifetime = true, ValidateIssuerSigningKey = true, ValidIssuer = builder.Configuration["Jwt:Issuer"],
        ValidAudience = builder.Configuration["Jwt:Audience"], IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(key)), ClockSkew = TimeSpan.FromSeconds(30) };
});
builder.Services.AddAuthorization();
builder.Services.AddRateLimiter(o => o.AddPolicy("login", context => RateLimitPartition.GetFixedWindowLimiter(
    context.Connection.RemoteIpAddress?.ToString() ?? "local", _ => new FixedWindowRateLimiterOptions { PermitLimit = 15, Window = TimeSpan.FromMinutes(1), QueueLimit = 0 })));
var app = builder.Build();
if (app.Environment.IsDevelopment()) app.Services.GetRequiredService<AutoMapper.IConfigurationProvider>().AssertConfigurationIsValid();
if (args.Contains("--seed"))
{
    if (!app.Environment.IsDevelopment()) throw new InvalidOperationException("Demo seed is for Development only.");
    using var scope = app.Services.CreateScope();
    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    await db.Database.MigrateAsync();
    await DemoData.SeedAsync(db, builder.Configuration["Seed:Password"] ?? throw new InvalidOperationException("Set Seed:Password."));
    return;
}
app.UseApiErrors();
if (app.Environment.IsDevelopment()) { app.MapOpenApi(); app.MapScalarApiReference(); }
app.UseHttpsRedirection();
app.UseRateLimiter();
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();
app.Run();
