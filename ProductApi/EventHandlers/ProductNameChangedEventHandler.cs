﻿using ProductApi.Data;
using ProductApi.Models;
using SharedModels;
using SharedModels.EventStoreCQRS;
using SharedModels.ProductAPICommon.Events;

namespace ProductApiQ.EventHandlers
{
    public class ProductNameChangedEventHandler : IEventHandler<ProductNameChanged>
    {

     
        public Task HandleAsync(ProductNameChanged @event, IServiceProvider provider)
        {
            using (var scope = provider.CreateScope())
            {

                var services = scope.ServiceProvider;
                var repository = services.GetService<IRepository<Product>>();
                
                var product = repository.Get(@event.Id);

                if (product == null)
                    throw new InvalidOperationException("Product not found, cannot change name for Product that does not exist.");

                product.Name = @event.Name;

                repository.Edit(product);

                return Task.CompletedTask;
            }
        }
    }
}
