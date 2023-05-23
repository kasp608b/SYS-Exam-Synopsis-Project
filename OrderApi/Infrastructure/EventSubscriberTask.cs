using Common.EventStoreCQRS;
using EventStore.Client;
using Newtonsoft.Json;
using SharedModels;
using SharedModels.EventStoreCQRS;
using SharedModels.OrderAPICommon.Events;

namespace OrderApiQ.Infrastructure
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

                var ordertRepos = services.GetService<IRepository<Order>>();

                var eventStoreClient = services.GetService<EventStoreClient>();

                var deserializer = services.GetService<EventDeserializer>();

                var orderCreatedEventHandler = services.GetService<IEventHandler<OrderCreated>>();
                var orderCompletedEventHandler = services.GetService<IEventHandler<OrderCompleted>>();
                var orderPayedforEventHandler = services.GetService<IEventHandler<OrderPayedfor>>();
                var orderShippedEventHandler = services.GetService<IEventHandler<OrderShipped>>();
                var orderCanceledEventHandler = services.GetService<IEventHandler<OrderCanceled>>();
                var orderDeletedEventHandler = services.GetService<IEventHandler<OrderDeleted>>();

                await eventStoreClient.SubscribeToAllAsync(
                FromAll.Start,
                async (subscription, @event, cancellationToken) =>
                {
                    Console.WriteLine($"Got event {@event.Event.EventType}");
                    if (@event.Event.EventType.StartsWith("$") || @event.Event.EventType.StartsWith("_"))
                    {
                        return; // skip system events
                    }

                    if (@event.OriginalStreamId.StartsWith("Order-"))
                    {
                        try
                        {
                            var eventData = deserializer.Deserialize(@event.Event.Data.ToArray());

                            switch (eventData)
                            {
                                case OrderCreated orderCreated:
                                    await orderCreatedEventHandler.HandleAsync((OrderCreated)eventData, provider);
                                    break;

                                case OrderCompleted orderCompleted:
                                    await orderCompletedEventHandler.HandleAsync((OrderCompleted)eventData, provider);
                                    break;

                                case OrderPayedfor orderPayedfor:
                                    await orderPayedforEventHandler.HandleAsync((OrderPayedfor)eventData, provider);
                                    break;

                                case OrderShipped orderShipped:
                                    await orderShippedEventHandler.HandleAsync((OrderShipped)eventData, provider);
                                    break;

                                case OrderCanceled orderCanceled:
                                    await orderCanceledEventHandler.HandleAsync((OrderCanceled)eventData, provider);
                                    break;

                                case OrderDeleted orderDeleted:
                                    await orderDeletedEventHandler.HandleAsync((OrderDeleted)eventData, provider);
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
