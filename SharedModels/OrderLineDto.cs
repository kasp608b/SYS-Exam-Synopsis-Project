namespace SharedModels
{
    public class OrderLineDto
    {
        public Guid OrderId { get; set; }
        public Guid ProductId { get; set; }
        public int NoOfItems { get; set; }
    }
}