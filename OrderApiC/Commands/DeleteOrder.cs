using SharedModels.EventStoreCQRS;

namespace OrderApiC.Commands
{
    public class DeleteOrder : ICommand
    {
        public Guid Id { get; set; }
    }
}