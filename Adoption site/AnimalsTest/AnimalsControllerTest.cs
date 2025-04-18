using AnimalAdoption.Controllers;
using AnimalAdoption.Models;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Xunit;
using System.Collections.Generic;
using System.Linq;
using AnimalAdoption;

namespace AnimalsTest
{
	public class AnimalsControllerTests
	{
		private readonly Mock<AnimalContext> _mockContext;
		private readonly AnimalsController _controller;

		public AnimalsControllerTests()
		{
			_mockContext = new Mock<AnimalContext>();
			_controller = new AnimalsController(_mockContext.Object);
		}

		[Fact]
		public void GetAll_WithNoParameters_ReturnsAllAnimals()
		{
			var expectedAnimals = new List<Animal>
			{
				new Animal { Id = 1, Name = "Rex" },
				new Animal { Id = 2, Name = "Milo" }
			};

			_mockContext.SetupGet(x => x.Animals).Returns(expectedAnimals);

			var result = _controller.GetAll(null, null, null, null, null);

			var actionResult = Assert.IsType<ActionResult<IEnumerable<Animal>>>(result);
			var okResult = Assert.IsType<OkObjectResult>(actionResult.Result);
			okResult.Value.Should().BeEquivalentTo(expectedAnimals);
		}

		[Fact]
		public void GetAll_WithNameFilter_ReturnsFilteredAnimals()
		{
			var expectedAnimals = new List<Animal> { new Animal { Id = 1, Name = "Rex" } };
			_mockContext.Setup(x => x.GetFiltered("Rex", null, null)).Returns(expectedAnimals);

			var result = _controller.GetAll("Rex", null, null, null, null);

			var actionResult = Assert.IsType<ActionResult<IEnumerable<Animal>>>(result);
			var okResult = Assert.IsType<OkObjectResult>(actionResult.Result);
			okResult.Value.Should().BeEquivalentTo(expectedAnimals);
		}

		[Fact]
		public void GetAll_WithSorting_ReturnsSortedAnimals()
		{
			var expectedAnimals = new List<Animal>
			{
				new Animal { Id = 1, Name = "Alpha" },
				new Animal { Id = 2, Name = "Zoe" }
			};
			_mockContext.Setup(x => x.GetSorted("name", true)).Returns(expectedAnimals);

			var result = _controller.GetAll(null, null, null, "name", true);

			var actionResult = Assert.IsType<ActionResult<IEnumerable<Animal>>>(result);
			var okResult = Assert.IsType<OkObjectResult>(actionResult.Result);
			okResult.Value.Should().BeEquivalentTo(expectedAnimals);
		}

		[Fact]
		public void GetById_WithValidId_ReturnsAnimal()
		{
			var expectedAnimal = new Animal { Id = 1, Name = "Rex" };
			_mockContext.Setup(x => x.GetById(1)).Returns(expectedAnimal);

			var result = _controller.GetById(1);

			var actionResult = Assert.IsType<ActionResult<Animal>>(result);
			Assert.Equal(expectedAnimal, actionResult.Value);
		}

		[Fact]
		public void GetById_WithInvalidId_ReturnsNotFound()
		{
			_mockContext.Setup(x => x.GetById(It.IsAny<int>())).Returns((Animal)null);

			var result = _controller.GetById(999);

			var actionResult = Assert.IsType<ActionResult<Animal>>(result);
			Assert.IsType<NotFoundResult>(actionResult.Result);
		}

		[Fact]
		public void Create_ValidAnimal_ReturnsCreatedAtAction()
		{
			var newAnimal = new Animal { Name = "Rex" };

			var result = _controller.Create(newAnimal);

			var actionResult = Assert.IsType<ActionResult<Animal>>(result);
			var createdAtResult = Assert.IsType<CreatedAtActionResult>(actionResult.Result);

			Assert.Equal(nameof(AnimalsController.GetById), createdAtResult.ActionName);
			Assert.Equal(newAnimal, createdAtResult.Value);
			_mockContext.Verify(x => x.Add(newAnimal), Times.Once);
		}

		[Fact]
		public void Update_ValidRequest_ReturnsNoContent()
		{
			var existingAnimal = new Animal { Id = 1, Name = "Rex" };
			var updatedAnimal = new Animal { Id = 1, Name = "Rex Updated" };
			_mockContext.Setup(x => x.GetById(1)).Returns(existingAnimal);

			var result = _controller.Update(1, updatedAnimal);

			Assert.IsType<NoContentResult>(result);
			_mockContext.Verify(x => x.Update(updatedAnimal), Times.Once);
		}

		[Fact]
		public void Update_IdMismatch_ReturnsBadRequest()
		{
			// Arrange
			var animal = new Animal { Id = 2, Name = "Rex" };

			// Act
			var result = _controller.Update(1, animal);

			Assert.IsType<BadRequestResult>(result);
		}

		[Fact]
		public void Update_NonExistentAnimal_ReturnsNotFound()
		{
			var animal = new Animal { Id = 1, Name = "Rex" };
			_mockContext.Setup(x => x.GetById(1)).Returns((Animal)null);

			var result = _controller.Update(1, animal);

			Assert.IsType<NotFoundResult>(result);
		}

		[Fact]
		public void Delete_ValidId_ReturnsNoContent()
		{
			var animal = new Animal { Id = 1, Name = "Rex" };
			_mockContext.Setup(x => x.GetById(1)).Returns(animal);

			var result = _controller.Delete(1);

			Assert.IsType<NoContentResult>(result);
			_mockContext.Verify(x => x.Delete(1), Times.Once);
		}

		[Fact]
		public void Delete_NonExistentId_ReturnsNotFound()
		{
			_mockContext.Setup(x => x.GetById(1)).Returns((Animal)null);

			var result = _controller.Delete(1);

			Assert.IsType<NotFoundResult>(result);
		}
	}
}