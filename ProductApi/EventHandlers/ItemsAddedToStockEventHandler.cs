using Microsoft.Extensions.DependencyInjection;
using ProductApi.Data;
using ProductApi.Models;
using SharedModels;
using SharedModels.EventStoreCQRS;
using SharedModels.ProductAPICommon.Events;

namespace ProductApiQ.EventHandlers
{
    public class ItemsAddedToStockEventHandler : IEventHandler<ItemsAddedToStock>
    {

        public Task HandleAsync(ItemsAddedToStock @event , IServiceProvider provider)
        {
            using (var scope = provider.CreateScope())
            {

                var services = scope.ServiceProvider;
                var repository = services.GetService<IRepository<Product>>();

                var product = repository.Get(@event.Id);

                if (product == null)
                    throw new InvalidOperationException("Product not found, cannot add items to stock for Product that does not exist.");

                product.ItemsInStock += @event.ItemsInStock;

                repository.Edit(product);

                return Task.CompletedTask;
            }
        }

   
    }
}
