namespace SharedModels
{
    public class Order
    {
        public Guid OrderId { get; set; }
        public DateTime? Date { get; set; }
        public OrderStatus Status { get; set; }
        public Guid CustomerId { get; set; }
        public List<OrderLine> Orderlines { get; set; }

        public Order()
        {
            Orderlines = new List<OrderLine>();
        }
    }
}
 