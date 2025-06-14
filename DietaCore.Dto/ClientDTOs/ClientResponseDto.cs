namespace DietaCore.Dto.ClientDTOs
{
    public class ClientResponseDto
    {
        public int Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public string PhoneNumber { get; set; }
        public int? DietitianId { get; set; }
        public string DietitianName { get; set; }
        public DateTime DateOfBirth { get; set; }
        public int Age { get; set; }
        public string Gender { get; set; }
        public decimal Height { get; set; }
        public decimal InitialWeight { get; set; }
        public decimal? CurrentWeight { get; set; }
        public string MedicalConditions { get; set; }
        public string Allergies { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
    }
}
