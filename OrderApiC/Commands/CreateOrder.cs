using SharedModels;
using SharedModels.EventStoreCQRS;

namespace OrderApiC.Commands
{
    public class CreateOrder : ICommand
    {
        public Guid Id { get; set; }

        public Guid CustomerId { get; set; }

        public OrderStatus Status { get; set; }

        public List<OrderLine> OrderLines { get; set; }

        public CreateOrder(List<OrderLine> orderLines)
        {
            OrderLines = orderLines;
        }
    }
}
