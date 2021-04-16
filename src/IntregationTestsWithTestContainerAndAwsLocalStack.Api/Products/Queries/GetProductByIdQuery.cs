using MediatR;

namespace IntregationTestsWithTestContainerAndAwsLocalStack.Api.Products.Queries
{
    public class GetProductByIdQuery : IRequest<GetProductByIdQueryResult>
    {
        public int Id { get; set; }
    }

    public class GetProductByIdQueryResult
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public double Price { get; set; }
    }
}
