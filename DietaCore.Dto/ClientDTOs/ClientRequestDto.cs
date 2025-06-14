namespace DietaCore.Dto.ClientDTOs
{
    public class ClientRequestDto
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public string PhoneNumber { get; set; }
        public int? DietitianId { get; set; }
        public DateTime DateOfBirth { get; set; }
        public string Gender { get; set; }
        public decimal Height { get; set; }
        public decimal InitialWeight { get; set; }
        public string MedicalConditions { get; set; }
        public string Allergies { get; set; }
    }
}
