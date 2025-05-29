using AnimalAdoption.Models;
using AnimalAdoption.Models.DTOs;
using AnimalAdoption.Repositories;
using AutoMapper;
using System.Linq.Expressions;

namespace AnimalAdoption.Services
{
    public class AnimalService : IAnimalService
    {
        private readonly IAnimalRepository _animalRepository;
        private readonly IUserRepository _userRepository;
        private readonly IAdoptionRepository _adoptionRepository;
        private readonly IMapper _mapper;

        public AnimalService(
            IAnimalRepository animalRepository,
            IUserRepository userRepository,
            IAdoptionRepository adoptionRepository,
            IMapper mapper)
        {
            _animalRepository = animalRepository;
            _userRepository = userRepository;
            _adoptionRepository = adoptionRepository;
            _mapper = mapper;
        }

        public async Task<IEnumerable<AnimalSimpleDto>> GetAllAnimalsAsync()
            => _mapper.Map<IEnumerable<AnimalSimpleDto>>(await _animalRepository.GetAllAsync());

        public async Task<AnimalDto?> GetAnimalByIdAsync(int id)
            => _mapper.Map<AnimalDto>(await _animalRepository.GetByIdAsync(id));

        public async Task<AnimalDto> AddAnimalAsync(AnimalCreateDto animalCreateDto)
        {
            if (animalCreateDto == null)
                throw new ArgumentNullException(nameof(animalCreateDto));

            var animal = _mapper.Map<Animal>(animalCreateDto);
            animal.IsAdopted = false;

            await _animalRepository.AddAsync(animal);
            return _mapper.Map<AnimalDto>(animal);
        }

        public async Task<AnimalDto> UpdateAnimalAsync(int id, AnimalUpdateDto animalUpdateDto)
        {
            var existingAnimal = await GetExistingAnimalOrThrowAsync(id);
            _mapper.Map(animalUpdateDto, existingAnimal);
            await _animalRepository.UpdateAsync(existingAnimal);
            return _mapper.Map<AnimalDto>(existingAnimal);
        }

        public Task DeleteAnimalAsync(int id)
            => _animalRepository.DeleteAsync(id);

        public async Task<AnimalDto> AdoptAnimalAsync(int animalId, int userId)
        {
            var animal = await GetExistingAnimalOrThrowAsync(animalId);
            var user = await GetExistingUserOrThrowAsync(userId);

            if (animal.IsAdopted)
                throw new InvalidOperationException("Animal is already adopted.");

            animal.IsAdopted = true;

            var adoption = new Adoption
            {
                AnimalId = animalId,
                UserId = userId,
                AdoptionDate = DateTime.UtcNow
            };

            await _adoptionRepository.AddAsync(adoption);
            await _animalRepository.UpdateAsync(animal);

            return _mapper.Map<AnimalDto>(animal);
        }

        public async Task<AnimalDto> ReturnAnimalAsync(int animalId)
        {
            var animal = await GetExistingAnimalOrThrowAsync(animalId);

            if (!animal.IsAdopted)
                throw new InvalidOperationException("Animal is not currently adopted.");

            animal.IsAdopted = false;

            var adoption = await _adoptionRepository.GetLatestByAnimalIdAsync(animalId);
            if (adoption != null)
                await _adoptionRepository.DeleteAsync(adoption);

            await _animalRepository.UpdateAsync(animal);

            return _mapper.Map<AnimalDto>(animal);
        }

        public async Task<IEnumerable<AnimalSimpleDto>> SearchAnimalsAsync(string searchTerm)
        {
            if (string.IsNullOrWhiteSpace(searchTerm))
                return await GetAllAnimalsAsync();

            var animals = await _animalRepository.FindAsync(a =>
                a.Name.Contains(searchTerm) ||
                a.Description.Contains(searchTerm) ||
                a.Breed.Contains(searchTerm) ||
                a.Species.Contains(searchTerm));

            return _mapper.Map<IEnumerable<AnimalSimpleDto>>(animals);
        }

        public Task<IEnumerable<AnimalSimpleDto>> GetAnimalsBySpeciesAsync(string species)
            => MapAnimalsAsync(_animalRepository.FindAsync(a => a.Species == species));

        public Task<IEnumerable<AnimalSimpleDto>> GetAvailableAnimalsAsync()
            => MapAnimalsAsync(_animalRepository.FindAsync(a => !a.IsAdopted));

        public Task<IEnumerable<AnimalSimpleDto>> GetAdoptedAnimalsAsync()
            => MapAnimalsAsync(_animalRepository.FindAsync(a => a.IsAdopted));

        public async Task<IEnumerable<AnimalSimpleDto>> GetFilteredAndSortedAnimalsAsync(
            Expression<Func<Animal, bool>>? filter = null,
            Func<IQueryable<Animal>, IOrderedQueryable<Animal>>? orderBy = null,
            string includeProperties = "")
        {
            var animals = await _animalRepository.GetFilteredAndSortedAsync(filter, orderBy, includeProperties);
            return _mapper.Map<IEnumerable<AnimalSimpleDto>>(animals);
        }

        // Private helper methods

        private async Task<Animal> GetExistingAnimalOrThrowAsync(int id)
        {
            var animal = await _animalRepository.GetByIdAsync(id);
            if (animal == null)
                throw new ArgumentException("Animal not found");
            return animal;
        }

        private async Task<User> GetExistingUserOrThrowAsync(int id)
        {
            var user = await _userRepository.GetByIdAsync(id);
            if (user == null)
                throw new ArgumentException("User not found");
            return user;
        }

        private async Task<IEnumerable<AnimalSimpleDto>> MapAnimalsAsync(Task<IEnumerable<Animal>> animalsTask)
        {
            var animals = await animalsTask;
            return _mapper.Map<IEnumerable<AnimalSimpleDto>>(animals);
        }

        public async Task<IEnumerable<AdoptionStatsDto>> GetAdoptionStatisticsAsync()
        {
            var animals = await _animalRepository.GetAdoptionStatisticsAsync();
            return animals;
        }
    }
}
