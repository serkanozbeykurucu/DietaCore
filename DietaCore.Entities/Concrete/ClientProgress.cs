namespace DietaCore.Entities.Concrete
{
    public class ClientProgress : BaseEntity
    {
        public int ClientId { get; set; }
        public Client Client { get; set; }
        public decimal Weight { get; set; }
        public decimal? BodyFatPercentage { get; set; }
        public decimal? MuscleMass { get; set; }
        public decimal? WaistCircumference { get; set; } 
        public decimal? ChestCircumference { get; set; } 
        public decimal? HipCircumference { get; set; }
        public string Notes { get; set; }
        public DateTime RecordedDate { get; set; }
        public int? RecordedByDietitianId { get; set; }
        public Dietitian RecordedByDietitian { get; set; }
        public bool IsClientEntry { get; set; }
    }
}
