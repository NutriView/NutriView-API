using System.ComponentModel.DataAnnotations;

namespace NutriView.API.Models.DTOs
{
    public class UploadedImageCreateDTO
    {
        [Required]
        public Guid UserId { get; set; }

        [Required]
        public string FilePath { get; set; } = string.Empty;
    }
}
