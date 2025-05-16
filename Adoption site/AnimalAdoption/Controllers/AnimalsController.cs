using Microsoft.AspNetCore.Mvc;
using AnimalAdoption.Models;
using AnimalAdoption.Services;
using System.Linq.Expressions;

namespace AnimalAdoption.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AnimalsController : ControllerBase
    {
        private readonly IAnimalService _animalService;

        public AnimalsController(IAnimalService animalService)
        {
            _animalService = animalService;
        }

        // GET: api/Animals
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Animal>>> GetAnimals(
            [FromQuery] string? species,
            [FromQuery] bool? isAdopted,
            [FromQuery] string? sortBy,
            [FromQuery] string? sortOrder)
        {
            Expression<Func<Animal, bool>>? filter = null;
            Func<IQueryable<Animal>, IOrderedQueryable<Animal>>? orderBy = null;

            // Build filter
            if (!string.IsNullOrEmpty(species) || isAdopted.HasValue)
            {
                filter = a =>
                    (string.IsNullOrEmpty(species) || a.Species == species) &&
                    (!isAdopted.HasValue || a.IsAdopted == isAdopted.Value);
            }

            // Build sorting
            if (!string.IsNullOrEmpty(sortBy))
            {
                bool descending = !string.IsNullOrEmpty(sortOrder) && sortOrder.ToLower() == "desc";

                switch (sortBy.ToLower())
                {
                    case "name":
                        orderBy = q => descending ? q.OrderByDescending(a => a.Name) : q.OrderBy(a => a.Name);
                        break;
                    case "age":
                        orderBy = q => descending ? q.OrderByDescending(a => a.Age) : q.OrderBy(a => a.Age);
                        break;
                    case "arrivaldate":
                        orderBy = q => descending ? q.OrderByDescending(a => a.ArrivalDate) : q.OrderBy(a => a.ArrivalDate);
                        break;
                    default:
                        orderBy = q => descending ? q.OrderByDescending(a => a.Id) : q.OrderBy(a => a.Id);
                        break;
                }
            }

            var animals = await _animalService.GetFilteredAndSortedAnimalsAsync(
                filter: filter,
                orderBy: orderBy,
                includeProperties: "Adopter");

            return Ok(animals);
        }

        // GET: api/Animals/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Animal>> GetAnimal(int id)
        {
            var animal = await _animalService.GetAnimalByIdAsync(id);

            if (animal == null)
            {
                return NotFound();
            }

            return animal;
        }

        // PUT: api/Animals/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutAnimal(int id, Animal animal)
        {
            if (id != animal.Id)
            {
                return BadRequest();
            }

            try
            {
                await _animalService.UpdateAnimalAsync(animal);
            }
            catch (ArgumentException)
            {
                return NotFound();
            }

            return NoContent();
        }

        // POST: api/Animals
        [HttpPost]
        public async Task<ActionResult<Animal>> PostAnimal(Animal animal)
        {
            await _animalService.AddAnimalAsync(animal);
            return CreatedAtAction("GetAnimal", new { id = animal.Id }, animal);
        }

        // DELETE: api/Animals/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteAnimal(int id)
        {
            try
            {
                await _animalService.DeleteAnimalAsync(id);
            }
            catch (ArgumentException)
            {
                return NotFound();
            }

            return NoContent();
        }

        // POST: api/Animals/5/adopt/2
        [HttpPost("{animalId}/adopt/{adopterId}")]
        public async Task<IActionResult> AdoptAnimal(int animalId, int adopterId)
        {
            try
            {
                await _animalService.AdoptAnimalAsync(animalId, adopterId);
                return NoContent();
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
        }

        // POST: api/Animals/5/return
        [HttpPost("{id}/return")]
        public async Task<IActionResult> ReturnAnimal(int id)
        {
            try
            {
                await _animalService.ReturnAnimalAsync(id);
                return NoContent();
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
        }

        // GET: api/Animals/search?term=buddy
        [HttpGet("search")]
        public async Task<ActionResult<IEnumerable<Animal>>> SearchAnimals([FromQuery] string term)
        {
            var animals = await _animalService.SearchAnimalsAsync(term);
            return Ok(animals);
        }

        // GET: api/Animals/available
        [HttpGet("available")]
        public async Task<ActionResult<IEnumerable<Animal>>> GetAvailableAnimals()
        {
            var animals = await _animalService.GetAvailableAnimalsAsync();
            return Ok(animals);
        }

        // GET: api/Animals/adopted
        [HttpGet("adopted")]
        public async Task<ActionResult<IEnumerable<Animal>>> GetAdoptedAnimals()
        {
            var animals = await _animalService.GetAdoptedAnimalsAsync();
            return Ok(animals);
        }
    }
}