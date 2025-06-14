namespace DietaCore.Dto.ClientProgressDTOs
{
    public class ProgressDto
    {
        public int Id { get; set; }
        public int ClientId { get; set; }
        public decimal Weight { get; set; }
        public decimal? BodyFat { get; set; }
        public decimal? Muscle { get; set; }
        public decimal? Waist { get; set; }
        public string Notes { get; set; }
        public DateTime Date { get; set; }
    }
}
