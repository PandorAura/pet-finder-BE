using AnimalAdoption.Models.DTOs;

namespace AnimalAdoption.Services
{
    public interface IAdoptionService
    {
        Task<IEnumerable<AdoptionDto>> GetAdoptionsByUserAsync(int userId);
        Task<AdoptionDto> AddAdoptionAsync(int animalId, int userId);
        Task DeleteAdoptionAsync(int adoptionId);
    }
}
