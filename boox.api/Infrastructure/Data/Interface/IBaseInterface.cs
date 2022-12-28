using boox.api.Infrasructure.Models.Helpers;
using boox.api.Infrasructure.Models.Users.Helpers;

namespace boox.api.Infrastructure.Data.Interface
{
    public interface IBaseInterface<T> where T : class
    {
        Task<T> Add(T entity);
        Task<ProcessResult> Update(T entity);
        Task<ProcessResult> Delete(int ID);
        Task<T> Get(int ID);
        Task<IEnumerable<T>> GetAll();
        Task<FilteredList<T>> FilteredList(FilteredList<T> request);
    }
}
