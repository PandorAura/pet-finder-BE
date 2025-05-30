using AnimalAdoption.Models;
using System.Text.Json.Serialization;
namespace AnimalAdoption.Models
{
    public class User
    {
        public int Id { get; set; }

        public string FirstName { get; set; } = null!;

        public string LastName { get; set; } = null!;

        public string Email { get; set; } = null!;

        public string PhoneNumber { get; set; } = null!;

        public string Address { get; set; } = null!;

        public DateTime RegistrationDate { get; set; }

        // Authentication fields
        public string Username { get; set; } = null!;

        public byte[]? PasswordHash { get; set; } = null!;

        public byte[]? PasswordSalt { get; set; } = null!;

        public UserRole Role { get; set; } = UserRole.User; // Default role
        public ICollection<Adoption> Adoptions { get; set; } = new List<Adoption>();

    }
}