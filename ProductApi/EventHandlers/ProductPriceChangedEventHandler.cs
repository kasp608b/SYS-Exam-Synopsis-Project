using ProductApi.Data;
using ProductApi.Models;
using SharedModels;
using SharedModels.EventStoreCQRS;
using SharedModels.ProductAPICommon.Events;

namespace ProductApiQ.EventHandlers
{
    public class ProductPriceChangedEventHandler : IEventHandler<ProductPriceChanged>
    {


        public Task HandleAsync(ProductPriceChanged @event, IServiceProvider provider)
        {
            using (var scope = provider.CreateScope())
            {

                var services = scope.ServiceProvider;
                var repository = services.GetService<IRepository<Product>>();

                var product = repository.Get(@event.Id);

                if (product == null)
                    throw new InvalidOperationException("Product not found, cannot change price for Product that does not exist.");

                product.Price = @event.Price;

                repository.Edit(product);

                return Task.CompletedTask;
            }
        }

        
    }
}
