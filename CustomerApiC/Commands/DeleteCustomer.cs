using SharedModels.EventStoreCQRS;

namespace CustomerApiC.Commands
{
    public class DeleteCustomer : ICommand
    {
        public Guid Id { get; set; }
    
    }
}
