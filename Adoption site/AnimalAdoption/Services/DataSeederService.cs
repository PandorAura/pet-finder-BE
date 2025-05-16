using AnimalAdoption.Data;
using AnimalAdoption.Models;
using Bogus;
using Dapper;
using Microsoft.Data.SqlClient;

namespace AnimalAdoption.Services
{
    public class DataSeederService
    {
            private readonly AnimalAdoptionContext _context;
            private readonly string _connectionString;

            public DataSeederService(AnimalAdoptionContext context, IConfiguration config)
            {
                _context = context;
                _connectionString = config.GetConnectionString("DefaultConnection");
            }

            public async Task SeedDataAsync(int adoptersCount = 100000, int animalsCount = 150000)
            {
                await SeedAdoptersAsync(adoptersCount);
                await SeedAnimalsAsync(animalsCount);
            }

            private async Task SeedAdoptersAsync(int count)
            {
                var adopterFaker = new Faker<User>()
                    .RuleFor(a => a.FirstName, f => f.Name.FirstName())
                    .RuleFor(a => a.LastName, f => f.Name.LastName())
                    .RuleFor(a => a.Email, (f, a) => f.Internet.Email(a.FirstName, a.LastName))
                    .RuleFor(a => a.PhoneNumber, f => f.Phone.PhoneNumber())
                    .RuleFor(a => a.Address, f => f.Address.FullAddress())
                    .RuleFor(a => a.RegistrationDate, f => f.Date.Past(3));

                using var connection = new SqlConnection(_connectionString);
                await connection.OpenAsync();

                var batchSize = 1000;
                for (int i = 0; i < count; i += batchSize)
                {
                    var adopters = adopterFaker.Generate(batchSize);
                    await connection.ExecuteAsync(
                        @"INSERT INTO Adopters (FirstName, LastName, Email, Phone, Address, RegistrationDate) 
                  VALUES (@FirstName, @LastName, @Email, @Phone, @Address, @RegistrationDate)",
                        adopters);

                    Console.WriteLine($"Inserted {Math.Min(i + batchSize, count)} adopters...");
                }
            }

            private async Task SeedAnimalsAsync(int count)
            {
                var species = new[] { "Dog", "Cat", "Rabbit", "Bird", "Hamster" };
                var dogBreeds = new[] { "Labrador", "Golden Retriever", "German Shepherd", "Bulldog", "Beagle" };
                var catBreeds = new[] { "Siamese", "Persian", "Maine Coon", "Bengal", "Sphynx" };

                var animalFaker = new Faker<Animal>()
                    .RuleFor(a => a.Name, f => f.Name.FirstName())
                    .RuleFor(a => a.Age, f => f.Random.Int(0, 15))
                    .RuleFor(a => a.Description, f => f.Lorem.Paragraph())
                    .RuleFor(a => a.Location, f => f.Address.City())
                    .RuleFor(a => a.IsAdopted, f => f.Random.Bool(0.3f)) // 30% chance of being adopted
                    .RuleFor(a => a.Breed, (f, a) => a.Species == "Dog"
                        ? f.PickRandom(dogBreeds)
                        : f.PickRandom(catBreeds))
                    .RuleFor(a => a.Species, f => f.PickRandom(species))
                    .RuleFor(a => a.ArrivalDate, f => f.Date.Past(2))
                    .RuleFor(a => a.Photo, f => $"https://picsum.photos/200/200?random={f.Random.Int(1, 1000)}")
                    .RuleFor(a => a.AdopterId, (f, a) => a.IsAdopted
                        ? f.Random.Int(1, 100000)
                        : (int?)null);

                using var connection = new SqlConnection(_connectionString);
                await connection.OpenAsync();

                var batchSize = 1000;
                for (int i = 0; i < count; i += batchSize)
                {
                    var animals = animalFaker.Generate(batchSize);
                    await connection.ExecuteAsync(
                        @"INSERT INTO Animals (Name, Age, Description, Location, IsAdopted, Breed, Species, 
                  ArrivalDate, Photo, AdopterId) 
                  VALUES (@Name, @Age, @Description, @Location, @IsAdopted, @Breed, @Species, 
                  @ArrivalDate, @Photo, @AdopterId)",
                        animals);

                    Console.WriteLine($"Inserted {Math.Min(i + batchSize, count)} animals...");
                }
            }
        
    }
}
