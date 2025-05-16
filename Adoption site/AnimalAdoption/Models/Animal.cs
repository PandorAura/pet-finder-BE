namespace AnimalAdoption.Models
{
    public class Animal
    {
        public int Id { get; set; }
        public string Photo { get; set; }
        public string Name { get; set; }
        public int Age { get; set; }
        public string Description { get; set; }
        public string Location { get; set; }
        public bool IsAdopted { get; set; }
        public string Breed { get; set; }
        public string Species { get; set; }
        public DateTime ArrivalDate { get; set; }

        // Navigation property for the adopter
        public int? AdopterId { get; set; }
        public User? Adopter { get; set; }

    }
}
