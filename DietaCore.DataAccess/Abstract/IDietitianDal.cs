using DietaCore.Entities.Concrete;

namespace DietaCore.DataAccess.Abstract
{
    public interface IDietitianDal : IGenericDal<Dietitian>
    {
        Task<Dietitian> GetByUserIdAsync(int userId);
        Task<IList<Dietitian>> GetAllWithUsersAsync();
        Task<Dietitian> GetByIdWithUsersAsync(int id);
        Task<Dietitian> GetByIdWithClientsAsync(int id);
    }
}
