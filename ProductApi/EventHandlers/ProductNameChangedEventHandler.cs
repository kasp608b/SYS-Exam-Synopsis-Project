using ProductApi.Data;
using ProductApi.Models;
using SharedModels.EventStoreCQRS;
using SharedModels.ProductAPICommon.Events;

namespace ProductApiQ.EventHandlers
{
    public class ProductNameChangedEventHandler : IEventHandler<ProductNameChanged>
    {

        private readonly IRepository<Product> repository;

        public ProductNameChangedEventHandler(IRepository<Product> repos)
        {
            repository = repos;
        }

        public Task HandleAsync(ProductNameChanged @event)
        {
            var product = repository.Get(@event.Id);

            if (product == null)
                throw new InvalidOperationException("Product not found, cannot change name for Product that does not exist.");

            product.Name = @event.Name;

            repository.Edit(product);

            return Task.CompletedTask;
        }
    }
}
