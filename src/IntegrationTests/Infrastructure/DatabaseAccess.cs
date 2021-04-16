using Dapper;
using IntregationTestsWithTestContainerAndAwsLocalStack.Api.Products;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;

namespace IntegrationTests.Infrastructure
{
    public class DatabaseAccess
    {
        private readonly string _connectionString;

        public DatabaseAccess(string connectionString)
        {
            _connectionString = connectionString;
            CreateDatabase();
        }

        private void CreateDatabase()
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                var command = "CREATE TABLE Product (Id int IDENTITY(1,1), Name varchar(50), Price float);";
                connection.Execute(command);
            }
        }

        public async Task<IEnumerable<Product>> GetProductsAsync()
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                var query = @"SELECT Id, Name, Price FROM Product";
                var products = await connection.QueryAsync<Product>(query);
                return products;
            }
        }

        public async Task AddProductAsync(Product product)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                var command = @"INSERT INTO [Product] VALUES (@Name, @Price); SELECT CAST(SCOPE_IDENTITY() as int);";

                var result = await connection.QueryAsync<int>(command, product);
                product.SetId(result.First());
            }
        }
    }
}
