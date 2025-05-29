using System.ComponentModel.DataAnnotations;

namespace AnimalAdoption.Models.DTOs
{
    public class UserSimpleDto
    {
        public int Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
    }

    public class UserDto
    {
        public int Id { get; set; }
        public string FirstName { get; set; } = null!;
        public string LastName { get; set; } = null!;
        public string Email { get; set; } = null!;
        public string PhoneNumber { get; set; } = null!;
        public string Address { get; set; } = null!;
        public DateTime RegistrationDate { get; set; }

        // Initialize to empty collection to avoid null refs
        public List<AdoptionDto> Adoptions { get; set; } = new List<AdoptionDto>();
    }


    public class UserResponseDto
    {
        public int Id { get; set; }
        public string Username { get; set; }
        public string Email { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public UserRole Role { get; set; } // Use enum here
    }

    public class UserCreateDto
    {
        [Required]
        public string Username { get; set; }

        [Required]
        [EmailAddress]
        public string Email { get; set; }

        [Required]
        public string FirstName { get; set; }

        [Required]
        public string LastName { get; set; }

        public string PhoneNumber { get; set; }
        public string Address { get; set; }
        public UserRole Role { get; set; } = UserRole.User; // Default role
    }
}
