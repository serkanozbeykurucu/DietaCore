using DietaCore.Dto.MealDTOs;

namespace DietaCore.Dto.DietPlanDTOs
{
    public class DietPlanResponseDto
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public decimal InitialWeight { get; set; }
        public decimal TargetWeight { get; set; }
        public int ClientId { get; set; }
        public string ClientName { get; set; }
        public int CreatedByDietitianId { get; set; }
        public string DietitianName { get; set; }
        public List<MealResponseDto> Meals { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
    }
}
