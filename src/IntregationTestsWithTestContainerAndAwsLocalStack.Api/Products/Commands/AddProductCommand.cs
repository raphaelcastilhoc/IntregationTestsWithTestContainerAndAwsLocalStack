using MediatR;

namespace IntregationTestsWithTestContainerAndAwsLocalStack.Api.Products.Commands
{
    public class AddProductCommand : IRequest<int>
    {
        public string Name { get; set; }

        public double Price { get; set; }
    }
}
