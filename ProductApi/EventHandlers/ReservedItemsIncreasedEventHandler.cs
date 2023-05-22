using ProductApi.Data;
using ProductApi.Models;
using SharedModels;
using SharedModels.EventStoreCQRS;
using SharedModels.ProductAPICommon.Events;

namespace ProductApiQ.EventHandlers
{
    public class ReservedItemsIncreasedEventHandler : IEventHandler<ReservedItemsIncreased>
    {

        
        public Task HandleAsync(ReservedItemsIncreased @event, IServiceProvider provider)
        {
            using (var scope = provider.CreateScope())
            {

                var services = scope.ServiceProvider;
                var repository = services.GetService<IRepository<Product>>();
                
                var product = repository.Get(@event.Id);

                if (product == null)
                    throw new InvalidOperationException("Product not found, cannot increase reserved items for Product that does not exist.");

                product.ItemsReserved += @event.ItemsReserved;

                repository.Edit(product);

                return Task.CompletedTask;
            }
        }
    }
}
