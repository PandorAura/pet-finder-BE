using AnimalAdoption.Models;
using System.Linq.Expressions;

namespace AnimalAdoption.Repositories
{
    public interface IAdoptionRepository
    {
        Task<Adoption> GetByIdAsync(int id);
        Task<IEnumerable<Adoption>> FindAsync(Expression<Func<Adoption, bool>> predicate);
        Task AddAsync(Adoption adoption);
        Task UpdateAsync(Adoption adoption);
        Task DeleteAsync(Adoption adoption);
        Task<Adoption?> GetLatestByAnimalIdAsync(int animalId);

    }
}
