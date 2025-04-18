using AnimalAdoption.Models;
using Microsoft.AspNetCore.Mvc;

namespace AnimalAdoption.Controllers
{
        [ApiController]
        [Route("api/[controller]")]
        public class AnimalsController : ControllerBase
        {
            private readonly AnimalContext _context;

            public AnimalsController(AnimalContext context)
            {
                _context = context;
            }

            [HttpGet]
            public ActionResult<IEnumerable<Animal>> GetAll(
                [FromQuery] string? name,
                [FromQuery] int? age,
                [FromQuery] string? location,
                [FromQuery] string? sortBy,
                [FromQuery] bool? ascending)
            {
                if (!string.IsNullOrEmpty(name) || age.HasValue || !string.IsNullOrEmpty(location))
                {
                    return Ok(_context.GetFiltered(name, age, location));
                }

                if (!string.IsNullOrEmpty(sortBy))
                {
                    return Ok(_context.GetSorted(sortBy, ascending ?? true));
                }

                return Ok(_context.Animals);
            }

            [HttpGet("{id}")]
            public ActionResult<Animal> GetById(int id)
            {
                var animal = _context.GetById(id);
                if (animal == null) return NotFound();
                return animal;
            }

        [HttpPost]
        public ActionResult<Animal> Create(Animal animal)
        {
            if (animal.Age < 0)
            {
                return BadRequest("Age cannot be negative");
            }

            if (string.IsNullOrEmpty(animal.Name))
            {
                return BadRequest("Name is required");
            }

            if (string.IsNullOrEmpty(animal.Description))
            {
                return BadRequest("Description is required");
            }

            _context.Add(animal);

            return CreatedAtAction(nameof(GetById), new { id = animal.Id }, animal);
        }

        [HttpPut("{id}")]
            public IActionResult Update(int id, Animal animal)
            {
                if (id != animal.Id) return BadRequest();

                var existing = _context.GetById(id);
                if (existing == null) return NotFound();

                _context.Update(animal);
                return NoContent();
            }

            [HttpDelete("{id}")]
            public IActionResult Delete(int id)
            {
                var existing = _context.GetById(id);
                if (existing == null) return NotFound();

                _context.Delete(id);
                return NoContent();
            }
        }
    }
