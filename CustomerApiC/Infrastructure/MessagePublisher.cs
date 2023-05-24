﻿using EasyNetQ;

namespace CustomerApi.Infrastructure
{
    public class MessagePublisher : IMessagePublisher, IDisposable
    {
        IBus bus;

        public MessagePublisher(string connectionString)
        {
            bus = RabbitHutch.CreateBus(connectionString);
        }

        public void Dispose()
        {
            bus.Dispose();
        }

        //public void PublishOrderStatusChangedMessage(int? customerId, List<OrderLine> orderLines, string topic)
        //{
        //    var message = new OrderStatusChangedMessage
        //    {
        //        CustomerId = customerId,
        //        OrderLines = orderLines
        //    };
        //    Console.WriteLine(message + "message published" + topic);
        //    bus.PubSub.Publish(message, topic);
        //}

    }
}
