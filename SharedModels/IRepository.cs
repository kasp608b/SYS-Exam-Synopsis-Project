using System.Collections.Generic;

namespace SharedModels
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
