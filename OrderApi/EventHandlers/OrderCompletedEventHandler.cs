﻿using OrderApiQ.Data;
using OrderApiQ.Models.Converters;
using SharedModels.EventStoreCQRS;
using SharedModels.OrderAPICommon.Events;

namespace OrderApiQ.EventHandlers
{
    public class OrderCompletedEventHandler : IEventHandler<OrderCompleted>
    {
        public Task HandleAsync(OrderCompleted @event, IServiceProvider provider)
        {
            using (var scope = provider.CreateScope())
            {

                var services = scope.ServiceProvider;
                var repository = services.GetService<IOrderRepository>();

                var order = repository.Get(@event.Id);

                if (order == null)
                    throw new InvalidOperationException("Order not found, cannot complete Order that does not exist.");

                order.Status = new OrderStatusConverter().Convert(@event.Status);

                repository.Edit(order);

                return Task.CompletedTask;
            }
        }
    }
}
