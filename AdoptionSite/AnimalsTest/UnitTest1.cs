using AnimalAdoption;
using AnimalAdoption.Models;
using FluentAssertions;
using Xunit;


namespace AnimalsTest;
    public class UnitTest1
    {


            private readonly AnimalContext _context;

            public UnitTest1()
            {
                _context = new AnimalContext(); 
            }

            [Fact]
            public void Add_ShouldIncreaseAnimalCount()
            {
                var animal = new Animal { Name = "Max", Age = 2 };

                _context.Add(animal);

                _context.Animals.Should().HaveCount(1);
                _context.Animals.Should().Contain(animal);
            }

            [Fact]
            public void GetById_ShouldReturnCorrectAnimal()
            {
                var animal = new Animal { Name = "Bella", Age = 3 };
                _context.Add(animal);
                int id = animal.Id;

                var result = _context.GetById(id);

                result.Should().BeEquivalentTo(animal);
            }

            [Fact]
            public void GetById_ShouldReturnNullForInvalidId()
            {
                var result = _context.GetById(999); // Non-existent ID

                result.Should().BeNull();
            }

            [Fact]
            public void Update_ShouldModifyExistingAnimal()
            {
                var animal = new Animal { Name = "Charlie", Age = 4 };
                _context.Add(animal);
                animal.Name = "Updated Charlie";

                _context.Update(animal);

                var updatedAnimal = _context.GetById(animal.Id);
                updatedAnimal.Name.Should().Be("Updated Charlie");
            }

            [Fact]
            public void Delete_ShouldRemoveAnimal()
            {
                var animal = new Animal { Name = "Lucy", Age = 5 };
                _context.Add(animal);
                int id = animal.Id;

                _context.Delete(id);

                _context.Animals.Should().BeEmpty();
            }

            [Fact]
            public void GetFiltered_ShouldFilterByName()
            {
                _context.Add(new Animal { Name = "Rex", Age = 2 });
                _context.Add(new Animal { Name = "Milo", Age = 3 });

                var filtered = _context.GetFiltered(name: "Rex", age: null, location: null);

                filtered.Should().HaveCount(1);
                filtered.First().Name.Should().Be("Rex");
            }

            [Fact]
            public void GetSorted_ShouldSortByNameAscending()
            {
                _context.Add(new Animal { Name = "Zoe", Age = 2 });
                _context.Add(new Animal { Name = "Alpha", Age = 1 });

                var sorted = _context.GetSorted(sortBy: "name", ascending: true);

                sorted.Select(a => a.Name).Should().BeInAscendingOrder();
            }

	[Fact]
	public void GetFiltered_ShouldFilterByAge()
	{
		_context.Add(new Animal { Name = "Rex", Age = 2 });
		_context.Add(new Animal { Name = "Milo", Age = 3 });

		var filtered = _context.GetFiltered(name: null, age: 2, location: null);

		filtered.Should().HaveCount(1);
		filtered.First().Age.Should().Be(2);
	}

	[Fact]
	public void GetFiltered_ShouldFilterByLocation()
	{
		_context.Add(new Animal { Name = "Rex", Age = 2, Location = "New York" });
		_context.Add(new Animal { Name = "Milo", Age = 3, Location = "Chicago" });

		var filtered = _context.GetFiltered(name: null, age: null, location: "New York");

		filtered.Should().HaveCount(1);
		filtered.First().Location.Should().Be("New York");
	}

	[Fact]
	public void GetSorted_ShouldSortByAgeAscending()
	{
		_context.Add(new Animal { Name = "Zoe", Age = 2 });
		_context.Add(new Animal { Name = "Alpha", Age = 1 });

		var sorted = _context.GetSorted(sortBy: "age", ascending: true);

		sorted.Select(a => a.Age).Should().BeInAscendingOrder();
	}

	[Fact]
	public void GetSorted_ShouldSortByLocationDescending()
	{
		_context.Add(new Animal { Name = "A", Location = "Chicago" });
		_context.Add(new Animal { Name = "B", Location = "New York" });

		var sorted = _context.GetSorted(sortBy: "location", ascending: false);

		sorted.Select(a => a.Location).Should().BeInDescendingOrder();
	}

	[Fact]
	public void GetSorted_ShouldReturnDefaultOrderForInvalidSortBy()
	{
		_context.Add(new Animal { Name = "Zoe", Age = 2 });
		_context.Add(new Animal { Name = "Alpha", Age = 1 });

		var sorted = _context.GetSorted(sortBy: "invalid", ascending: true);

		sorted.Should().HaveCount(2);
	}

	[Fact]
	public void GetSorted_ShouldSortByNameDescending()
	{
		_context.Add(new Animal { Name = "Alpha", Age = 1 });
		_context.Add(new Animal { Name = "Zoe", Age = 2 });

		var sorted = _context.GetSorted(sortBy: "name", ascending: false);

		sorted.Select(a => a.Name).Should().BeInDescendingOrder();
	}

	[Fact]
	public void GetSorted_ShouldSortByAgeDescending()
	{
		_context.Add(new Animal { Name = "A", Age = 1 });
		_context.Add(new Animal { Name = "B", Age = 2 });

		var sorted = _context.GetSorted(sortBy: "age", ascending: false);

		sorted.Select(a => a.Age).Should().BeInDescendingOrder();
	}

	[Fact]
	public void GetSorted_ShouldSortByLocationAscending()
	{
		_context.Add(new Animal { Name = "A", Location = "Chicago" });
		_context.Add(new Animal { Name = "B", Location = "New York" });

		var sorted = _context.GetSorted(sortBy: "location", ascending: true);

		sorted.Select(a => a.Location).Should().BeInAscendingOrder();
	}

	[Fact]
	public void GetSorted_ShouldHandleEmptyCollection()
	{
		var sorted = _context.GetSorted(sortBy: "name", ascending: true);

		sorted.Should().BeEmpty();
	}

	[Fact]
	public void GetSorted_ShouldHandleNullPropertiesWhenSorting()
	{
		_context.Add(new Animal { Name = null, Age = 2 });
		_context.Add(new Animal { Name = "Zoe", Age = 1 });

		var sorted = _context.GetSorted(sortBy: "name", ascending: true);

		sorted.First().Name.Should().BeNull();
		sorted.Last().Name.Should().Be("Zoe");
	}

	[Fact]
	public void GetSorted_ShouldHandleCaseInsensitiveSortByParameter()
	{
		_context.Add(new Animal { Name = "Alpha", Age = 1 });
		_context.Add(new Animal { Name = "Zoe", Age = 2 });

		var sorted = _context.GetSorted(sortBy: "NAME", ascending: true);

		sorted.Select(a => a.Name).Should().BeInAscendingOrder();
	}

	[Fact]
	public void GetSorted_ShouldMaintainOriginalOrderForDefaultCase()
	{
		var animal1 = new Animal { Name = "Zoe", Age = 2 };
		var animal2 = new Animal { Name = "Alpha", Age = 1 };
		_context.Add(animal1);
		_context.Add(animal2);

		var sorted = _context.GetSorted(sortBy: "invalid", ascending: true);

		sorted.Should().ContainInOrder(animal1, animal2);
	}

	[Fact]
	public void GetSorted_ShouldHandleDuplicateValues()
	{
		_context.Add(new Animal { Name = "Alpha", Age = 1 });
		_context.Add(new Animal { Name = "Alpha", Age = 2 });
		_context.Add(new Animal { Name = "Beta", Age = 1 });

		var sorted = _context.GetSorted(sortBy: "name", ascending: true);

		sorted.Select(a => a.Name).Should().BeInAscendingOrder();
		sorted.Where(a => a.Name == "Alpha").Should().HaveCount(2);
	}
}
