namespace AnimalAdoption.Models.DTOs
{
    public class AnimalDto
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
    }

    public class AnimalSimpleDto
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Species { get; set; }
        public string Photo { get; set; }
        public int Age { get; set; }
        public bool IsAdopted { get; set; }
    }

    public class AnimalCreateDto
    {
        public string Photo { get; set; }
        public string Name { get; set; }
        public int Age { get; set; }
        public string Description { get; set; }
        public string Location { get; set; }
        public string Breed { get; set; }
        public string Species { get; set; }
    }

    public class AnimalUpdateDto
    {
        public string? Photo { get; set; }
        public string? Name { get; set; }
        public int? Age { get; set; }
        public string? Description { get; set; }
        public string? Location { get; set; }
        public string? Breed { get; set; }
        public string? Species { get; set; }
    }

    public class AdoptionStatsDto
    {
        public string Species { get; set; } = null!;
        public int Year { get; set; }
        public int Month { get; set; }
        public int TotalAnimals { get; set; }
        public int AdoptedCount { get; set; }
        public decimal AdoptionRate { get; set; }
        public double AvgDaysInShelter { get; set; }
    }
}