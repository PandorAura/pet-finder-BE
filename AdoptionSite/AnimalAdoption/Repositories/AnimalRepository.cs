using AnimalAdoption.Data;
using AnimalAdoption.Models;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;
using System.Linq;
using Microsoft.Data.SqlClient;
using AnimalAdoption.Models.DTOs;

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

        public async Task<IEnumerable<AdoptionStatsDto>> GetAdoptionStatisticsAsync()
        {
            var grouped = await _context.Animals
                .AsNoTracking()
                .SelectMany(a => a.Adoptions.DefaultIfEmpty(), (animal, adoption) => new
                {
                    animal.Species,
                    animal.ArrivalDate,
                    Year = animal.ArrivalDate.Year,
                    Month = animal.ArrivalDate.Month,
                    DaysInShelter = adoption != null
                        ? EF.Functions.DateDiffDay(animal.ArrivalDate, adoption.AdoptionDate)
                        : EF.Functions.DateDiffDay(animal.ArrivalDate, DateTime.Now),
                    Adopted = adoption != null ? 1 : 0
                })
                .GroupBy(x => new { x.Species, x.Year, x.Month })
                .Select(g => new AdoptionStatsDto
                {
                    Species = g.Key.Species,
                    Year = g.Key.Year,
                    Month = g.Key.Month,
                    TotalAnimals = g.Count(),
                    AdoptedCount = g.Sum(x => x.Adopted),
                    AdoptionRate = (decimal)g.Sum(x => x.Adopted) / g.Count() * 100,
                    AvgDaysInShelter = g.Average(x => x.DaysInShelter)
                })
                .OrderBy(x => x.Species)
                .ThenBy(x => x.Year)
                .ThenBy(x => x.Month)
                .ToListAsync();

            return grouped;
        }


    }
}

