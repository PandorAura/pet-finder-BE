using AnimalAdoption.Models;
using System.Linq.Expressions;

namespace AnimalAdoption.Repositories
{
    public interface IUserRepository
    {
        Task<IEnumerable<User>> GetAllAsync();
        Task<User?> GetByIdAsync(int id);
        Task<IEnumerable<User>> FindAsync(Expression<Func<User, bool>> predicate);
        Task AddAsync(User adopter);
        Task UpdateAsync(User adopter);
        Task DeleteAsync(int id);
        Task<IEnumerable<User>> GetFilteredAndSortedAsync(
            Expression<Func<User, bool>>? filter = null,
            Func<IQueryable<User>, IOrderedQueryable<User>>? orderBy = null,
            string includeProperties = "");
    }
}
