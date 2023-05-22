using ProductApi.Data;
using ProductApi.Models;
using SharedModels;
using SharedModels.EventStoreCQRS;
using SharedModels.ProductAPICommon.Events;

namespace ProductApiQ.EventHandlers
{
    public class ProductDeletedEventHandler : IEventHandler<ProductDeleted>
    {

       

        public Task HandleAsync(ProductDeleted @event, IServiceProvider provider)
        {
            using (var scope = provider.CreateScope())
            {

                var services = scope.ServiceProvider;
                var repository = services.GetService<IRepository<Product>>();
                
                if (repository.Get(@event.Id) == null)
                    throw new InvalidOperationException("Product not found, cannot remove Product that does not exist.");

                repository.Remove(@event.Id);

                return Task.CompletedTask;
            }
        }
    }
}
