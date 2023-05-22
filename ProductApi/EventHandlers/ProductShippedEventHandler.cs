using ProductApi.Data;
using ProductApi.Models;
using SharedModels;
using SharedModels.EventStoreCQRS;
using SharedModels.ProductAPICommon.Events;

namespace ProductApiQ.EventHandlers
{
    public class ProductShippedEventHandler : IEventHandler<ProductShipped>
    {

        public Task HandleAsync(ProductShipped @event, IServiceProvider provider)
        {
            using (var scope = provider.CreateScope())
            {

                var services = scope.ServiceProvider;
                var repository = services.GetService<IRepository<Product>>();

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
}
