namespace DietaCore.Entities.Concrete
{
    public class Client : BaseEntity
    {
        public int UserId { get; set; }
        public User User { get; set; }
        public int? DietitianId { get; set; }
        public Dietitian Dietitian { get; set; }
        public DateTime DateOfBirth { get; set; }
        public string Gender { get; set; }
        public decimal Height { get; set; }
        public decimal InitialWeight { get; set; }
        public decimal? CurrentWeight { get; set; }
        public string MedicalConditions { get; set; }
        public string Allergies { get; set; }
        public IList<DietPlan> DietPlans { get; set; }
    }
}
