using Microsoft.AspNetCore.Mvc;
using NutriView.API.Exceptions;
using NutriView.API.Models.DTOs;
using NutriView.API.Services;

namespace NutriView.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UploadedImageController : ControllerBase
    {
        private readonly IUploadedImageService _uploadedImageService;

        public UploadedImageController(IUploadedImageService uploadedImageService)
        {
            _uploadedImageService = uploadedImageService;
        }

        /// <summary>
        /// Get all uploaded images
        /// </summary>
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var images = await _uploadedImageService.GetAllAsync();
            return Ok(images);
        }

        /// <summary>
        /// Get uploaded image by ID
        /// </summary>
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(Guid id)
        {
            var image = await _uploadedImageService.GetByIdAsync(id);

            if (image == null)
                return NotFound();

            return Ok(image);
        }

        /// <summary>
        /// Create new uploaded image
        /// </summary>
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] UploadedImageCreateDTO dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            try
            {
                var created = await _uploadedImageService.CreateAsync(dto);

                return CreatedAtAction(nameof(GetById), new { id = created.UploadedImageId }, created);
            }
            catch (ValidationException ex)
            {
                return BadRequest(ex.Message);
            }
        }

        /// <summary>
        /// Update uploaded image
        /// </summary>
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(Guid id, [FromBody] UploadedImageUpdateDTO dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            try
            {
                var updated = await _uploadedImageService.UpdateAsync(id, dto);

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
        /// Delete uploaded image
        /// </summary>
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            var deleted = await _uploadedImageService.DeleteAsync(id);

            if (!deleted)
                return NotFound();

            return NoContent();
        }
    }
}