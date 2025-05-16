using AnimalAdoption.Models;
using System.Linq.Expressions;

namespace AnimalAdoption.Repositories
{
    public interface IAnimalRepository
    {
        Task<IEnumerable<Animal>> GetAllAsync();
        Task<Animal?> GetByIdAsync(int id);
        Task<IEnumerable<Animal>> FindAsync(Expression<Func<Animal, bool>> predicate);
        Task AddAsync(Animal animal);
        Task UpdateAsync(Animal animal);
        Task DeleteAsync(int id);
        Task<IEnumerable<Animal>> GetFilteredAndSortedAsync(
            Expression<Func<Animal, bool>>? filter = null,
            Func<IQueryable<Animal>, IOrderedQueryable<Animal>>? orderBy = null,
            string includeProperties = "");
    }
}
