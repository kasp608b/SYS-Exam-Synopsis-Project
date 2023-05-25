using EasyNetQ;
using ProductAPIC.Command;
using ProductAPIC.Commands;
using SharedModels;
using SharedModels.EventStoreCQRS;

namespace ProductAPIC.Infrastructure
{
    public class MessageListener
    {
        IServiceProvider provider;
        string connectionString;

        // The service provider is passed as a parameter, because the class needs
        // access to the product repository. With the service provider, we can create
        // a service scope that can provide an instance of the product repository.
        public MessageListener(IServiceProvider provider, string connectionString)
        {
            this.provider = provider;
            this.connectionString = connectionString;
        }

        public async Task StartAsync()
        {
            //Implement the functionality for the method to try to connect again if the connection fails.
            var connectionEstablished = false;

            try
            {
                using (var bus = RabbitHutch.CreateBus(connectionString))
                {

                    while (!connectionEstablished)
                    {


                        Console.WriteLine("Started Listening on " + connectionString + " ");

                        var subscriptionResult = bus.PubSub.SubscribeAsync<OrderStatusChangedMessage>("productApiHkCompleted",
                            HandleOrderCompleted, x => x.WithTopic("completed")).AsTask();

                        // Add code to subscribe to other OrderStatusChanged events:
                        // * cancelled
                        bus.PubSub.Subscribe<OrderStatusChangedMessage>("productApiHkCancelled",
                            HandleOrderCancelled, x => x.WithTopic("cancelled"));
                        // * shipped
                        bus.PubSub.Subscribe<OrderStatusChangedMessage>("productApiHkShipped",
                            HandleOrderShipped, x => x.WithTopic("shipped"));
                        // * paid
                        // Implement an event handler for each of these events.
                        // Be careful that each subscribe has a unique subscription id
                        // (this is the first parameter to the Subscribe method). If they
                        // get the same subscription id, they will listen on the same
                        // queue.

                        await subscriptionResult.WaitAsync(CancellationToken.None);
                        connectionEstablished = subscriptionResult.Status == TaskStatus.RanToCompletion;
                        if (!connectionEstablished) Thread.Sleep(1000);
                    }

                    Console.WriteLine("connectionEstablished= " + connectionEstablished);
                    // Block the thread so that it will not exit and stop subscribing.
                    lock (this)
                    {
                        Monitor.Wait(this);
                    }

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

        }

        private async void HandleOrderShipped(OrderStatusChangedMessage message)
        {
            Console.WriteLine("Handle order shipped called");
            using (var scope = provider.CreateScope())
            {
                var services = scope.ServiceProvider;
                var ShipProductCommandHandler = services.GetService<ICommandHandler<ShipProduct>>();

                //Remove the reservartion items of ordered product (should be a single transaction).
                //Remove the Items form the stock of the ordered product (should be a single transaction).

                foreach (var orderLine in message.OrderLines)
                {
                    await ShipProductCommandHandler.HandleAsync(new ShipProduct { Id = orderLine.ProductId, AmountShipped = orderLine.NoOfItems });
                }
            }
        }

        private async void HandleOrderCancelled(OrderStatusChangedMessage message)
        {
            Console.WriteLine("Handle order cancelled called");
            using (var scope = provider.CreateScope())
            {

                Console.WriteLine("Revieces Order Cancelled Message");
                var services = scope.ServiceProvider;
                var decreaseReservedItemsCommandHandler = services.GetService<ICommandHandler<DecreaseReservedItems>>();

                //Remove the reservartion items of ordered product (should be a single transaction).

                foreach (var orderLine in message.OrderLines)
                {
                    await decreaseReservedItemsCommandHandler.HandleAsync(new DecreaseReservedItems { Id = orderLine.ProductId, ItemsReserved = orderLine.NoOfItems });

                }
            }

        }

        private async void HandleOrderCompleted(OrderStatusChangedMessage message)
        {
            Console.WriteLine("Handle order completed called");
            // A service scope is created to get an instance of the product repository.
            // When the service scope is disposed, the product repository instance will
            // also be disposed.
            using (var scope = provider.CreateScope())
            {
                var services = scope.ServiceProvider;
                var increaseReservedItemsCommandHandler = services.GetService<ICommandHandler<IncreaseReservedItems>>();

                // Reserve items of ordered product (should be a single transaction).
                // Beware that this operation is not idempotent.
                foreach (var orderLine in message.OrderLines)
                {

                    await increaseReservedItemsCommandHandler.HandleAsync(new IncreaseReservedItems { Id = orderLine.ProductId, ItemsReserved = orderLine.NoOfItems });


                }
            }
        }

    }
}
