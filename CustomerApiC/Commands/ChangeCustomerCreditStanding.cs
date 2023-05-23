using SharedModels.EventStoreCQRS;

namespace CustomerApiC.Commands
{
    public class ChangeCustomerCreditStanding : ICommand
    {
        public Guid Id { get; set; }

        public bool CreditStanding { get; set; }

    }
}
