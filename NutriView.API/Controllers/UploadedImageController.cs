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
    public class UploadedImageController : ControllerBase
    {
        private readonly IUploadedImageService _uploadedImageService;

        public UploadedImageController(IUploadedImageService uploadedImageService)
        {
            _uploadedImageService = uploadedImageService;
        }

        /// <summary>
        /// Get all uploaded images for the signed-in user
        /// </summary>
        [HttpGet("me")]
        public async Task<IActionResult> GetMine()
        {
            var images = await _uploadedImageService.GetAllByUserAsync(User.GetUserId());
            return Ok(images);
        }

        /// <summary>
        /// Get one of the signed-in user's uploaded images by id
        /// </summary>
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(Guid id)
        {
            var image = await _uploadedImageService.GetByIdAsync(User.GetUserId(), id);

            if (image == null)
                return NotFound();

            return Ok(image);
        }

        /// <summary>
        /// Create a new uploaded image for the signed-in user
        /// </summary>
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] UploadedImageCreateDTO dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            try
            {
                var created = await _uploadedImageService.CreateAsync(User.GetUserId(), dto);

                return CreatedAtAction(nameof(GetById), new { id = created.UploadedImageId }, created);
            }
            catch (ValidationException ex)
            {
                return BadRequest(ex.Message);
            }
        }

        /// <summary>
        /// Update one of the signed-in user's uploaded images
        /// </summary>
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(Guid id, [FromBody] UploadedImageUpdateDTO dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            try
            {
                var updated = await _uploadedImageService.UpdateAsync(User.GetUserId(), id, dto);

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
        /// Delete one of the signed-in user's uploaded images
        /// </summary>
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            var deleted = await _uploadedImageService.DeleteAsync(User.GetUserId(), id);

            if (!deleted)
                return NotFound();

            return NoContent();
        }
    }
}
