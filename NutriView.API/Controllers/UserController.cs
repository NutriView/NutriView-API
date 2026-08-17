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
    public class UserController : ControllerBase
    {
        private readonly IUserService _service;

        public UserController(IUserService service)
        {
            _service = service;
        }

        /// <summary>
        /// Register a new user and sign them in
        /// </summary>
        [AllowAnonymous]
        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] UserCreateDTO dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            try
            {
                var auth = await _service.RegisterAsync(dto);

                return CreatedAtAction(nameof(GetMe), null, auth);
            }
            catch (ValidationException ex)
            {
                return BadRequest(ex.Message);
            }
        }

        /// <summary>
        /// Log in with email and password
        /// </summary>
        [AllowAnonymous]
        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginDTO dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var auth = await _service.LoginAsync(dto);

            if (auth == null)
                return Unauthorized("Invalid email or password");

            return Ok(auth);
        }

        /// <summary>
        /// Get the signed-in user's profile
        /// </summary>
        [HttpGet("me")]
        public async Task<IActionResult> GetMe()
        {
            var user = await _service.GetByIdAsync(User.GetUserId());

            if (user == null)
                return NotFound();

            return Ok(user);
        }

        /// <summary>
        /// Update the signed-in user's profile
        /// </summary>
        [HttpPut("me")]
        public async Task<IActionResult> UpdateMe([FromBody] UserUpdateDTO dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var updated = await _service.UpdateAsync(User.GetUserId(), dto);

            if (!updated)
                return NotFound();

            return NoContent();
        }

        /// <summary>
        /// Delete the signed-in user's account
        /// </summary>
        [HttpDelete("me")]
        public async Task<IActionResult> DeleteMe()
        {
            var deleted = await _service.DeleteAsync(User.GetUserId());

            if (!deleted)
                return NotFound();

            return NoContent();
        }

        /// <summary>
        /// Get the signed-in user's daily nutrition goal
        /// </summary>
        [HttpGet("me/nutrition-goal")]
        public async Task<IActionResult> GetNutritionGoal()
        {
            var goal = await _service.GetNutritionGoalAsync(User.GetUserId());

            if (goal == null)
                return NotFound();

            return Ok(goal);
        }

        /// <summary>
        /// Set (create or replace) the signed-in user's daily nutrition goal
        /// </summary>
        [HttpPut("me/nutrition-goal")]
        public async Task<IActionResult> SetNutritionGoal([FromBody] NutritionValueDTO dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var updated = await _service.SetNutritionGoalAsync(User.GetUserId(), dto);

            if (!updated)
                return NotFound();

            return NoContent();
        }
    }
}
