using CustomerApi.Data;
using CustomerApi.Models;
using SharedModels;
using SharedModels.CustomerAPICommon.Events;
using SharedModels.EventStoreCQRS;

namespace CustomerApiQ.EventHandlers
{
    public class CustomerCreditStandingChangedEventHandler : IEventHandler<CustomerCreditStandingChanged>
    {
        public Task HandleAsync(CustomerCreditStandingChanged @event, IServiceProvider provider)
        {
            using (var scope = provider.CreateScope())
            {

                var services = scope.ServiceProvider;
                var repository = services.GetService<ICustomerRepository>();

                var customer = repository.Get(@event.Id);

                if (customer == null)
                    throw new InvalidOperationException("Customer not found, cannot change credit standing for customer that does not exist.");

                customer.CreditStanding = @event.CreditStanding;

                repository.Edit(customer);

                return Task.CompletedTask;
            }
        }
    }
}
