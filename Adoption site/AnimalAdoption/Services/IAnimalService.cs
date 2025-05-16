using AnimalAdoption.Models;
using System.Linq.Expressions;

namespace AnimalAdoption.Services
{
    public interface IAnimalService
    {
        Task<IEnumerable<Animal>> GetAllAnimalsAsync();
        Task<Animal?> GetAnimalByIdAsync(int id);
        Task AddAnimalAsync(Animal animal);
        Task UpdateAnimalAsync(Animal animal);
        Task DeleteAnimalAsync(int id);
        Task AdoptAnimalAsync(int animalId, int adopterId);
        Task ReturnAnimalAsync(int animalId);
        Task<IEnumerable<Animal>> SearchAnimalsAsync(string searchTerm);
        Task<IEnumerable<Animal>> GetAnimalsBySpeciesAsync(string species);
        Task<IEnumerable<Animal>> GetAvailableAnimalsAsync();
        Task<IEnumerable<Animal>> GetAdoptedAnimalsAsync();
        Task<IEnumerable<Animal>> GetFilteredAndSortedAnimalsAsync(
            Expression<Func<Animal, bool>>? filter = null,
            Func<IQueryable<Animal>, IOrderedQueryable<Animal>>? orderBy = null,
            string includeProperties = "");
    }
}
