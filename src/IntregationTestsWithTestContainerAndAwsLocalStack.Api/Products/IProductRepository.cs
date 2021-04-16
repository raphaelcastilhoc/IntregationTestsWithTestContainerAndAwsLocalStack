using System.Threading.Tasks;

namespace IntregationTestsWithTestContainerAndAwsLocalStack.Api.Products
{
    public interface IProductRepository
    {
        Task<Product> GetAsync(int id);

        Task AddAsync(Product product);
    }
}
