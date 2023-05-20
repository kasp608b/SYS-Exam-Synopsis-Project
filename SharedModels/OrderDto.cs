namespace SharedModels {
    public class OrderDto
    {
        public Guid OrderId { get; set; }
        public DateTime? Date { get; set; }
        public OrderStatusDto Status { get; set; }
        public Guid CustomerId { get; set; }
        public List<OrderLineDto> Orderlines { get; set; }

        public OrderDto()
        {
            Orderlines = new List<OrderLineDto>();
        }



    }
}