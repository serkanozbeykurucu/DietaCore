namespace DietaCore.Entities.Concrete
{
    public class Dietitian : BaseEntity
    {
        public int UserId { get; set; }
        public User User { get; set; }
        public string Specialization { get; set; }
        public string LicenseNumber { get; set; }
        public string Education { get; set; }
        public string Biography { get; set; }
        public IList<Client> Clients { get; set; }
    }
}
