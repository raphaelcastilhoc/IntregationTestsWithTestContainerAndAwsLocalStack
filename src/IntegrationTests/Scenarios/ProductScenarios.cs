using FluentAssertions;
using IntegrationTests.Fixtures;
using IntregationTestsWithTestContainerAndAwsLocalStack.Api.Products;
using IntregationTestsWithTestContainerAndAwsLocalStack.Api.Products.Commands;
using IntregationTestsWithTestContainerAndAwsLocalStack.Api.Products.Dtos;
using IntregationTestsWithTestContainerAndAwsLocalStack.Api.Products.Queries;
using Microsoft.AspNetCore.Http;
using System;
using System.Linq;
using System.Net.Http;
using System.Net.Mime;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Xunit;

namespace IntegrationTests.Scenarios
{
    [Collection("ServerCollection")]
    public class ProductScenarios
    {
        private ServerCollectionFixture _serverCollectionFixture;

        public ProductScenarios(ServerCollectionFixture serverCollectionFixture)
        {
            _serverCollectionFixture = serverCollectionFixture;
        }

        [Fact]
        public async Task Get_ShouldReturnOkWithResult()
        {
            //Arrange
            var httpClient = _serverCollectionFixture.WebApplicationFactory.CreateClient();

            var product = new Product("Name", 10.0);
            await _serverCollectionFixture.DatabaseAccess.AddProductAsync(product);

            var expectedContentResult = new GetProductByIdQueryResult { Id = product.Id,  Name = "Name", Price = 10.0 };
            var expectedStatusCodeResult = StatusCodes.Status200OK;

            //Act
            var result = await httpClient.GetAsync($"api/products/{product.Id}");
            var contentResult = JsonSerializer.Deserialize<GetProductByIdQueryResult>(
                await result.Content.ReadAsStringAsync(),
                new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase });

            //Assert
            contentResult.Should().BeEquivalentTo(expectedContentResult);
            result.StatusCode.Should().BeEquivalentTo(expectedStatusCodeResult);
        }

        [Fact]
        public async Task Post_ShouldReturnCreated()
        {
            //Arrange
            var httpClient = _serverCollectionFixture.WebApplicationFactory.CreateClient();

            var command = new AddProductCommand { Name = "Name", Price = 10.0 };
            var serializedObject = JsonSerializer.Serialize(command);
            var httpContent = new StringContent(serializedObject, Encoding.UTF8, MediaTypeNames.Application.Json);

            var expectedAddedProductResult = new Product("Name", 10.0);
            var expectedQueuedProductResult = new ProductAddedEvent { Name = "Name", Price = 10.0 };

            //Act
            var result = await httpClient.PostAsync("api/products", httpContent);
            var contentResult = JsonSerializer.Deserialize<AddProductCommandResult>(
                await result.Content.ReadAsStringAsync(),
                new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase });

            var addedProductResult = (await _serverCollectionFixture.DatabaseAccess.GetProductsAsync()).First();

            var queueMessages = await _serverCollectionFixture.LocalStackAccess.GetMessages();
            var queuedProductResult = JsonSerializer.Deserialize<ProductAddedEvent>(queueMessages.First());

            var expectedStatusCodeResult = StatusCodes.Status201Created;
            var expectedLocationHeaderResult = new Uri($"http://localhost/api/Products/{addedProductResult.Id}");
            var expectedContentResult = new AddProductCommandResult { Id = addedProductResult.Id };

            //Assert
            addedProductResult.Should().BeEquivalentTo(expectedAddedProductResult, options => options.Excluding(x => x.Id));
            queuedProductResult.Should().BeEquivalentTo(expectedQueuedProductResult);

            result.StatusCode.Should().BeEquivalentTo(expectedStatusCodeResult);
            result.Headers.Location.Should().BeEquivalentTo(expectedLocationHeaderResult);
            contentResult.Should().BeEquivalentTo(expectedContentResult);
        }
    }
}
