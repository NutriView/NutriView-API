using System.ComponentModel.DataAnnotations;

namespace NutriView.API.Models.DTOs
{
    public class UploadedImageCreateDTO
    {
        [Required]
        public string FilePath { get; set; } = string.Empty;
    }
}
