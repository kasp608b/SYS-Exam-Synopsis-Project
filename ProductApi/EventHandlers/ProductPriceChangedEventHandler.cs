using ProductApi.Data;
using ProductApi.Models;
using SharedModels.EventStoreCQRS;
using SharedModels.ProductAPICommon.Events;

namespace ProductApiQ.EventHandlers
{
    public class ProductPriceChangedEventHandler : IEventHandler<ProductPriceChanged>
    {

        private readonly IRepository<Product> repository;

        public ProductPriceChangedEventHandler(IRepository<Product> repos)
        {
            repository = repos;
        }

        public Task HandleAsync(ProductPriceChanged @event)
        {
            var product = repository.Get(@event.Id);

            if (product == null)
                throw new InvalidOperationException("Product not found, cannot change price for Product that does not exist.");

            product.Price = @event.Price;

            repository.Edit(product);

            return Task.CompletedTask;
        }
    }
}
