using ProductApi.Data;
using ProductApi.Models;
using SharedModels.EventStoreCQRS;
using SharedModels.ProductAPICommon.Events;

namespace ProductApiQ.EventHandlers
{
    public class ItemsAddedToStockEventHandler : IEventHandler<ItemsAddedToStock>
    {

        private readonly IRepository<Product> repository;

        public ItemsAddedToStockEventHandler(IRepository<Product> repos)
        {
            repository = repos;
        }

        public Task HandleAsync(ItemsAddedToStock @event)
        {

            var product = repository.Get(@event.Id);

            if (product == null)
                throw new InvalidOperationException("Product not found, cannot add items to stock for Product that does not exist.");

            product.ItemsInStock += @event.ItemsInStock;

            repository.Edit(product);

            return Task.CompletedTask;
        }
    }
}
