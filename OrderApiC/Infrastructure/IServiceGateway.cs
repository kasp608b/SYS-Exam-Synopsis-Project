namespace OrderApiC.Infrastructure
{
    public interface IServiceGateway<T>
    {
        T Get(Guid id);
    }
}
