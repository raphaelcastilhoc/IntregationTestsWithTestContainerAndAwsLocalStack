using Microsoft.AspNetCore.Builder;

namespace IntregationTestsWithTestContainerAndAwsLocalStack.Api.Extensions
{
    public static class ApplicationBuilderExtensions
    {
        public static void UseCustomSwagger(this IApplicationBuilder app, string apiName)
        {
            app.UseSwagger();
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", apiName);
                c.RoutePrefix = string.Empty;
            });
        }
    }
}
