using AnimalAdoption.Data;
using AnimalAdoption.Models;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;
using System.Linq;

namespace AnimalAdoption.Repositories
{
    public class AnimalRepository : IAnimalRepository
    {

            private readonly AnimalAdoptionContext _context;

            public AnimalRepository(AnimalAdoptionContext context)
            {
                _context = context;
            }

            public async Task<IEnumerable<Animal>> GetAllAsync()
            {
                return await _context.Animals.ToListAsync();
            }

            public async Task<Animal?> GetByIdAsync(int id)
            {
                return await _context.Animals.FindAsync(id);
            }

            public async Task<IEnumerable<Animal>> FindAsync(Expression<Func<Animal, bool>> predicate)
            {
                return await _context.Animals.Where(predicate).ToListAsync();
            }

            public async Task AddAsync(Animal animal)
            {
                await _context.Animals.AddAsync(animal);
                await _context.SaveChangesAsync();
            }

            public async Task UpdateAsync(Animal animal)
            {
                _context.Animals.Update(animal);
                await _context.SaveChangesAsync();
            }

            public async Task DeleteAsync(int id)
            {
                var animal = await _context.Animals.FindAsync(id);
                if (animal != null)
                {
                    _context.Animals.Remove(animal);
                    await _context.SaveChangesAsync();
                }
            }

            public async Task<IEnumerable<Animal>> GetFilteredAndSortedAsync(
                Expression<Func<Animal, bool>>? filter = null,
                Func<IQueryable<Animal>, IOrderedQueryable<Animal>>? orderBy = null,
                string includeProperties = "")
            {
                IQueryable<Animal> query = _context.Animals;

                if (filter != null)
                {
                    query = query.Where(filter);
                }

                foreach (var includeProperty in includeProperties.Split
                    (new[] { ',' }, StringSplitOptions.RemoveEmptyEntries))
                {
                    query = query.Include(includeProperty);
                }

                if (orderBy != null)
                {
                    return await orderBy(query).ToListAsync();
                }

                return await query.ToListAsync();
            }
        }

        
}

