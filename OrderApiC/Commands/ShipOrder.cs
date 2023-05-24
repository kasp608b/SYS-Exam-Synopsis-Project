using SharedModels;
using SharedModels.EventStoreCQRS;

namespace OrderApiC.Commands
{
    public class ShipOrder : ICommand
    {
        public Guid Id { get; set; }

        public OrderStatus Status { get; set; }
    }
}