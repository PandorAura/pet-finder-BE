using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using AnimalAdoption.Data;

namespace AnimalAdoption.Data
{
    public class AnimalAdoptionContextFactory : IDesignTimeDbContextFactory<AnimalAdoptionContext>
    {
        public AnimalAdoptionContext CreateDbContext(string[] args)
        {
            var optionsBuilder = new DbContextOptionsBuilder<AnimalAdoptionContext>();

            // Read connection string from environment variable
            var connStr = Environment.GetEnvironmentVariable("ConnectionStrings__DefaultConnection")
              ?? "Host=hopper.proxy.rlwy.net;Port=54158;Database=railway;Username=postgres;Password=zjsgtbVjlAQkSKanqKXGFGIJOuSrkJZI;SSL Mode=Require;Trust Server Certificate=true\r\n";


            if (string.IsNullOrEmpty(connStr))
            {
                throw new InvalidOperationException("Connection string not found in environment variables.");
            }

            optionsBuilder.UseNpgsql(connStr);

            return new AnimalAdoptionContext(optionsBuilder.Options);
        }
    }

}