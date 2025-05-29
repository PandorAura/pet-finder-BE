using AnimalAdoption.Data;
using AnimalAdoption.Models;
using Bogus;
using Dapper;
using Microsoft.Data.SqlClient;
using System.Security.Cryptography;
using System.Text;

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

        public async Task SeedDataAsync(int userCount = 50, int animalCount = 50, int adoptionCount = 20)
        {
            await SeedUsersAsync(userCount);
            await SeedAnimalsAsync(animalCount);
            await SeedAdoptionsAsync(adoptionCount, userCount, animalCount);
            await UpdateAnimalAdoptionFlagsAsync();
        }

        private async Task SeedUsersAsync(int count)
        {
            var userFaker = new Faker<User>()
                .RuleFor(u => u.FirstName, f => f.Name.FirstName())
                .RuleFor(u => u.LastName, f => f.Name.LastName())
                .RuleFor(u => u.Email, (f, u) => f.Internet.Email(u.FirstName, u.LastName))
                .RuleFor(u => u.PhoneNumber, f => f.Phone.PhoneNumber())
                .RuleFor(u => u.Address, f => f.Address.FullAddress())
                .RuleFor(u => u.RegistrationDate, f => f.Date.Past(3))
                .RuleFor(u => u.Username, f => f.Internet.UserName())
                .RuleFor(u => u.Role, _ => UserRole.User)
                .RuleFor(u => u.PasswordSalt, _ => GenerateSalt())
                .RuleFor(u => u.PasswordHash, (f, u) => HashPassword("Password123", u.PasswordSalt));

            using var connection = new SqlConnection(_connectionString);
            await connection.OpenAsync();

            const int batchSize = 1000;
            for (int i = 0; i < count; i += batchSize)
            {
                var users = userFaker.Generate(Math.Min(batchSize, count - i));
                await connection.ExecuteAsync(
                    @"INSERT INTO [Users] (FirstName, LastName, Email, PhoneNumber, Address, RegistrationDate, 
                        Username, PasswordHash, PasswordSalt, Role) 
                      VALUES (@FirstName, @LastName, @Email, @PhoneNumber, @Address, @RegistrationDate, 
                        @Username, @PasswordHash, @PasswordSalt, @Role)",
                    users);

                Console.WriteLine($"Inserted {Math.Min(i + batchSize, count)} users...");
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
                .RuleFor(a => a.Species, f => f.PickRandom(species))
                .RuleFor(a => a.Breed, (f, a) => a.Species == "Dog"
                    ? f.PickRandom(dogBreeds)
                    : f.PickRandom(catBreeds))
                .RuleFor(a => a.ArrivalDate, f => f.Date.Past(2))
                .RuleFor(a => a.IsAdopted, _ => false) // Initially false; updated later
                .RuleFor(a => a.Photo, f => $"https://picsum.photos/200/200?random={f.Random.Int(1, 10000)}");

            using var connection = new SqlConnection(_connectionString);
            await connection.OpenAsync();

            const int batchSize = 1000;
            for (int i = 0; i < count; i += batchSize)
            {
                var animals = animalFaker.Generate(Math.Min(batchSize, count - i));
                await connection.ExecuteAsync(
                    @"INSERT INTO Animals (Name, Age, Description, Location, IsAdopted, Breed, Species, 
                        ArrivalDate, Photo) 
                      VALUES (@Name, @Age, @Description, @Location, @IsAdopted, @Breed, @Species, 
                        @ArrivalDate, @Photo)",
                    animals);

                Console.WriteLine($"Inserted {Math.Min(i + batchSize, count)} animals...");
            }
        }

        private async Task SeedAdoptionsAsync(int count, int userCount, int animalCount)
        {
            var faker = new Faker();

            using var connection = new SqlConnection(_connectionString);
            await connection.OpenAsync();

            const int batchSize = 1000;
            for (int i = 0; i < count; i += batchSize)
            {
                var batchSizeActual = Math.Min(batchSize, count - i);
                var adoptions = new List<Adoption>(batchSizeActual);

                for (int j = 0; j < batchSizeActual; j++)
                {
                    var userId = faker.Random.Int(1, userCount);
                    var animalId = faker.Random.Int(1, animalCount);

                    // Simple check to avoid duplicates or you can handle this in DB with constraints
                    adoptions.Add(new Adoption
                    {
                        UserId = userId,
                        AnimalId = animalId,
                        AdoptionDate = faker.Date.Past(1)
                    });
                }

                await connection.ExecuteAsync(
                    @"INSERT INTO Adoptions (UserId, AnimalId, AdoptionDate) 
                      VALUES (@UserId, @AnimalId, @AdoptionDate)",
                    adoptions);

                Console.WriteLine($"Inserted {i + batchSizeActual} adoptions...");
            }
        }

        private async Task UpdateAnimalAdoptionFlagsAsync()
        {
            // Update Animals.IsAdopted = 1 for animals that have an adoption record
            using var connection = new SqlConnection(_connectionString);
            await connection.OpenAsync();

            var rowsUpdated = await connection.ExecuteAsync(
                @"UPDATE Animals
                  SET IsAdopted = 1
                  WHERE Id IN (SELECT DISTINCT AnimalId FROM Adoptions)");

            Console.WriteLine($"Updated IsAdopted flag for {rowsUpdated} animals.");
        }

        private static byte[] GenerateSalt()
        {
            var salt = new byte[16];
            using var rng = RandomNumberGenerator.Create();
            rng.GetBytes(salt);
            return salt;
        }

        private static byte[] HashPassword(string password, byte[] salt)
        {
            using var hmac = new HMACSHA512(salt);
            return hmac.ComputeHash(Encoding.UTF8.GetBytes(password));
        }
    }
}
