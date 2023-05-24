using SharedModels;

namespace OrderApiQ.Data
{
    public interface IOrderRepository : IRepository<Order>
    {
        IEnumerable<Order> GetByCustomer(Guid customerId);
    }
}
