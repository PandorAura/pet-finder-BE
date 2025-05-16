using AnimalAdoption.Models;
using Microsoft.EntityFrameworkCore;

namespace AnimalAdoption.Data
{
    public class AnimalAdoptionContext : DbContext
    {

            public AnimalAdoptionContext(DbContextOptions<AnimalAdoptionContext> options)
                : base(options)
            {
            }

            public DbSet<Animal> Animals { get; set; }
            public DbSet<User> Users { get; set; }

            protected override void OnModelCreating(ModelBuilder modelBuilder)
            {
                // Configure the one-to-many relationship
                modelBuilder.Entity<Animal>()
                    .HasOne(a => a.Adopter)
                    .WithMany(ad => ad.AdoptedAnimals)
                    .HasForeignKey(a => a.AdopterId)
                    .OnDelete(DeleteBehavior.SetNull); // If adopter is deleted, set Animal.AdopterId to null

                // Seed some initial data

            modelBuilder.Entity<User>().HasData(
new User
{
    Id = 1,
    FirstName = "John",
    LastName = "Doe",
    Email = "john@example.com",
    PhoneNumber = "1234567890",
    Address = "123 Main St",
    RegistrationDate = new DateTime(2025, 4, 15) // fixed static date
},
new User
{
    Id = 2,
    FirstName = "Jane",
    LastName = "Smith",
    Email = "jane@example.com",
    PhoneNumber = "0987654321",
    Address = "456 Oak Ave",
    RegistrationDate = new DateTime(2025, 4, 30) // fixed static date
}
);

            modelBuilder.Entity<Animal>().HasData(
                new Animal
                {
                    Id = 1,
                    Name = "Buddy",
                    Age = 3,
                    Description = "Friendly golden retriever",
                    Location = "New York",
                    IsAdopted = true,
                    AdopterId = 1,
                    Breed = "Golden Retriever",
                    Species = "Dog",
                    ArrivalDate = new DateTime(2025, 2, 15), // fixed static date
                    Photo = "buddy.jpg"
                },
                new Animal
                {
                    Id = 2,
                    Name = "Mittens",
                    Age = 2,
                    Description = "Playful tabby cat",
                    Location = "Chicago",
                    IsAdopted = false,
                    Breed = "Tabby",
                    Species = "Cat",
                    ArrivalDate = new DateTime(2025, 3, 15), // fixed static date
                    Photo = "mittens.jpg"
                },
                new Animal
                {
                    Id = 3,
                    Name = "Rocky",
                    Age = 5,
                    Description = "Calm and gentle",
                    Location = "Los Angeles",
                    IsAdopted = true,
                    AdopterId = 2,
                    Breed = "Labrador",
                    Species = "Dog",
                    ArrivalDate = new DateTime(2025, 1, 15), // fixed static date
                    Photo = "rocky.jpg"
                }
            );

        }
    }
}
