﻿using SharedModels.EventStoreCQRS;

namespace SharedModels.OrderAPICommon.Events
{
    public class OrderCanceled : IEvent
    {
        public Guid Id { get; set; }

        public OrderStatusDto Status { get; set; }

        public DateTime CanceledAt { get; set; }

    }
}
