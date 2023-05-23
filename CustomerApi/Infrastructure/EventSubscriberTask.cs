using Common.EventStoreCQRS;
using CustomerApi.Models;
using EventStore.Client;
using Newtonsoft.Json;
using SharedModels;
using SharedModels.CustomerAPICommon.Events;
using SharedModels.EventStoreCQRS;

namespace CustomerApiQ.Infrastructure
{
    public class EventSubscriberTask
    {
        IServiceProvider provider;

        public EventSubscriberTask(IServiceProvider provider)
        {
            this.provider = provider;
        }

        public async Task StartAsync()
        {

            using (var scope = provider.CreateScope())
            {
                var services = scope.ServiceProvider;

                var productRepos = services.GetService<IRepository<Customer>>();

                var eventStoreClient = services.GetService<EventStoreClient>();

                var deserializer = services.GetService<EventDeserializer>();

                var customerCreatedEventHandler = services.GetService<IEventHandler<CustomerCreated>>();
                var customerCreditStandingChangedEventHandler = services.GetService<IEventHandler<CustomerCreditStandingChanged>>();
                var customerDeletedEventHandler = services.GetService<IEventHandler<CustomerDeleted>>();
                var customerInfoChangedEventHandler = services.GetService<IEventHandler<CustomerInfoChanged>>();

                await eventStoreClient.SubscribeToAllAsync(
                FromAll.Start,
                async (subscription, @event, cancellationToken) =>
                {
                    Console.WriteLine($"Got event {@event.Event.EventType}");
                    if (@event.Event.EventType.StartsWith("$") || @event.Event.EventType.StartsWith("_"))
                    {
                        return; // skip system events
                    }

                    if (@event.OriginalStreamId.StartsWith("Customer-"))
                    {
                        try
                        {
                            var eventData = deserializer.Deserialize(@event.Event.Data.ToArray());

                            switch (eventData)
                            {
                                case CustomerCreated customerCreated:
                                    await customerCreatedEventHandler.HandleAsync((CustomerCreated)eventData, provider);
                                    break;

                                case CustomerCreditStandingChanged customerCreditStandingChanged:
                                    await customerCreditStandingChangedEventHandler.HandleAsync((CustomerCreditStandingChanged)eventData, provider);
                                    break;

                                case CustomerDeleted customerDeleted:
                                    await customerDeletedEventHandler.HandleAsync((CustomerDeleted)eventData, provider);
                                    break;

                                case CustomerInfoChanged customerInfoChanged:
                                    await customerInfoChangedEventHandler.HandleAsync((CustomerInfoChanged)eventData, provider);
                                    break;

                                default:
                                    throw new InvalidOperationException($"Unknown event type: {eventData.GetType().Name}");


                            }
                        }
                        catch (AggregateException ae)
                        {
                            ae.Flatten().Handle(e =>
                            {
                                if (e is InvalidOperationException)
                                {
                                    Console.WriteLine($" An invalid operation exception was thrown with message: \n {e.Message}");
                                    return true;
                                }
                                else if (e is Exception)
                                {
                                    Console.WriteLine($" An exception was thrown with message: \n {e.Message}");
                                    return true;
                                }
                                else
                                {
                                    return false;
                                }
                            });

                        }
                        catch (InvalidOperationException ex)
                        {
                            Console.WriteLine($" An invalid operation exception was thrown with message: \n {ex.Message}");
                        }
                        catch (JsonSerializationException ex)
                        {
                            Console.WriteLine($" A Json serialization exception was thrown with message: \n {ex.Message}");
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine($" An exception was thrown with message: \n {ex.Message}");

                        }

                    }
                }, subscriptionDropped: (subscription, reason, exception) =>
                {
                    Console.WriteLine($"Subscription dropped due to {reason}");
                });

            }

            // Block the thread so that it will not exit and stop subscribing.
            lock (this)
            {
                Monitor.Wait(this);
            }
        }
    }
}
