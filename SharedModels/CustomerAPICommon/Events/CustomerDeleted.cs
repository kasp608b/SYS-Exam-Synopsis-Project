using SharedModels.EventStoreCQRS;

namespace SharedModels.CustomerAPICommon.Events
{
    public class CustomerDeleted : IEvent
    {
        public Guid Id { get; set; }

        public DateTime DeletedAt { get; set; }
    }
}
