using System.Collections.Generic;
using Microsoft.Extensions.Options;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.Swagger;

namespace NetPayAdvance.LoanManagement.Presentation.Swagger
{
    public class ConfigureSwaggerOptions : IConfigureNamedOptions<SwaggerOptions>
    {
        public ConfigureSwaggerOptions() { }

        public void Configure(SwaggerOptions options)
        {
            options.PreSerializeFilters.Add((swagger, httpReq) =>
            {
                if (httpReq.Headers.ContainsKey("X-Forwarded-Host"))
                {
                    /* The httpReq.PathBase and httpReq.Headers["X-Forwarded-Prefix"] is what we need to get the base path.
                    For some reason, they are returning as null/blank. Perhaps this has something to do with how the proxy is configured which I don't have control.
                    For the time being, the base path is manually set here that corresponds to the APIM API Url Prefix.
                    In this case we set it to 'sample-app'. */ 

                    var basePath = "";
                    var serverUrl = $"{httpReq.Scheme}://{httpReq.Headers["X-Forwarded-Host"]}/{basePath}";
                    swagger.Servers = new List<OpenApiServer> {new OpenApiServer {Url = serverUrl}};
                }
            });
        }

        public void Configure(string name, SwaggerOptions options)
        {
            Configure(options);
        }
    }
}

