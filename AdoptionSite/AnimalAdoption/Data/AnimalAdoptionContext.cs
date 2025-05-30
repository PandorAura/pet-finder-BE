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
        public DbSet<Adoption> Adoptions { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.LogTo(Console.WriteLine, Microsoft.Extensions.Logging.LogLevel.Debug);
            base.OnConfiguring(optionsBuilder);
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Adoption entity configuration
            modelBuilder.Entity<Adoption>(entity =>
            {
                entity.HasKey(a => a.Id);

                entity.Property(a => a.Id)
                      .ValueGeneratedOnAdd();

                entity.HasOne(a => a.User)
                      .WithMany(u => u.Adoptions)
                      .HasForeignKey(a => a.UserId)
                      .IsRequired()
                      .OnDelete(DeleteBehavior.Cascade);

                entity.HasOne(a => a.Animal)
                      .WithMany(an => an.Adoptions)
                      .HasForeignKey(a => a.AnimalId)
                      .IsRequired()
                      .OnDelete(DeleteBehavior.Cascade);

                entity.Property(a => a.AdoptionDate)
                      .IsRequired();
            });

            // User entity configuration
            modelBuilder.Entity<User>(entity =>
            {
                entity.Property(u => u.Email)
                      .HasColumnType("varchar(255)")
                      .IsRequired();

                entity.Property(u => u.Username)
                      .HasColumnType("varchar(100)")
                      .IsRequired();

                entity.Property(u => u.PasswordHash)
                      .HasColumnType("bytea")
                      .IsRequired();

                entity.Property(u => u.PasswordSalt)
                      .HasColumnType("bytea")
                      .IsRequired();

                entity.Property(u => u.FirstName)
                      .HasColumnType("varchar(100)")
                      .IsRequired();

                entity.Property(u => u.LastName)
                      .HasColumnType("varchar(100)")
                      .IsRequired();

                entity.Property(u => u.PhoneNumber)
                      .HasColumnType("varchar(50)")
                      .IsRequired();

                entity.Property(u => u.Address)
                      .HasColumnType("varchar(255)")
                      .IsRequired();

                entity.Property(u => u.RegistrationDate)
                      .IsRequired();

                entity.Property(u => u.Role)
                      .HasConversion<int>()  // Store enum as int
                      .IsRequired();
            });

            // Animal entity configuration
            modelBuilder.Entity<Animal>(entity =>
            {
                entity.Property(a => a.Photo)
                      .HasColumnType("varchar(255)")
                      .IsRequired();

                entity.Property(a => a.Name)
                      .HasColumnType("varchar(100)")
                      .IsRequired();

                entity.Property(a => a.Description)
                      .HasColumnType("text")
                      .IsRequired();

                entity.Property(a => a.Location)
                      .HasColumnType("varchar(100)")
                      .IsRequired();

                entity.Property(a => a.Breed)
                      .HasColumnType("varchar(100)")
                      .IsRequired();

                entity.Property(a => a.Species)
                      .HasColumnType("varchar(100)")
                      .IsRequired();

                entity.Property(a => a.Age)
                      .IsRequired();

                entity.Property(a => a.IsAdopted)
                      .IsRequired();

                entity.Property(a => a.ArrivalDate)
                      .IsRequired();
            });
        }
    }
}
