using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.OpenApi;
using Microsoft.OpenApi;

namespace bank_API.Services;

// Documentation only: the authentication/authorization middleware enforces access.
public sealed class ApiSecurityDocumentation : IOpenApiDocumentTransformer, IOpenApiOperationTransformer
{
    public Task TransformAsync(OpenApiDocument document, OpenApiDocumentTransformerContext context, CancellationToken cancellationToken)
    {
        document.Info.Title = "Bank API";
        document.Info.Description = "Payment authorisation and saved outcomes only. Requires the NWSDB service credential. Demonstration payments do not move real money.";
        document.Components ??= new OpenApiComponents();
        document.Components.SecuritySchemes ??= new Dictionary<string, IOpenApiSecurityScheme>();
        document.Components.SecuritySchemes["ServiceKey"] = new OpenApiSecurityScheme
        {
            Type = SecuritySchemeType.ApiKey,
            Name = "X-Service-Key",
            In = ParameterLocation.Header,
            
            
            Description = "Use the service key configured in both APIs."
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
            [new OpenApiSecuritySchemeReference("ServiceKey", context.Document)] = []
        });
        operation.Responses ??= new OpenApiResponses();
        operation.Responses.TryAdd("401", new OpenApiResponse { Description = "Missing, invalid or expired access token." });
        operation.Responses.TryAdd("403", new OpenApiResponse { Description = "The authenticated role cannot use this endpoint." });
        return Task.CompletedTask;
    }
}
