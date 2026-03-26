using System;

namespace NutriView.API.Models.Entities
{
    public class UploadedImage
    {
        public Guid UploadedImageId { get; set; }

        public Guid UserId { get; set; }

        public string FilePath { get; set; } = string.Empty;

        public DateTime UploadedAt { get; set; }

        public bool IsProcessed { get; set; }

        public Guid? DetectedFoodId { get; set; }

        public float? AIConfidence { get; set; }

        public User User { get; set; } = null!;

        public Food? DetectedFood { get; set; }
    }
}
