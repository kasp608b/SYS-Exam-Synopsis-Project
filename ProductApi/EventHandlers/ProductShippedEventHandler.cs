using ProductApi.Data;
using ProductApi.Models;
using SharedModels.EventStoreCQRS;
using SharedModels.ProductAPICommon.Events;

namespace ProductApiQ.EventHandlers
{
    public class ProductShippedEventHandler : IEventHandler<ProductShipped>
    {

        private readonly IRepository<Product> repository;

        public ProductShippedEventHandler(IRepository<Product> repos)
        {
            repository = repos;
        }

        public Task HandleAsync(ProductShipped @event)
        {
            var product = repository.Get(@event.Id);

            if (product == null)
                throw new InvalidOperationException("Product not found, cannot ship Product that does not exist.");

            product.ItemsReserved -= @event.AmountShipped;
            product.ItemsInStock -= @event.AmountShipped;

            repository.Edit(product);

            return Task.CompletedTask;
        }
    }
}
