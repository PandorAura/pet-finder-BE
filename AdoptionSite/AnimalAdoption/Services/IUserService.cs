using AnimalAdoption.Models;
using System.Linq.Expressions;

namespace AnimalAdoption.Services
{
    public interface IUserService
    {
        Task<IEnumerable<User>> GetAllAdoptersAsync();
        Task<User?> GetAdopterByIdAsync(int id);
        Task AddAdopterAsync(User adopter);
        Task UpdateAdopterAsync(User adopter);
        Task DeleteAdopterAsync(int id);
        Task<IEnumerable<User>> SearchAdoptersAsync(string searchTerm);
        Task<IEnumerable<User>> GetFilteredAndSortedAdoptersAsync(
            Expression<Func<User, bool>>? filter = null,
            Func<IQueryable<User>, IOrderedQueryable<User>>? orderBy = null,
            string includeProperties = "");
    }
}
