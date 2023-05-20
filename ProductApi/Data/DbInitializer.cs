using System.Collections.Generic;
using System.Linq;
using ProductApi.Models;

namespace ProductApi.Data
{
    public class DbInitializer : IDbInitializer
    {
        // This method will create and seed the database.
        public void Initialize(ProductApiContext context)
        {
            context.Database.EnsureDeleted();
            context.Database.EnsureCreated();

            // Look for any Products
            if (context.Products.Any())
            {
                return;   // DB has been seeded
            }

            var productId = Guid.Parse("c4e5ee96-faa4-4fd1-a2ff-801dc2722dc3");

            List<Product> products = new List<Product>
            {
                new Product {ProductId = productId, Name = "Hammer", Category = "Toy" , Price = 100, ItemsInStock = 10, ItemsReserved = 0 },
                new Product { Name = "Screwdriver",Category = "Tool" , Price = 70, ItemsInStock = 20, ItemsReserved = 0 },
                new Product { Name = "Drill",Category = "Tool" , Price = 500, ItemsInStock = 2, ItemsReserved = 0 }
            };

            context.Products.AddRange(products);
            context.SaveChanges();
        }
    }
}
