using SharedModels.EventStoreCQRS;

namespace CustomerApiC.Commands
{
    public class ChangeCustomerInfo : ICommand
    {
        public Guid Id { get; set; }

        public string CompanyName { get; set; }

        public string RegistrationNumber { get; set; }

        public string Email { get; set; }

        public string Phone { get; set; }

        public string BillingAddress { get; set; }

        public string ShippingAddress { get; set; }
    }
}
