using SharedModels.EventStoreCQRS;

namespace SharedModels.OrderAPICommon.Events
{
    public class OrderDeleted : IEvent
    {
        public Guid Id { get; set; }

        public DateTime DeletedAt { get; set; }

    }
}
