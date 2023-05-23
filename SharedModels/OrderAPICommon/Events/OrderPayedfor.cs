using SharedModels.EventStoreCQRS;

namespace SharedModels.OrderAPICommon.Events
{
    public class OrderPayedfor : IEvent
    {
        public Guid Id { get; set; }

        public OrderStatusDto Status { get; set; }

        public DateTime PayedForAt { get; set; }

    }
}
