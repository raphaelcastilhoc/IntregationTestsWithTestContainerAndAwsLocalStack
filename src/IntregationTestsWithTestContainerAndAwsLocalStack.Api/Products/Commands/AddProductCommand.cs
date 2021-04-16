using MediatR;

namespace IntregationTestsWithTestContainerAndAwsLocalStack.Api.Products.Commands
{
    public class AddProductCommand : IRequest<AddProductCommandResult>
    {
        public string Name { get; set; }

        public double Price { get; set; }
    }

    public class AddProductCommandResult
    {
        public int Id { get; set; }
    }
}
