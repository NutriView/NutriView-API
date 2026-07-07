namespace NutriView.API.Models.DTOs
{
    public class UploadedImageResponseDTO
    {
        public Guid UploadedImageId { get; set; }

        public Guid UserId { get; set; }

        public string FilePath { get; set; } = string.Empty;

        public DateTime UploadedAt { get; set; }

        public bool IsProcessed { get; set; }

        public Guid? DetectedFoodId { get; set; }
        public string? DetectedFoodName { get; set; }

        public float? AIConfidence { get; set; }
    }
}
