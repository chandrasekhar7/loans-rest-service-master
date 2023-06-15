using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Mvc.ApiExplorer;
using Microsoft.Extensions.Options;
using Swashbuckle.AspNetCore.SwaggerUI;

namespace NetPayAdvance.LoanManagement.Presentation.Swagger
{
    public class ConfigureSwaggerUIOptions : IConfigureNamedOptions<SwaggerUIOptions>
    {
        private readonly IApiVersionDescriptionProvider provider;

        public ConfigureSwaggerUIOptions(IApiVersionDescriptionProvider provider)
        {
            this.provider = provider;
        }
        
        public void Configure(SwaggerUIOptions options)
        {
            foreach (var description in provider.ApiVersionDescriptions)
            {
                options.SwaggerEndpoint($"{description.GroupName}/swagger.json", description.GroupName.ToUpperInvariant());
            }
        }

        public void Configure(string name, SwaggerUIOptions options)
        {
            Configure(options);
        }
    }
}
