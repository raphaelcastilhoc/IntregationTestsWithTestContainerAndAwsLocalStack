using Dapper;
using System.Data;
using System.Linq;
using System.Threading.Tasks;

namespace IntregationTestsWithTestContainerAndAwsLocalStack.Api.Products
{
    public class ProductRepository : IProductRepository
    {
        private IDbConnection _connection;

        public ProductRepository(IDbConnection connection)
        {
            _connection = connection;
        }

        public async Task<Product> GetAsync(int id)
        {
            var query = @"SELECT Id, Name, Price FROM Product where Id = @Id";
            var product = await _connection.QueryFirstOrDefaultAsync<Product>(query, new { Id = id });

            return product;
        }

        public async Task AddAsync(Product product)
        {
            var command = @"INSERT INTO [Product] VALUES (@Name, @Price); SELECT CAST(SCOPE_IDENTITY() as int);";

            var result = await _connection.QueryAsync<int>(command, product);
            product.SetId(result.First());
        }
    }
}
