namespace OrderApiQ.Data
{
    public interface IDbInitializer
    {
        void Initialize(OrderApiContext context);
    }
}
