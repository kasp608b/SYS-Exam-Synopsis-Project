using ProductApi.Data;
using ProductApi.Models;
using SharedModels.EventStoreCQRS;
using SharedModels.ProductAPICommon.Events;

namespace ProductApiQ.EventHandlers
{
    public class ReservedItemsDecreasedEventHandler : IEventHandler<ReservedItemsDecreased>
    {

        private readonly IRepository<Product> repository;

        public ReservedItemsDecreasedEventHandler(IRepository<Product> repos)
        {
            repository = repos;
        }

        public Task HandleAsync(ReservedItemsDecreased @event)
        {
            var product = repository.Get(@event.Id);

            if (product == null)
                throw new InvalidOperationException("Product not found, cannot decrease reserved items for Product that does not exist.");

            product.ItemsReserved -= @event.ItemsReserved;

            repository.Edit(product);

            return Task.CompletedTask;
        }
    }
}
