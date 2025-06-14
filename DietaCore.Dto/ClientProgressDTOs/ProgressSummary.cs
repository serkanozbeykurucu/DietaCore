namespace DietaCore.Dto.ClientProgressDTOs
{
    public class ProgressSummary
    {
        public int ClientId { get; set; }
        public string ClientName { get; set; }
        public decimal StartWeight { get; set; }
        public decimal CurrentWeight { get; set; }
        public decimal TargetWeight { get; set; }
        public decimal WeightLoss { get; set; }
        public List<ProgressDto> Recent { get; set; }
    }
}
