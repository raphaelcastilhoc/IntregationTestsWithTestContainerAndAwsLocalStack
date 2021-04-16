using System.Threading.Tasks;

namespace IntregationTestsWithTestContainerAndAwsLocalStack.Api.Products
{
    public interface IProductRepository
    {
        Task AddAsync(Product product);
    }
}
