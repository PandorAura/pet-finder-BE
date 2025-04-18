using AnimalAdoption.Models;
using System.Collections.Concurrent;

namespace AnimalAdoption
{
    public class AnimalContext
    {
            private readonly ConcurrentDictionary<int, Animal> _animals = new();
            private int _nextId = 1;

            public virtual IEnumerable<Animal> Animals => _animals.Values;

            public virtual Animal? GetById(int id) => _animals.TryGetValue(id, out var animal) ? animal : null;

            public virtual void Add(Animal animal)
            {
                animal.Id = _nextId++;
                _animals[animal.Id] = animal;
            }

            public virtual void Update(Animal animal)
            {
                if (_animals.ContainsKey(animal.Id))
                {
                    _animals[animal.Id] = animal;
                }
            }

            public virtual void Delete(int id)
            {
                _animals.TryRemove(id, out _);
            }

            public virtual IEnumerable<Animal> GetFiltered(string? name, int? age, string? location)
            {
                var query = _animals.Values.AsQueryable();

                if (!string.IsNullOrEmpty(name))
                    query = query.Where(a => a.Name.Contains(name, StringComparison.OrdinalIgnoreCase));

                if (age.HasValue)
                    query = query.Where(a => a.Age == age.Value);

                if (!string.IsNullOrEmpty(location))
                    query = query.Where(a => a.Location.Contains(location, StringComparison.OrdinalIgnoreCase));

                return query.ToList();
            }

        public virtual List<Animal> GetSorted(string sortBy, bool ascending)
        {
            var query = _animals.Values.AsEnumerable();

            return (sortBy.ToLower() switch
            {
                "name" => ascending ? query.OrderBy(a => a.Name) : query.OrderByDescending(a => a.Name),
                "age" => ascending ? query.OrderBy(a => a.Age) : query.OrderByDescending(a => a.Age),
                "location" => ascending ? query.OrderBy(a => a.Location) : query.OrderByDescending(a => a.Location),
                _ => query
            }).ToList();
        }
    }
}
