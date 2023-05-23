using SharedModels.EventStoreCQRS;

namespace SharedModels.CustomerAPICommon.Events
{
    public class CustomerCreditStandingChanged : IEvent
    {
        public Guid Id { get; set; }

        public bool CreditStanding { get; set; }

        public DateTime CreditStandingChangedAt { get; set; }
    }
}
