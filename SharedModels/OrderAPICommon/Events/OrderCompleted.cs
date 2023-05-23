using SharedModels.EventStoreCQRS;

namespace SharedModels.OrderAPICommon.Events
{
    public class OrderCompleted : IEvent
    {
        public Guid Id { get; set; }

        public OrderStatusDto Status { get; set; }

        public DateTime CompletedAt { get; set; }

    }
}
