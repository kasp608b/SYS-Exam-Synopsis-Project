using CustomerApi.Models;
using SharedModels;
using SharedModels.CustomerAPICommon.Events;
using SharedModels.EventStoreCQRS;

namespace CustomerApiQ.EventHandlers
{
    public class CustomerInfoChangedEventHandler : IEventHandler<CustomerInfoChanged>
    {
        public Task HandleAsync(CustomerInfoChanged @event, IServiceProvider provider)
        {
            using (var scope = provider.CreateScope())
            {

                var services = scope.ServiceProvider;
                var repository = services.GetService<IRepository<Customer>>();

                var customer = repository.Get(@event.Id);

                if (customer == null)
                    throw new InvalidOperationException("Customer not found, cannot change info for customer that does not exist.");

                customer.CompanyName = @event.CompanyName;
                customer.RegistrationNumber = @event.RegistrationNumber;
                customer.Email = @event.Email;
                customer.Phone = @event.Phone;
                customer.BillingAddress = @event.BillingAddress;
                customer.ShippingAddress = @event.ShippingAddress;

                repository.Edit(customer);

                return Task.CompletedTask;
            }
        }
    }
}
