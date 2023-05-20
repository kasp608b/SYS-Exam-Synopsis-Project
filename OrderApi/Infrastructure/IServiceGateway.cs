namespace OrderApi.Infrastructure
{
    public interface IServiceGateway<T>
    {
        T Get(Guid id);
    }
}
