using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.OpenApi;
using Microsoft.OpenApi;

namespace nwsdb_API.Services;

// Documentation only: the authentication/authorization middleware enforces access.
public sealed class ApiSecurityDocumentation : IOpenApiDocumentTransformer, IOpenApiOperationTransformer
{
    public Task TransformAsync(OpenApiDocument document, OpenApiDocumentTransformerContext context, CancellationToken cancellationToken)
    {
        document.Info.Title = "NWSDB API";
        document.Info.Description = "Classroom water billing services. Sign in using POST /api/auth/login, copy accessToken, then paste it into Bearer authentication. Staff: accounts, readings and billing. Customer: own records. PartnerCashier: bill lookup and simulated payments. Sample tariff and synthetic data only.";
        document.Components ??= new OpenApiComponents();
        document.Components.SecuritySchemes ??= new Dictionary<string, IOpenApiSecurityScheme>();
        document.Components.SecuritySchemes["Bearer"] = new OpenApiSecurityScheme
        {
            Type = SecuritySchemeType.Http,
            Scheme = "bearer",
            BearerFormat = "JWT",
            Description = "Paste only the accessToken returned by login, without the Bearer prefix."
        };
        return Task.CompletedTask;
    }

    public Task TransformAsync(OpenApiOperation operation, OpenApiOperationTransformerContext context, CancellationToken cancellationToken)
    {
        var metadata = context.Description.ActionDescriptor.EndpointMetadata;
        if (metadata.OfType<IAllowAnonymous>().Any() || !metadata.OfType<IAuthorizeData>().Any())
            return Task.CompletedTask;

        operation.Security ??= [];
        operation.Security.Add(new OpenApiSecurityRequirement
        {
            [new OpenApiSecuritySchemeReference("Bearer", context.Document)] = []
        });
        operation.Responses ??= new OpenApiResponses();
        operation.Responses.TryAdd("401", new OpenApiResponse { Description = "Missing, invalid or expired access token." });
        operation.Responses.TryAdd("403", new OpenApiResponse { Description = "The authenticated role cannot use this endpoint." });
        return Task.CompletedTask;
    }
}
