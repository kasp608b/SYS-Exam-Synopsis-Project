using ProductApi.Data;
using ProductApi.Models;
using SharedModels.EventStoreCQRS;
using SharedModels.ProductAPICommon.Events;

namespace ProductApiQ.EventHandlers
{
    public class ReservedItemsIncreasedEventHandler : IEventHandler<ReservedItemsIncreased>
    {

        private readonly IRepository<Product> repository;

        public ReservedItemsIncreasedEventHandler(IRepository<Product> repos)
        {
            repository = repos;
        }

        public Task HandleAsync(ReservedItemsIncreased @event)
        {
            var product = repository.Get(@event.Id);

            if (product == null)
                throw new InvalidOperationException("Product not found, cannot increase reserved items for Product that does not exist.");

            product.ItemsReserved += @event.ItemsReserved;

            repository.Edit(product);

            return Task.CompletedTask;
        }
    }
}
