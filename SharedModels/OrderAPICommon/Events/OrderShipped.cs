using SharedModels.EventStoreCQRS;

namespace SharedModels.OrderAPICommon.Events
{
    public class OrderShipped : IEvent
    {
        public Guid Id { get; set; }

        public OrderStatusDto Status { get; set; }

        public DateTime ShippedAt { get; set; }

    }
}
