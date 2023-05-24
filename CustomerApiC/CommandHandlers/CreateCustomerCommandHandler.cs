using Common.EventStoreCQRS;
using CustomerApiC.Aggregates;
using CustomerApiC.Commands;
using EventStore.Client;
using SharedModels.CustomerAPICommon.Events;
using SharedModels.EventStoreCQRS;

namespace CustomerApiC.CommandHandlers
{
    public class CreateCustomerCommandHandler : ICommandHandler<CreateCustomer>
    {

        private readonly EventStoreClient _eventStore;

        private readonly EventSerializer _eventSerializer;

        private readonly EventDeserializer _eventDeserializer;

        private readonly CancellationToken _cancellationToken;

        public CreateCustomerCommandHandler(EventStoreClient eventStore, EventSerializer eventSerializer, EventDeserializer eventDeserializer)
        {
            _eventStore = eventStore;
            _eventSerializer = eventSerializer;
            _eventDeserializer = eventDeserializer;
            _cancellationToken = new CancellationToken();
        }
        public async Task HandleAsync(CreateCustomer command)
        {
            //Check if the customer already exists
            Customer? customer = await _eventStore.Find<Customer, Guid>(command.Id, _eventDeserializer, _cancellationToken);

            if (customer != null)
            {
                throw new InvalidOperationException($"The customer with id:{customer.Id} is allready created");
            }

            var @event = new CustomerCreated
            {
                Id = command.Id,
                CompanyName = command.CompanyName,
                RegistrationNumber = command.RegistrationNumber,
                Email = command.Email,
                Phone = command.Phone,
                BillingAddress = command.BillingAddress,
                ShippingAddress = command.ShippingAddress,
                CreditStanding = command.CreditStanding,
                CreatedAt = DateTime.UtcNow
            };

            await _eventStore.Append(@event, typeof(Customer).Name, _eventSerializer, _cancellationToken);
        }
    }
}
