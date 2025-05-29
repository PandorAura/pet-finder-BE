using AnimalAdoption.Models;
using AnimalAdoption.Models.DTOs;
using AnimalAdoption.Repositories;
using AutoMapper;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
namespace AnimalAdoption.Services
{
    public class AdoptionService : IAdoptionService
    {
        private readonly IAdoptionRepository _adoptionRepository;
        private readonly IAnimalRepository _animalRepository;
        private readonly IUserRepository _userRepository;
        private readonly IMapper _mapper;

        public AdoptionService(
            IAdoptionRepository adoptionRepository,
            IAnimalRepository animalRepository,
            IUserRepository userRepository,
            IMapper mapper)
        {
            _adoptionRepository = adoptionRepository;
            _animalRepository = animalRepository;
            _userRepository = userRepository;
            _mapper = mapper;
        }

        public async Task<IEnumerable<AdoptionDto>> GetAdoptionsByUserAsync(int userId)
        {
            var adoptions = await _adoptionRepository.FindAsync(a => a.UserId == userId);
            return _mapper.Map<IEnumerable<AdoptionDto>>(adoptions);
        }

        public async Task<AdoptionDto> AddAdoptionAsync(int animalId, int userId)
        {
            var animal = await _animalRepository.GetByIdAsync(animalId);
            if (animal == null) throw new ArgumentException("Animal not found");

            var user = await _userRepository.GetByIdAsync(userId);
            if (user == null) throw new ArgumentException("User not found");

            if (animal.IsAdopted) throw new InvalidOperationException("Animal is already adopted");

            var adoption = new Adoption
            {
                AnimalId = animalId,
                UserId = userId,
                AdoptionDate = DateTime.UtcNow
            };

            // Update animal status only — no UserId anymore
            animal.IsAdopted = true;
            await _animalRepository.UpdateAsync(animal);

            await _adoptionRepository.AddAsync(adoption);

            return _mapper.Map<AdoptionDto>(adoption);
        }

        public async Task DeleteAdoptionAsync(int adoptionId)
        {
            var adoption = await _adoptionRepository.GetByIdAsync(adoptionId);
            if (adoption == null) throw new ArgumentException("Adoption not found");

            var animal = await _animalRepository.GetByIdAsync(adoption.AnimalId);
            if (animal != null)
            {
                animal.IsAdopted = false;
                await _animalRepository.UpdateAsync(animal);
            }

            await _adoptionRepository.DeleteAsync(adoption);
        }
    }


}
