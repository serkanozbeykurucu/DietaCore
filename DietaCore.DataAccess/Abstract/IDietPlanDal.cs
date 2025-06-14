using DietaCore.Entities.Concrete;

namespace DietaCore.DataAccess.Abstract
{
    public interface IDietPlanDal : IGenericDal<DietPlan>
    {
        Task<DietPlan> GetByIdWithDetailsAsync(int id);
        Task<IList<DietPlan>> GetByClientIdAsync(int clientId);
        Task<IList<DietPlan>> GetByDietitianIdAsync(int dietitianId);

        Task<IList<DietPlan>> GetActiveByClientIdAsync(int clientId);
        Task<IList<DietPlan>> GetByClientIdWithDetailsAsync(int clientId);
        Task<IList<DietPlan>> GetActivePlansAsync();
        Task<DietPlan> GetCurrentActivePlanByClientIdAsync(int clientId);
    }
}
