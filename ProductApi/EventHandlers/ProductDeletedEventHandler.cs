using ProductApi.Data;
using ProductApi.Models;
using SharedModels.EventStoreCQRS;
using SharedModels.ProductAPICommon.Events;

namespace ProductApiQ.EventHandlers
{
    public class ProductDeletedEventHandler : IEventHandler<ProductDeleted>
    {

        private readonly IRepository<Product> repository;

        public ProductDeletedEventHandler(IRepository<Product> repos)
        {
            repository = repos;
        }

        public Task HandleAsync(ProductDeleted @event)
        {
            if (repository.Get(@event.Id) == null)
                throw new InvalidOperationException("Product not found, cannot remove Product that does not exist.");

            repository.Remove(@event.Id);

            return Task.CompletedTask;
        }
    }
}
