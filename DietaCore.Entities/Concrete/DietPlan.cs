using System.ComponentModel.DataAnnotations;
using System.Security.Principal;

namespace DietaCore.Entities.Concrete
{
    public class DietPlan : BaseEntity
    {
        public string Title { get; set; }
        public string Description { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public decimal InitialWeight { get; set; }
        public decimal TargetWeight { get; set; } 
        public int ClientId { get; set; }
        public Client Client { get; set; }
        public int CreatedByDietitianId { get; set; }
        public Dietitian CreatedByDietitian { get; set; }
        public IList<Meal> Meals { get; set; }
    }
}
