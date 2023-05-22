using Common.EventStoreCQRS;
using EventStore.Client;
using Newtonsoft.Json;
using ProductApi.Models;
using ProductApiQ.EventHandlers;
using SharedModels;
using SharedModels.EventStoreCQRS;
using SharedModels.ProductAPICommon.Events;

namespace ProductApiQ.Infrastructure
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

                var productRepos = services.GetService<IRepository<Product>>();
                
                var eventStoreClient = services.GetService<EventStoreClient>();

                var deserializer = services.GetService<EventDeserializer>();

                var itemsAddedToStockEventHandler = services.GetService<IEventHandler<ItemsAddedToStock>>();
                var itemsRemovedFromStockEventHandler = services.GetService<IEventHandler<ItemsRemovedFromStock>>();
                var productCategoryChangedEventHandler = services.GetService<IEventHandler<ProductCategoryChanged>>();
                var productCreatedEventHandler = services.GetService<IEventHandler<ProductCreated>>();
                var productDeletedEventHandler = services.GetService<IEventHandler<ProductDeleted>>();
                var productNameChangedEventHandler = services.GetService<IEventHandler<ProductNameChanged>>();
                var productPriceChangedEventHandler = services.GetService<IEventHandler<ProductPriceChanged>>();
                var productShippedEventHandler = services.GetService<IEventHandler<ProductShipped>>();
                var reservedItemsDecreasedEventHandler = services.GetService<IEventHandler<ReservedItemsDecreased>>();
                var reservedItemsIncreasedEventHandler = services.GetService<IEventHandler<ReservedItemsIncreased>>();

                await eventStoreClient.SubscribeToAllAsync(
                FromAll.Start,
                async (subscription, @event, cancellationToken) =>
                {
                    Console.WriteLine($"Got event {@event.Event.EventType}");
                    if (@event.Event.EventType.StartsWith("$") || @event.Event.EventType.StartsWith("_"))
                    {
                        return; // skip system events
                    }

                    if (@event.OriginalStreamId.StartsWith("Product-"))
                    {
                        try
                        {
                            var eventData = deserializer.Deserialize(@event.Event.Data.ToArray());

                            switch (eventData)
                            {
                                case ItemsAddedToStock itemsAddedToStock:
                                    await itemsAddedToStockEventHandler.HandleAsync((ItemsAddedToStock)eventData, provider);
                                    break;

                                case ItemsRemovedFromStock itemsRemovedFromStock:
                                    await itemsRemovedFromStockEventHandler.HandleAsync((ItemsRemovedFromStock)eventData, provider);
                                    break;

                                case ProductCategoryChanged productCategoryChanged:
                                    await productCategoryChangedEventHandler.HandleAsync((ProductCategoryChanged)eventData, provider);
                                    break;

                                case ProductCreated productCreated:
                                    await productCreatedEventHandler.HandleAsync((ProductCreated)eventData, provider);
                                    break;

                                case ProductDeleted productDeleted:
                                    await productDeletedEventHandler.HandleAsync((ProductDeleted)eventData, provider);
                                    break;

                                case ProductNameChanged productNameChanged:
                                    await productNameChangedEventHandler.HandleAsync((ProductNameChanged)eventData, provider);
                                    break;

                                case ProductPriceChanged productPriceChanged:
                                    await productPriceChangedEventHandler.HandleAsync((ProductPriceChanged)eventData, provider);
                                    break;

                                case ProductShipped productShipped:
                                    await productShippedEventHandler.HandleAsync((ProductShipped)eventData, provider);
                                    break;

                                case ReservedItemsDecreased reservedItemsDecreased:
                                    await reservedItemsDecreasedEventHandler.HandleAsync((ReservedItemsDecreased)eventData, provider);
                                    break;

                                case ReservedItemsIncreased reservedItemsIncreased:
                                    await reservedItemsIncreasedEventHandler.HandleAsync((ReservedItemsIncreased)eventData, provider);
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
