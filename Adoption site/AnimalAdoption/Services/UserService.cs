using AnimalAdoption.Models;
using static AnimalAdoption.Repositories.AnimalRepository;
using System.Linq.Expressions;
using AnimalAdoption.Repositories;

namespace AnimalAdoption.Services
{
    public class UserService : IUserService
    {
        private readonly IUserRepository _adopterRepository;
        private readonly IAnimalRepository _animalRepository;

        public UserService(IUserRepository adopterRepository, IAnimalRepository animalRepository)
        {
            _adopterRepository = adopterRepository;
            _animalRepository = animalRepository;
        }

        public async Task<IEnumerable<User>> GetAllAdoptersAsync()
        {
            return await _adopterRepository.GetAllAsync();
        }

        public async Task<User?> GetAdopterByIdAsync(int id)
        {
            return await _adopterRepository.GetByIdAsync(id);
        }

        public async Task AddAdopterAsync(User adopter)
        {
            if (adopter == null) throw new ArgumentNullException(nameof(adopter));

            adopter.RegistrationDate = DateTime.Now;
            await _adopterRepository.AddAsync(adopter);
        }

        public async Task UpdateAdopterAsync(User adopter)
        {
            if (adopter == null) throw new ArgumentNullException(nameof(adopter));

            await _adopterRepository.UpdateAsync(adopter);
        }

        public async Task DeleteAdopterAsync(int id)
        {
            await _adopterRepository.DeleteAsync(id);
        }

        public async Task<IEnumerable<User>> SearchAdoptersAsync(string searchTerm)
        {
            if (string.IsNullOrWhiteSpace(searchTerm))
                return await _adopterRepository.GetAllAsync();

            return await _adopterRepository.FindAsync(a =>
                a.FirstName.Contains(searchTerm) ||
                a.LastName.Contains(searchTerm) ||
                a.Email.Contains(searchTerm) ||
                a.PhoneNumber.Contains(searchTerm) ||
                a.Address.Contains(searchTerm));
        }

        public async Task<IEnumerable<User>> GetFilteredAndSortedAdoptersAsync(
            Expression<Func<User, bool>>? filter = null,
            Func<IQueryable<User>, IOrderedQueryable<User>>? orderBy = null,
            string includeProperties = "")
        {
            return await _adopterRepository.GetFilteredAndSortedAsync(filter, orderBy, includeProperties);
        }
    }
}
