namespace DietaCore.Dto.DietPlanDTOs
{
    public class DietPlanRequestDto
    {
        public string Title { get; set; }
        public string Description { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public decimal InitialWeight { get; set; }
        public decimal TargetWeight { get; set; }
        public int ClientId { get; set; }
    }
}
