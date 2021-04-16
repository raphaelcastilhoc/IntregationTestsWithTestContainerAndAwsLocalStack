using Amazon.SQS;
using IntregationTestsWithTestContainerAndAwsLocalStack.Api.Products;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System.Data;
using System.Data.SqlClient;

namespace IntregationTestsWithTestContainerAndAwsLocalStack.Api.Extensions
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddScoped<IProductRepository, ProductRepository>();

            var connectionString = configuration["ConnectionString"];
            services.AddScoped<IDbConnection>(x => new SqlConnection(connectionString));

            services.AddSingleton<IAmazonSQS, AmazonSQSClient>();

            return services;
        }
    }
}
