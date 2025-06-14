using DietaCore.Entities.Concrete;

namespace DietaCore.DataAccess.Abstract
{
    public interface IClientDal : IGenericDal<Client>
    {
        Task<Client> GetByUserIdAsync(int userId);
        Task<IList<Client>> GetAllWithUsersAsync();
        Task<Client> GetByIdWithUserAsync(int id);
        Task<IList<Client>> GetByDietitianIdAsync(int dietitianId);
        Task<int?> GetDietitianIdByClientId(int clientId);
        Task<Client> GetByIdWithDietPlansAsync(int id);
    }

}
