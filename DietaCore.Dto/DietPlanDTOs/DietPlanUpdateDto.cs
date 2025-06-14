namespace DietaCore.Dto.DietPlanDTOs
{
    public class DietPlanUpdateDto
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public decimal TargetWeight { get; set; }
    }
}
