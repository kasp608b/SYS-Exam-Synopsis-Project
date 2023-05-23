using CustomerApi.Models;
using SharedModels;
using SharedModels.CustomerAPICommon.Events;
using SharedModels.EventStoreCQRS;

namespace CustomerApiQ.EventHandlers
{
    public class CustomerCreatedEventHandler : IEventHandler<CustomerCreated>
    {
        public Task HandleAsync(CustomerCreated @event, IServiceProvider provider)
        {
            using (var scope = provider.CreateScope())
            {

                var services = scope.ServiceProvider;
                var repository = services.GetService<IRepository<Customer>>();

                if (repository.Get(@event.Id) != null)
                    throw new InvalidOperationException("Customer already exists, cannot create Customer that already exists.");

                Customer newCustomer = new Customer
                {
                    CustomerId = @event.Id,
                    CompanyName = @event.CompanyName,
                    RegistrationNumber = @event.RegistrationNumber,
                    Email = @event.Email,
                    Phone = @event.Phone,
                    BillingAddress = @event.BillingAddress,
                    ShippingAddress = @event.ShippingAddress,
                    CreditStanding = @event.CreditStanding,
                };

                repository.Add(newCustomer);

                return Task.CompletedTask;
            }
        }
    }
}
