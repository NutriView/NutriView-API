namespace NutriView.API.Models.DTOs
{
    public class UploadedImageUpdateDTO
    {
        public bool IsProcessed { get; set; }

        public Guid? DetectedFoodId { get; set; }

        public float? AIConfidence { get; set; }
    }
}