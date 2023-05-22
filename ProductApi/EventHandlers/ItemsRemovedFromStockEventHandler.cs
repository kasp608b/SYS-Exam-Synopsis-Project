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

            product.ItemsInStock -= @event.ItemsInStock;

            repository.Edit(product);

            return Task.CompletedTask;
        }
    }
}
