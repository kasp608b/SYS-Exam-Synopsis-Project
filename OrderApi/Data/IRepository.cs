using System.Collections.Generic;

namespace OrderApi.Data
{
    public interface IRepository<T>
    {
        IEnumerable<T> GetAll();
        T Get(Guid id);
        T Add(T entity);
        void Edit(T entity);
        void Remove(Guid id);
    }
}
