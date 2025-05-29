namespace AnimalAdoption.Controllers
{
    using global::AnimalAdoption.Models.DTOs;
    using global::AnimalAdoption.Services;
    using Microsoft.AspNetCore.Mvc;

    namespace AnimalAdoption.Controllers
    {
        [ApiController]
        [Route("api/[controller]")]
        public class AdoptionController : ControllerBase
        {
            private readonly IAdoptionService _adoptionService;

            public AdoptionController(IAdoptionService adoptionService)
            {
                _adoptionService = adoptionService;
            }

            // GET api/adoption/user/5
            [HttpGet("user/{userId}")]
            public async Task<ActionResult<IEnumerable<AdoptionDto>>> GetAdoptionsByUser(int userId)
            {
                var adoptions = await _adoptionService.GetAdoptionsByUserAsync(userId);
                return Ok(adoptions);
            }

            // POST api/adoption
            [HttpPost]
            public async Task<ActionResult<AdoptionDto>> AdoptAnimal([FromBody] AdoptionCreateDto adoptionCreateDto)
            {
                if (!ModelState.IsValid)
                    return BadRequest(ModelState);

                try
                {
                    var adoption = await _adoptionService.AddAdoptionAsync(adoptionCreateDto.AnimalId, adoptionCreateDto.UserId);
                    return CreatedAtAction(nameof(GetAdoptionsByUser), new { userId = adoption.UserId }, adoption);
                }
                catch (ArgumentException ex)
                {
                    return NotFound(ex.Message);
                }
                catch (InvalidOperationException ex)
                {
                    return BadRequest(ex.Message);
                }
            }

            // DELETE api/adoption/5
            [HttpDelete("{id}")]
            public async Task<IActionResult> ReturnAnimal(int id)
            {
                try
                {
                    await _adoptionService.DeleteAdoptionAsync(id);
                    return NoContent();
                }
                catch (ArgumentException ex)
                {
                    return NotFound(ex.Message);
                }
            }
        }
    }

}
