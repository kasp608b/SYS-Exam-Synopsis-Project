using SharedModels.EventStoreCQRS;

namespace SharedModels.CustomerAPICommon.Events
{
    public class CustomerInfoChanged : IEvent
    {
        public Guid Id { get; set; }

        public string CompanyName { get; set; }

        public string RegistrationNumber { get; set; }

        public string Email { get; set; }

        public string Phone { get; set; }

        public string BillingAddress { get; set; }

        public string ShippingAddress { get; set; }

        public bool CreditStanding { get; set; }

        public DateTime CreatedAt { get; set; }
    }
}
