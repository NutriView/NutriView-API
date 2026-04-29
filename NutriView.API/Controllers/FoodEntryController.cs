using Microsoft.AspNetCore.Mvc;
using NutriView.API.Models.DTOs;
using NutriView.API.Services;

namespace NutriView.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class FoodEntryController : ControllerBase
    {
        private readonly IFoodEntryService _service;

        public FoodEntryController(IFoodEntryService service)
        {
            _service = service;
        }

        /// <summary>
        /// Get all food entries for a specific user
        /// </summary>
        [HttpGet("user/{userId}")]
        public async Task<IActionResult> GetByUser(Guid userId)
        {
            var entries = await _service.GetAllByUserAsync(userId);
            return Ok(entries);
        }

        /// <summary>
        /// Get a specific food entry by id
        /// </summary>
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(Guid id)
        {
            var entry = await _service.GetByIdAsync(id);

            if (entry == null)
                return NotFound();

            return Ok(entry);
        }

        /// <summary>
        /// Create a new food entry
        /// </summary>
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] FoodEntryCreateDTO dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var created = await _service.CreateAsync(dto);

            return CreatedAtAction(
                nameof(GetById),
                new { id = created.FoodEntryId },
                created);
        }

        /// <summary>
        /// Update an existing food entry
        /// </summary>
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(Guid id, [FromBody] FoodEntryUpdateDTO dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var updated = await _service.UpdateAsync(id, dto);

            if (!updated)
                return NotFound();

            return NoContent();
        }

        /// <summary>
        /// Delete a food entry
        /// </summary>
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            var deleted = await _service.DeleteAsync(id);

            if (!deleted)
                return NotFound();

            return NoContent();
        }
    }
}