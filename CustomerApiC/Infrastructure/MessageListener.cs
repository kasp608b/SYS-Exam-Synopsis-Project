using Common.EventStoreCQRS;
using CustomerApiC.Aggregates;
using CustomerApiC.Commands;
using EasyNetQ;
using EventStore.Client;
using SharedModels;
using SharedModels.EventStoreCQRS;

namespace CustomerApi.Infrastructure
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

        public async Task Start()
        {
            var connectionEstablished = false;
            using (var bus = RabbitHutch.CreateBus(connectionString))
            {
                try
                {
                    while (!connectionEstablished)
                    {

                        Console.WriteLine("Started Listening on " + connectionString + " ");

                        // * shipped
                        var subscriptionResult = bus.PubSub.SubscribeAsync<OrderStatusChangedMessage>("customerApiHkShipped",
                            HandleOrderShipped, x => x.WithTopic("shipped")).AsTask();

                        // * credit standing changed
                        bus.PubSub.Subscribe<CreditStandingChangedMessage>("customerApiHkCreditStandingChanged",
                            HandleCreditStandingChanged);


                        // * cancelled
                        bus.PubSub.Subscribe<OrderStatusChangedMessage>("customerApiHkCancelled",
                            HandleOrderCancelled, x => x.WithTopic("cancelled"));

                        // * order rejected
                        bus.PubSub.Subscribe<OrderRejectedMessage>("customerApiHkOrderRejected",
                            HandleOrderRejected);

                        // * order compleated
                        bus.PubSub.Subscribe<OrderStatusChangedMessage>("customerApiHkCompleted",
                           HandleOrderCompleted, x => x.WithTopic("completed"));

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

        }

        private async void HandleOrderCancelled(OrderStatusChangedMessage message)
        {
            Console.WriteLine("Handle order Cancelled called");
            using (var scope = provider.CreateScope())
            {
                var services = scope.ServiceProvider;
                var emailService = services.GetService<IEmailService>();
                var eventStore = services.GetService<EventStoreClient>();
                var eventDeserializer = services.GetService<EventDeserializer>();
                var cancellationToken = new CancellationToken();

                if (message.CustomerId != null && eventStore != null && emailService != null)
                {
                    var customer = await eventStore.Find<Customer, Guid>((Guid)message.CustomerId, eventDeserializer, cancellationToken);

                    if (customer == null)
                        return;

                    var sb = new System.Text.StringBuilder();
                    sb.AppendLine($"Dear {customer.CompanyName}");
                    sb.AppendLine($"Order with order id number: {message.OrderLines[0].OrderId} has been cancelled.");
                    sb.AppendLine($"Have a nice day");

                    emailService.SendEmail(customer.Email, "Order cancelled", sb.ToString());

                }

            }
        }

        private async void HandleOrderCompleted(OrderStatusChangedMessage message)
        {
            Console.WriteLine("Handle order completed called");
            using (var scope = provider.CreateScope())
            {
                var services = scope.ServiceProvider;
                var emailService = services.GetService<IEmailService>();
                var eventStore = services.GetService<EventStoreClient>();
                var eventDeserializer = services.GetService<EventDeserializer>();
                var cancellationToken = new CancellationToken();
                var productServiceGateway = services.GetService<IServiceGateway<ProductDto>>();


                if (message.CustomerId != null && eventStore != null && emailService != null && productServiceGateway != null)
                {
                    var customer = await eventStore.Find<Customer, Guid>((Guid)message.CustomerId, eventDeserializer, cancellationToken);
                    if (customer == null)
                        return;

                    decimal totalPrice = 0;

                    var sb = new System.Text.StringBuilder();
                    sb.AppendLine($"Dear {customer.CompanyName}");
                    sb.AppendLine($"Order with order id number: {message.OrderLines[0].OrderId} has been completed.");
                    sb.AppendLine($"The following is a list of the products and the number of ordered products");
                    foreach (OrderLineDto orderline in message.OrderLines)
                    {
                        var product = productServiceGateway.Get(orderline.ProductId);
                        sb.AppendLine($"Product name: {product.Name}, Price: {product.Price}, Number of ordered items: {orderline.NoOfItems}");

                        totalPrice += product.Price * orderline.NoOfItems;
                    }

                    sb.AppendLine($"Total price: {totalPrice}kr.");
                    sb.AppendLine($"The ordered products will be shipped shortly");
                    sb.AppendLine($"Have a nice day");

                    emailService.SendEmail(customer.Email, "Order completed", sb.ToString());

                }

            }
        }

        private async void HandleOrderRejected(OrderRejectedMessage message)
        {
            Console.WriteLine("Handle order rejected called");
            using (var scope = provider.CreateScope())
            {
                var services = scope.ServiceProvider;
                var emailService = services.GetService<IEmailService>();
                var eventStore = services.GetService<EventStoreClient>();
                var eventDeserializer = services.GetService<EventDeserializer>();
                var cancellationToken = new CancellationToken();
                var productServiceGateway = services.GetService<IServiceGateway<ProductDto>>();


                if (eventStore != null && emailService != null && productServiceGateway != null)
                {

                    var customer = await eventStore.Find<Customer, Guid>((Guid)message.orderDto.CustomerId, eventDeserializer, cancellationToken);
                    if (customer == null)
                        return;

                    decimal totalPrice = 0;


                    var sb = new System.Text.StringBuilder();
                    sb.AppendLine($"Dear {customer.CompanyName}");
                    sb.AppendLine($"Order with order id number: {message.orderDto.OrderId} has been Rejected.");
                    sb.AppendLine($"The following is a list of the products and the number of ordered products");
                    foreach (OrderLineDto orderline in message.orderDto.Orderlines)
                    {
                        var product = productServiceGateway.Get(orderline.ProductId);
                        sb.AppendLine($"Product name: {product.Name}, Price: {product.Price}, Number of ordered items: {orderline.NoOfItems}");

                        totalPrice += product.Price * orderline.NoOfItems;
                    }

                    sb.AppendLine($"Total price: {totalPrice}kr.");
                    sb.AppendLine($"The order was rejected because: {message.message}");

                    emailService.SendEmail(customer.Email, "Order rejected", sb.ToString());

                }

            }
        }

        private async void HandleCreditStandingChanged(CreditStandingChangedMessage message)
        {
            Console.WriteLine("Handle credit status changed called");
            using (var scope = provider.CreateScope())
            {

                var services = scope.ServiceProvider;
                var eventStore = services.GetService<EventStoreClient>();
                var eventDeserializer = services.GetService<EventDeserializer>();
                var cancellationToken = new CancellationToken();
                var ChangeCustomerCreditStandingCommandHandler = services.GetService<ICommandHandler<ChangeCustomerCreditStanding>>();


                if (ChangeCustomerCreditStandingCommandHandler != null)
                {

                    await ChangeCustomerCreditStandingCommandHandler.HandleAsync(new ChangeCustomerCreditStanding { Id = message.CustomerId, CreditStanding = message.NewCreditStanding });

                }

            }
        }

        private async void HandleOrderShipped(OrderStatusChangedMessage message)
        {
            Console.WriteLine("Handle order shipped called");
            using (var scope = provider.CreateScope())
            {
                var services = scope.ServiceProvider;
                var emailService = services.GetService<IEmailService>();
                var ChangeCustomerCreditStandingCommandHandler = services.GetService<ICommandHandler<ChangeCustomerCreditStanding>>();
                var eventStore = services.GetService<EventStoreClient>();
                var eventDeserializer = services.GetService<EventDeserializer>();
                var cancellationToken = new CancellationToken();
                var productServiceGateway = services.GetService<IServiceGateway<ProductDto>>();


                if (message.CustomerId != Guid.Empty && ChangeCustomerCreditStandingCommandHandler != null && emailService != null && productServiceGateway != null)
                {

                    await ChangeCustomerCreditStandingCommandHandler.HandleAsync(new ChangeCustomerCreditStanding { Id = (Guid)message.CustomerId, CreditStanding = false });

                    var customer = await eventStore.Find<Customer, Guid>((Guid)message.CustomerId, eventDeserializer, cancellationToken);

                    if (customer == null)
                        return;

                    decimal totalPrice = 0;

                    var sb = new System.Text.StringBuilder();
                    sb.AppendLine($"Dear {customer.CompanyName}");
                    sb.AppendLine($"Order with order id number: {message.OrderLines[0].OrderId} has been shipped.");
                    sb.AppendLine($"The following is a list of the products and the number of ordered products");
                    foreach (OrderLineDto orderline in message.OrderLines)
                    {
                        var product = productServiceGateway.Get(orderline.ProductId);
                        sb.AppendLine($"Product name: {product.Name}, Price: {product.Price}, Number of ordered items: {orderline.NoOfItems}");

                        totalPrice += product.Price * orderline.NoOfItems;
                    }

                    sb.AppendLine($"Total price: {totalPrice}kr.");
                    sb.AppendLine($"Until this invoice is paid you will be unable to make further orders");
                    sb.AppendLine($"Please pay");

                    emailService.SendEmail(customer.Email, "Order shipped", sb.ToString());

                }

            }
        }


    }
}
