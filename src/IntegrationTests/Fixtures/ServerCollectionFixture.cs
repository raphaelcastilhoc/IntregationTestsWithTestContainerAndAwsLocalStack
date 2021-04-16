using Amazon.SQS;
using DotNet.Testcontainers.Containers.Builders;
using DotNet.Testcontainers.Containers.Configurations.Databases;
using DotNet.Testcontainers.Containers.Modules;
using DotNet.Testcontainers.Containers.Modules.Databases;
using DotNet.Testcontainers.Containers.WaitStrategies;
using IntegrationTests.Infrastructure;
using IntregationTestsWithTestContainerAndAwsLocalStack.Api;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace IntegrationTests.Fixtures
{
    public class ServerCollectionFixture : IDisposable
    {
        public WebApplicationFactory<Startup> WebApplicationFactory { get; private set; }

        public LocalStackAccess LocalStackAccess { get; private set; }

        public DatabaseAccess DatabaseAccess { get; private set; }

        private MsSqlTestcontainer MsSqlTestcontainer { get; set; }

        private TestcontainersContainer LocalStackTestcontainer { get; set; }

        public ServerCollectionFixture()
        {
            var databaseTask = BuildDatabase();
            var localStackTask = BuildLocalStack();

            Task.WhenAll(databaseTask, localStackTask).GetAwaiter().GetResult();

            WebApplicationFactory = new WebApplicationFactory<Startup>().WithWebHostBuilder(builder =>
            {
                builder
                .ConfigureAppConfiguration((context, configBuilder) =>
                {
                    configBuilder.AddInMemoryCollection(
                        new Dictionary<string, string>
                        {
                            ["ConnectionString"] = MsSqlTestcontainer.ConnectionString
                        });
                })
                .ConfigureTestServices(services =>
                {
                    services.AddSingleton<IAmazonSQS>(x => { return LocalStackAccess.SQSClient; });
                });
            });
        }

        private async Task BuildLocalStack()
        {
            var localStackTestContainersBuilder = new TestcontainersBuilder<TestcontainersContainer>()
                            .WithImage("localstack/localstack")
                            .WithName("localstack")
                            .WithPortBinding(4566)
                            .WithWaitStrategy(Wait.ForUnixContainer().UntilPortIsAvailable(4566));

            LocalStackTestcontainer = localStackTestContainersBuilder.Build();
            await LocalStackTestcontainer.StartAsync();

            LocalStackAccess = new LocalStackAccess();
            LocalStackAccess.CreateQueue("ProductQueue");
        }

        private async Task BuildDatabase()
        {
            var msSqlTestContainersBuilder = new TestcontainersBuilder<MsSqlTestcontainer>()
                .WithDatabase(new MsSqlTestcontainerConfiguration
                {
                    Password = "yourStrong(!)Password"
                });

            MsSqlTestcontainer = msSqlTestContainersBuilder.Build();
            await MsSqlTestcontainer.StartAsync();

            DatabaseAccess = new DatabaseAccess(MsSqlTestcontainer.ConnectionString);
        }

        public void Dispose()
        {
            WebApplicationFactory.Dispose();
            MsSqlTestcontainer.DisposeAsync().GetAwaiter().GetResult();
            LocalStackTestcontainer.DisposeAsync().GetAwaiter().GetResult();
        }
    }
}
