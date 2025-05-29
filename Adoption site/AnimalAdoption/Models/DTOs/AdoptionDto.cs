namespace AnimalAdoption.Models.DTOs
{
    public class AdoptionCreateDto
    {
        public int AnimalId { get; set; }
        public int UserId { get; set; }
    }

    public class AdoptionDto
    {
        public int Id { get; set; }
        public int AnimalId { get; set; }
        public int UserId { get; set; }
        public DateTime AdoptionDate { get; set; }

        // Optionally include more info
        public AnimalDto Animal { get; set; }
        public UserDto User { get; set; }
    }


}