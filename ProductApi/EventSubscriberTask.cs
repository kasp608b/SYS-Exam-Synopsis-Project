using Common.EventStoreCQRS;
using EventStore.Client;

namespace ProductApiQ
{
    public class EventSubscriberTask
    {
        EventStoreClient eventStoreClient;
        EventDeserializer deserializer;

        public EventSubscriberTask(EventStoreClient eventStoreClient, EventDeserializer deserializer)
        {
            this.eventStoreClient = eventStoreClient;
            this.deserializer = deserializer;
        }

        public async Task StartAsync()
        {
            decimal total = 0;
            await eventStoreClient.SubscribeToAllAsync(
                FromAll.Start,
                async (subscription, @event, cancellationToken) =>
                {
                    Console.WriteLine($"Got event {@event.Event.EventType}");
                    if (@event.Event.EventType.StartsWith("$") || @event.Event.EventType.StartsWith("_"))
                    {
                        return; // skip system events
                    }

                    if (@event.OriginalStreamId.StartsWith("Account-"))
                    {
                        try
                        {
                            var eventData = deserializer.Deserialize(@event.Event.Data.ToArray());

                            switch (eventData)
                            {
                                case AddToTotal addToTotal:
                                    Console.WriteLine($"AddToTotal: {addToTotal.Amount}");
                                    total += addToTotal.Amount;
                                    Console.WriteLine($"Current total:{total} ");
                                    break;
                            }
                        }
                        catch (JsonSerializationException ex)
                        {
                            Console.WriteLine(ex.Message);
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine(ex.Message);

                        }

                    }
                }, subscriptionDropped: (subscription, reason, exception) =>
                {
                    Console.WriteLine($"Subscription dropped due to {reason}");
                });

            // Block the thread so that it will not exit and stop subscribing.
            lock (this)
            {
                Monitor.Wait(this);
            }
        }
    }
}
