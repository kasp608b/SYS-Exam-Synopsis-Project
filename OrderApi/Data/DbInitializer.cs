using System.Collections.Generic;
using System.Linq;
using SharedModels;
using System;

namespace OrderApi.Data
{
    public class DbInitializer : IDbInitializer
    {
        // This method will create and seed the database.
        public void Initialize(OrderApiContext context)
        {
            context.Database.EnsureDeleted();
            context.Database.EnsureCreated();

            // Look for any Products
            if (context.Orders.Any())
            {
                return;   // DB has been seeded
            }
            var orderId = Guid.Parse("0d5909b4-e6ee-432f-8b2c-5823ef75d0a1");

            var productId = Guid.Parse("1fa60396-6644-4f09-b5d1-2ebcdeca49b1");

            var customerId = Guid.Parse("2749cb35-1664-4c53-aa45-aff486bedf39");

            List<Order> orders = new List<Order>
            {
                new Order { OrderId = orderId, Date = DateTime.Today, Status = OrderStatus.proccesing, CustomerId = customerId}
            };

            List<OrderLine> orderLines = new List<OrderLine>
            {
                 new OrderLine {OrderId = orderId , NoOfItems = 5, ProductId = productId},
            };

            context.Orders.AddRange(orders);
            context.Orderlines.AddRange(orderLines);
            context.SaveChanges();
        }
    }
}
