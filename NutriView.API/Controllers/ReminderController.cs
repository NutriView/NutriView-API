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
    public class ReminderController : ControllerBase
    {
        private readonly IReminderService _service;

        public ReminderController(IReminderService service)
        {
            _service = service;
        }

        /// <summary>
        /// Get all reminders for the signed-in user
        /// </summary>
        [HttpGet("me")]
        public async Task<IActionResult> GetMine()
        {
            var reminders = await _service.GetAllByUserAsync(User.GetUserId());
            return Ok(reminders);
        }

        /// <summary>
        /// Get one of the signed-in user's reminders by id
        /// </summary>
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(Guid id)
        {
            var reminder = await _service.GetByIdAsync(User.GetUserId(), id);

            if (reminder == null)
                return NotFound();

            return Ok(reminder);
        }

        /// <summary>
        /// Create a new reminder for the signed-in user
        /// </summary>
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] ReminderCreateDTO dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            try
            {
                var created = await _service.CreateAsync(User.GetUserId(), dto);

                return CreatedAtAction(nameof(GetById), new { id = created.ReminderId }, created);
            }
            catch (ValidationException ex)
            {
                return BadRequest(ex.Message);
            }
        }

        /// <summary>
        /// Update one of the signed-in user's reminders
        /// </summary>
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(Guid id, [FromBody] ReminderUpdateDTO dto)
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
        /// Delete one of the signed-in user's reminders
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
