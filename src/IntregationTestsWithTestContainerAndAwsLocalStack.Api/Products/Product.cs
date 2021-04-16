namespace IntregationTestsWithTestContainerAndAwsLocalStack.Api.Products
{
    public class Product
    {
        public Product(string name, double price)
        {
            Name = name;
            Price = price;
        }

        public int Id { get; private set; }

        public string Name { get; private set; }

        public double Price { get; private set; }

        public void SetId(int id)
        {
            Id = id;
        }
    }
}
