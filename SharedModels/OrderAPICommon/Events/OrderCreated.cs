using SharedModels.EventStoreCQRS;

namespace SharedModels.OrderAPICommon.Events
{
    internal class OrderCreated : IEvent
    {
        public Guid Id { get; set; }

        public Guid CustomerId { get; set; }

        public DateTime? Date { get; set; }

        public OrderStatusDto Status { get; set; }

        public List<OrderLineDto> Orderlines { get; set; }

        public DateTime CreatedtAt { get; set; }

        public OrderCreated()
        {
            Orderlines = new List<OrderLineDto>();
        }
    }
}
