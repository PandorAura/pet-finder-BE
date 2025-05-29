using AnimalAdoption.Models;
using AnimalAdoption.Models.DTOs;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace AnimalAdoption.Services
{
    public interface IAnimalService
    {
        Task<IEnumerable<AnimalSimpleDto>> GetAllAnimalsAsync();
        Task<AnimalDto?> GetAnimalByIdAsync(int id);
        Task<AnimalDto> AddAnimalAsync(AnimalCreateDto animalCreateDto);
        Task<AnimalDto> UpdateAnimalAsync(int id, AnimalUpdateDto animalUpdateDto);
        Task DeleteAnimalAsync(int id);

        // Adoption Operations
        Task<AnimalDto> AdoptAnimalAsync(int animalId, int userId);
        Task<AnimalDto> ReturnAnimalAsync(int animalId);

        // Search and Filter Operations
        Task<IEnumerable<AnimalSimpleDto>> SearchAnimalsAsync(string searchTerm);
        Task<IEnumerable<AnimalSimpleDto>> GetAnimalsBySpeciesAsync(string species);
        Task<IEnumerable<AnimalSimpleDto>> GetAvailableAnimalsAsync();
        Task<IEnumerable<AnimalSimpleDto>> GetAdoptedAnimalsAsync();

        // Advanced Query Operations
        Task<IEnumerable<AdoptionStatsDto>> GetAdoptionStatisticsAsync();
        Task<IEnumerable<AnimalSimpleDto>> GetFilteredAndSortedAnimalsAsync(
            Expression<Func<Animal, bool>>? filter = null,
            Func<IQueryable<Animal>, IOrderedQueryable<Animal>>? orderBy = null,
            string includeProperties = "");
    }
}
