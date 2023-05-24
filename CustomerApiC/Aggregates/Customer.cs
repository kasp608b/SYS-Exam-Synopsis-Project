using SharedModels.CustomerAPICommon.Events;
using SharedModels.EventStoreCQRS;

namespace CustomerApiC.Aggregates
{
    public class Customer : Aggregate<Guid>
    {
        public Guid CustomerId { get; set; }

        public string CompanyName { get; set; }

        public string RegistrationNumber { get; set; }

        public string Email { get; set; }

        public string Phone { get; set; }

        public string BillingAddress { get; set; }

        public string ShippingAddress { get; set; }

        public bool CreditStanding { get; set; }

        public bool Deleted { get; set; }

        public override void When(object @event)
        {
            switch (@event)
            {
                case CustomerCreated CustomerCreated:
                    Apply(CustomerCreated);
                    break;
                case CustomerInfoChanged customerInfoChanged:
                    Apply(customerInfoChanged);
                    break;
                case CustomerCreditStandingChanged customerCreditStandingChanged:
                    Apply(customerCreditStandingChanged);
                    break;
                case CustomerDeleted customerDeleted:
                    Apply(customerDeleted);
                    break;
                default:
                    throw new InvalidOperationException($"Unknown event type: {@event.GetType().Name}");

            }

        }

        private void Apply(CustomerCreated @event)
        {
            CustomerId = @event.Id;
            CompanyName = @event.CompanyName;
            RegistrationNumber = @event.RegistrationNumber;
            Email = @event.Email;
            Phone = @event.Phone;
            BillingAddress = @event.BillingAddress;
            ShippingAddress = @event.ShippingAddress;
            CreditStanding = @event.CreditStanding;


        }
        private void Apply(CustomerInfoChanged @event)
        {
            CustomerId = @event.Id;
            CompanyName = @event.CompanyName;
            RegistrationNumber = @event.RegistrationNumber;
            Email = @event.Email;
            Phone = @event.Phone;
            BillingAddress = @event.BillingAddress;
            ShippingAddress = @event.ShippingAddress;
        }
        private void Apply(CustomerCreditStandingChanged @event)
        {
            CustomerId = @event.Id;
            CreditStanding = @event.CreditStanding;


        }

        private void Apply(CustomerDeleted @event)
        {
            Deleted = true;

        }





    }
}
