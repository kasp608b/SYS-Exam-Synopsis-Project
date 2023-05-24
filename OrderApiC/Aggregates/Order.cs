using OrderApiC.Models.Converters;
using SharedModels.EventStoreCQRS;
using SharedModels.OrderAPICommon.Events;

namespace OrderApiC.Aggregates
{
    public class OrderAggregate : Aggregate<Guid>
    {
        public Guid OrderId { get; set; }
        public DateTime? Date { get; set; }
        public OrderStatus Status { get; set; }
        public Guid CustomerId { get; set; }
        public List<OrderLine> Orderlines { get; set; }
        public bool Deleted { get; set; }

        public OrderAggregate()
        {
            Orderlines = new List<OrderLine>();
        }

        public override void When(object @event)
        {
            switch (@event)
            {
                case OrderCreated orderCreated:
                    Apply(orderCreated);
                    break;
                case OrderDeleted orderDeleted:
                    Apply(orderDeleted);
                    break;
                case OrderCanceled orderCanceled:
                    Apply(orderCanceled);
                    break;
                case OrderPayedfor orderPayedfor:
                    Apply(orderPayedfor);
                    break;
                case OrderShipped orderShipped:
                    Apply(orderShipped);
                    break;
                case OrderCompleted orderCompleted:
                    Apply(orderCompleted);
                    break;
                default:
                    throw new InvalidOperationException($"Unknown event type: {@event.GetType().Name}");
            }
        }

        private void Apply(OrderCreated @event)
        {
            OrderId = @event.Id;
            CustomerId = @event.CustomerId;
            Date = @event.Date;
            Status = new OrderStatusConverter().Convert(@event.Status);
            Orderlines = @event.Orderlines.Select(x => new OrderlineConverter().Convert(x)).ToList();

        }

        private void Apply(OrderDeleted @event)
        {
            Deleted = true;
        }

        private void Apply(OrderCanceled @event)
        {
            Status = new OrderStatusConverter().Convert(@event.Status);
        }

        private void Apply(OrderPayedfor @event)
        {
            Status = new OrderStatusConverter().Convert(@event.Status);
        }

        private void Apply(OrderShipped @event)
        {
            Status = new OrderStatusConverter().Convert(@event.Status);
        }

        private void Apply(OrderCompleted @event)
        {
            Status = new OrderStatusConverter().Convert(@event.Status);
        }
    }
}
