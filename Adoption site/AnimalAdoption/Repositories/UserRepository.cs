using AnimalAdoption.Data;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;
using System.Linq;
using AnimalAdoption.Models;

namespace AnimalAdoption.Repositories
{
    public class UserRepository : IUserRepository
    {
            private readonly AnimalAdoptionContext _context;

            public UserRepository(AnimalAdoptionContext context)
            {
                _context = context;
            }

            public async Task<IEnumerable<User>> GetAllAsync()
            {
                return await _context.Users.ToListAsync();
            }

            public async Task<User?> GetByIdAsync(int id)
            {
                return await _context.Users.FindAsync(id);
            }

            public async Task<IEnumerable<User>> FindAsync(Expression<Func<User, bool>> predicate)
            {
                return await _context.Users.Where(predicate).ToListAsync();
            }

            public async Task AddAsync(User adopter)
            {
                await _context.Users.AddAsync(adopter);
                await _context.SaveChangesAsync();
            }

            public async Task UpdateAsync(User adopter)
            {
                _context.Users.Update(adopter);
                await _context.SaveChangesAsync();
            }

        public async Task DeleteAsync(int id)
        {
            var adopter = await _context.Users.FindAsync(id);
            if (adopter != null)
            {
                // Get all adoptions by this user
                var adoptions = await _context.Adoptions
                    .Where(a => a.UserId == id)
                    .ToListAsync();

                // For each adoption, mark the animal as not adopted
                foreach (var adoption in adoptions)
                {
                    var animal = await _context.Animals.FindAsync(adoption.AnimalId);
                    if (animal != null)
                    {
                        animal.IsAdopted = false;
                    }
                }

                // Remove all adoptions by this user
                _context.Adoptions.RemoveRange(adoptions);

                // Remove the user
                _context.Users.Remove(adopter);

                await _context.SaveChangesAsync();
            }
        }


        public async Task<IEnumerable<User>> GetFilteredAndSortedAsync(
                Expression<Func<User, bool>>? filter = null,
                Func<IQueryable<User>, IOrderedQueryable<User>>? orderBy = null,
                string includeProperties = "")
            {
                IQueryable<User> query = _context.Users;

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

