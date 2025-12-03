using EventManagement.DTOs;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace EventManagement.Repositories
{
    public interface IEventManagementRepository<T> where T : class
    {
        Task<IEnumerable<T>> GetAllAsync();
        Task<T> GetByIdAsync(int id);
        Task<T> AddAsync(T entity);
        Task<bool> UpdateAsync(T entity);
        Task<bool> DeleteAsync(int id);

        Task<T> GetFirstOrDefaultAsync(Expression<Func<T, bool>> predicate);

        Task<bool> ExistsAsync(Expression<Func<T, bool>> predicate);
        Task GetWhereAsync(Func<object, bool> value1, Func<object, RegistrationDTO> value2, Func<object, object> value3);
    }
}
