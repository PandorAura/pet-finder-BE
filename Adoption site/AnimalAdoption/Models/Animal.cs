namespace AnimalAdoption.Models
{
    public class Animal
    {
        public int Id { get; set; }
        public string Photo { get; set; } = null!;
        public string Name { get; set; } = null!;
        public int Age { get; set; }
        public string Description { get; set; } = null!;
        public string Location { get; set; } = null!;
        public bool IsAdopted { get; set; }
        public string Breed { get; set; } = null!;
        public string Species { get; set; } = null!;
        public DateTime ArrivalDate { get; set; }

        // Navigation property for adoptions
        public ICollection<Adoption> Adoptions { get; set; } = new List<Adoption>();
    }
}
