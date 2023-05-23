using SharedModels.EventStoreCQRS;

namespace SharedModels.OrderAPICommon.Events
{
    internal class OrderDeleted : IEvent
    {
        public Guid Id { get; set; }

        public DateTime DeletedAt { get; set; }

    }
}
