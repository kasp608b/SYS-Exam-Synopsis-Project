using Common.EventStoreCQRS;
using CustomerApiC.Aggregates;
using CustomerApiC.Commands;
using EventStore.Client;
using SharedModels.CustomerAPICommon.Events;
using SharedModels.EventStoreCQRS;

namespace CustomerApiC.CommandHandlers
{
    public class ChangeCustomerCreditStandingCommandHandler : ICommandHandler<ChangeCustomerCreditStanding>
    {

        private readonly EventStoreClient _eventStore;

        private readonly EventSerializer _eventSerializer;

        private readonly EventDeserializer _eventDeserializer;

        private readonly CancellationToken _cancellationToken;

        public ChangeCustomerCreditStandingCommandHandler(EventStoreClient eventStore, EventSerializer eventSerializer, EventDeserializer eventDeserializer)
        {
            _eventStore = eventStore;
            _eventSerializer = eventSerializer;
            _eventDeserializer = eventDeserializer;
            _cancellationToken = new CancellationToken();
        }
        
        public async Task HandleAsync(ChangeCustomerCreditStanding command)
        {
            //Check if the customer exists.
            Customer? customer = await _eventStore.Find<Customer, Guid>(command.Id, _eventDeserializer, _cancellationToken);

            if (customer == null)
            {
                throw new InvalidOperationException($"The customer with id:{command.Id} does not exist yet and therfore can not be updated");
            }

            var @event = new CustomerCreditStandingChanged
            {
                Id = command.Id,
                CreditStanding = command.CreditStanding,
                CreditStandingChangedAt = DateTime.UtcNow
            };

            await _eventStore.Append(@event, typeof(Customer).Name, _eventSerializer, _cancellationToken);
        }
    }
}
