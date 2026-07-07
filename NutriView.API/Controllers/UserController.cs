using Microsoft.AspNetCore.Mvc;
using NutriView.API.Exceptions;
using NutriView.API.Models.DTOs;
using NutriView.API.Services;

namespace NutriView.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UserController : ControllerBase
    {
        private readonly IUserService _service;

        public UserController(IUserService service)
        {
            _service = service;
        }

        /// <summary>
        /// Get all users
        /// </summary>
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var users = await _service.GetAllAsync();
            return Ok(users);
        }

        /// <summary>
        /// Get user by id
        /// </summary>
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(Guid id)
        {
            var user = await _service.GetByIdAsync(id);

            if (user == null)
                return NotFound();

            return Ok(user);
        }

        /// <summary>
        /// Create new user (register)
        /// </summary>
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] UserCreateDTO dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            try
            {
                var created = await _service.CreateAsync(dto);

                return CreatedAtAction(nameof(GetById), new { id = created.UserId }, created);
            }
            catch (ValidationException ex)
            {
                return BadRequest(ex.Message);
            }
        }

        /// <summary>
        /// Update user profile
        /// </summary>
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(Guid id, [FromBody] UserUpdateDTO dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var updated = await _service.UpdateAsync(id, dto);

            if (!updated)
                return NotFound();

            return NoContent();
        }

        /// <summary>
        /// Delete user
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