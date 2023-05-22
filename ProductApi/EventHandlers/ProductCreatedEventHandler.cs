using ProductApi.Data;
using ProductApi.Models;
using SharedModels;
using SharedModels.EventStoreCQRS;
using SharedModels.ProductAPICommon.Events;

namespace ProductApiQ.EventHandlers
{
    public class ProductCreatedEventHandler : IEventHandler<ProductCreated>
    {

        

        public Task HandleAsync(ProductCreated @event, IServiceProvider provider)
        {
            using (var scope = provider.CreateScope())
            {

                var services = scope.ServiceProvider;
                var repository = services.GetService<IRepository<Product>>();
                
                if (repository.Get(@event.Id) != null)
                    throw new InvalidOperationException("Product already exists, cannot create Product that already exists.");

                Product newProduct = new Product
                {
                    ProductId = @event.Id,
                    Name = @event.Name,
                    Price = @event.Price,
                    Category = @event.Category,
                    ItemsInStock = @event.ItemsInStock,
                    ItemsReserved = @event.ItemsReserved
                };

                repository.Add(newProduct);

                return Task.CompletedTask;
            }
        }
    }
}
