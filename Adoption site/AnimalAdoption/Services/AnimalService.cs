using AnimalAdoption.Models;
using static AnimalAdoption.Repositories.AnimalRepository;
using System.Linq.Expressions;
using AnimalAdoption.Repositories;

namespace AnimalAdoption.Services
{
        public class AnimalService : IAnimalService
        {
            private readonly IAnimalRepository _animalRepository;
            private readonly IUserRepository _adopterRepository;

            public AnimalService(IAnimalRepository animalRepository, IUserRepository adopterRepository)
            {
                _animalRepository = animalRepository;
                _adopterRepository = adopterRepository;
            }

            public async Task<IEnumerable<Animal>> GetAllAnimalsAsync()
            {
                return await _animalRepository.GetAllAsync();
            }

            public async Task<Animal?> GetAnimalByIdAsync(int id)
            {
                return await _animalRepository.GetByIdAsync(id);
            }

            public async Task AddAnimalAsync(Animal animal)
            {
                if (animal == null) throw new ArgumentNullException(nameof(animal));

                animal.IsAdopted = false;
                animal.AdopterId = null;
                await _animalRepository.AddAsync(animal);
            }

            public async Task UpdateAnimalAsync(Animal animal)
            {
                if (animal == null) throw new ArgumentNullException(nameof(animal));

                await _animalRepository.UpdateAsync(animal);
            }

            public async Task DeleteAnimalAsync(int id)
            {
                await _animalRepository.DeleteAsync(id);
            }

            public async Task AdoptAnimalAsync(int animalId, int adopterId)
            {
                var animal = await _animalRepository.GetByIdAsync(animalId);
                if (animal == null) throw new ArgumentException("Animal not found");

                var adopter = await _adopterRepository.GetByIdAsync(adopterId);
                if (adopter == null) throw new ArgumentException("Adopter not found");

                animal.IsAdopted = true;
                animal.AdopterId = adopterId;
                await _animalRepository.UpdateAsync(animal);
            }

            public async Task ReturnAnimalAsync(int animalId)
            {
                var animal = await _animalRepository.GetByIdAsync(animalId);
                if (animal == null) throw new ArgumentException("Animal not found");

                animal.IsAdopted = false;
                animal.AdopterId = null;
                await _animalRepository.UpdateAsync(animal);
            }

            public async Task<IEnumerable<Animal>> SearchAnimalsAsync(string searchTerm)
            {
                if (string.IsNullOrWhiteSpace(searchTerm))
                    return await _animalRepository.GetAllAsync();

                return await _animalRepository.FindAsync(a =>
                    a.Name.Contains(searchTerm) ||
                    a.Description.Contains(searchTerm) ||
                    a.Breed.Contains(searchTerm) ||
                    a.Species.Contains(searchTerm));
            }

            public async Task<IEnumerable<Animal>> GetAnimalsBySpeciesAsync(string species)
            {
                return await _animalRepository.FindAsync(a => a.Species == species);
            }

            public async Task<IEnumerable<Animal>> GetAvailableAnimalsAsync()
            {
                return await _animalRepository.FindAsync(a => !a.IsAdopted);
            }

            public async Task<IEnumerable<Animal>> GetAdoptedAnimalsAsync()
            {
                return await _animalRepository.FindAsync(a => a.IsAdopted);
            }

            public async Task<IEnumerable<Animal>> GetFilteredAndSortedAnimalsAsync(
                Expression<Func<Animal, bool>>? filter = null,
                Func<IQueryable<Animal>, IOrderedQueryable<Animal>>? orderBy = null,
                string includeProperties = "")
            {
                return await _animalRepository.GetFilteredAndSortedAsync(filter, orderBy, includeProperties);
            }
        }

    
}

