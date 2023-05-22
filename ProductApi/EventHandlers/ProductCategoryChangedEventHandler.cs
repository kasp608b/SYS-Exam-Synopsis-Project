using ProductApi.Data;
using ProductApi.Models;
using SharedModels.EventStoreCQRS;
using SharedModels.ProductAPICommon.Events;

namespace ProductApiQ.EventHandlers
{
    public class ProductCategoryChangedEventHandler : IEventHandler<ProductCategoryChanged>
    {

        private readonly IRepository<Product> repository;

        public ProductCategoryChangedEventHandler(IRepository<Product> repos)
        {
            repository = repos;
        }

        public Task HandleAsync(ProductCategoryChanged @event)
        {
            var product = repository.Get(@event.Id);

            if (product == null)
                throw new InvalidOperationException("Product not found, cannot change category for Product that does not exist.");

            product.Category = @event.Category;

            repository.Edit(product);

            return Task.CompletedTask;
        }
    }
}
