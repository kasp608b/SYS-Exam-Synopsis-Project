using ProductApi.Data;
using ProductApi.Models;
using SharedModels.EventStoreCQRS;
using SharedModels.ProductAPICommon.Events;

namespace ProductApiQ.EventHandlers
{
    public class ItemsRemovedFromStockEventHandler : IEventHandler<ItemsRemovedFromStock>
    {

        private readonly IRepository<Product> repository;

        public ItemsRemovedFromStockEventHandler(IRepository<Product> repos)
        {
            repository = repos;
        }

        public Task HandleAsync(ItemsRemovedFromStock @event)
        {
            var product = repository.Get(@event.Id);

            if (product == null)
                throw new InvalidOperationException("Product not found, cannot remove items from stock for Product that does not exist.");

            product.ItemsInStock -= @event.ItemsInStock;

            repository.Edit(product);

            return Task.CompletedTask;
        }
    }
}
