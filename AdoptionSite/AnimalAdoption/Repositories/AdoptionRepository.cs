using AnimalAdoption.Data;
using AnimalAdoption.Models;
using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace AnimalAdoption.Repositories
{
    public class AdoptionRepository : IAdoptionRepository
    {
        private readonly AnimalAdoptionContext _context;

        public AdoptionRepository(AnimalAdoptionContext context)
        {
            _context = context;
        }

        public async Task<Adoption> GetByIdAsync(int id)
        {
            return await _context.Set<Adoption>()
                .Include(a => a.Animal)
                .Include(a => a.User)
                .FirstOrDefaultAsync(a => a.Id == id);
        }

        public async Task<IEnumerable<Adoption>> FindAsync(Expression<Func<Adoption, bool>> predicate)
        {
            return await _context.Set<Adoption>()
                .Include(a => a.Animal)
                .Include(a => a.User)
                .Where(predicate)
                .ToListAsync();
        }

        public async Task AddAsync(Adoption adoption)
        {
            await _context.Set<Adoption>().AddAsync(adoption);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(Adoption adoption)
        {
            _context.Set<Adoption>().Update(adoption);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(Adoption adoption)
        {
            _context.Set<Adoption>().Remove(adoption);
            await _context.SaveChangesAsync();
        }
        public async Task<Adoption?> GetLatestByAnimalIdAsync(int animalId)
        {
            return await _context.Adoptions
                .Where(a => a.AnimalId == animalId)
                .OrderByDescending(a => a.AdoptionDate)
                .FirstOrDefaultAsync();
        }

    }
}

