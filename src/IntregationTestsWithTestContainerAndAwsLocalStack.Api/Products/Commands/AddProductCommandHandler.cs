using Amazon.SQS;
using IntregationTestsWithTestContainerAndAwsLocalStack.Api.Products.Dtos;
using MediatR;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

namespace IntregationTestsWithTestContainerAndAwsLocalStack.Api.Products.Commands
{
    public class AddProductCommandHandler : IRequestHandler<AddProductCommand, AddProductCommandResult>
    {
        private readonly IProductRepository _productRepository;
        private readonly IAmazonSQS _sqsClient;

        public AddProductCommandHandler(IProductRepository productRepository,
            IAmazonSQS sqsClient)
        {
            _productRepository = productRepository;
            _sqsClient = sqsClient;
        }

        public async Task<AddProductCommandResult> Handle(AddProductCommand request, CancellationToken cancellationToken)
        {
            var product = new Product(request.Name, request.Price);
            await _productRepository.AddAsync(product);

            var productAddedEvent = new ProductAddedEvent { Name = product.Name, Price = product.Price };
            var sqsMessage = JsonSerializer.Serialize(productAddedEvent);
            await _sqsClient.SendMessageAsync("http://localhost:4566/000000000000/ProductQueue", sqsMessage);

            var result = new AddProductCommandResult { Id = product.Id };
            return result;
        }
    }
}
