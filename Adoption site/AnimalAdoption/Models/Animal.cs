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
        }
    }
