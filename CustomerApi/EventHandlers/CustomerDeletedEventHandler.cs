using CustomerApi.Models;
using SharedModels;
using SharedModels.CustomerAPICommon.Events;
using SharedModels.EventStoreCQRS;

namespace CustomerApiQ.EventHandlers
{
    public class CustomerDeletedEventHandler : IEventHandler<CustomerDeleted>
    {
        public Task HandleAsync(CustomerDeleted @event, IServiceProvider provider)
        {
            using (var scope = provider.CreateScope())
            {

                var services = scope.ServiceProvider;
                var repository = services.GetService<IRepository<Customer>>();

                if (repository.Get(@event.Id) == null)
                    throw new InvalidOperationException("Customer not found, cannot remove Customer that does not exist.");

                repository.Remove(@event.Id);

                return Task.CompletedTask;
            }
        }
    }
}
