using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NutriView.API.Exceptions;
using NutriView.API.Helpers;
using NutriView.API.Models.DTOs;
using NutriView.API.Services;

namespace NutriView.API.Controllers
{
    [ApiController]
    [Authorize]
    [Route("api/[controller]")]
    public class FoodEntryController : ControllerBase
    {
        private readonly IFoodEntryService _service;

        public FoodEntryController(IFoodEntryService service)
        {
            _service = service;
        }

        /// <summary>
        /// Get all food entries for the signed-in user
        /// </summary>
        [HttpGet("me")]
        public async Task<IActionResult> GetMine()
        {
            var entries = await _service.GetAllByUserAsync(User.GetUserId());
            return Ok(entries);
        }

        /// <summary>
        /// Get one of the signed-in user's food entries by id
        /// </summary>
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(Guid id)
        {
            var entry = await _service.GetByIdAsync(User.GetUserId(), id);

            if (entry == null)
                return NotFound();

            return Ok(entry);
        }

        /// <summary>
        /// Create a new food entry for the signed-in user
        /// </summary>
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] FoodEntryCreateDTO dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            try
            {
                var created = await _service.CreateAsync(User.GetUserId(), dto);

                return CreatedAtAction(
                    nameof(GetById),
                    new { id = created.FoodEntryId },
                    created);
            }
            catch (ValidationException ex)
            {
                return BadRequest(ex.Message);
            }
        }

        /// <summary>
        /// Update one of the signed-in user's food entries
        /// </summary>
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(Guid id, [FromBody] FoodEntryUpdateDTO dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            try
            {
                var updated = await _service.UpdateAsync(User.GetUserId(), id, dto);

                if (!updated)
                    return NotFound();

                return NoContent();
            }
            catch (ValidationException ex)
            {
                return BadRequest(ex.Message);
            }
        }

        /// <summary>
        /// Delete one of the signed-in user's food entries
        /// </summary>
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            var deleted = await _service.DeleteAsync(User.GetUserId(), id);

            if (!deleted)
                return NotFound();

            return NoContent();
        }
    }
}
