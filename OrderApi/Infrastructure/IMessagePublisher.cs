using SharedModels;

namespace OrderApi.Infrastructure
{
    public interface IMessagePublisher
    {
        void PublishCreditStandingChangedMessage(Guid customerId, bool newCreditStanding);
        void PublishOrderStatusChangedMessage(Guid? customerId,
            List<OrderLineDto> orderLines, string topic);

        void PublishOrderRejectedMessage(OrderDto order, string messageText);
    }
}
