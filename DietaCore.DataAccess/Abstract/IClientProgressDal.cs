using DietaCore.Entities.Concrete;

namespace DietaCore.DataAccess.Abstract
{
    public interface IClientProgressDal : IGenericDal<ClientProgress>
    {
        Task<IList<ClientProgress>> GetByClientIdAsync(int clientId);
        Task<IList<ClientProgress>> GetByClientIdWithDetailsAsync(int clientId);
        Task<ClientProgress> GetByIdWithDetailsAsync(int progressId);
        Task<IList<ClientProgress>> GetByDateRangeAsync(int clientId, DateTime startDate, DateTime endDate);
        Task<IList<ClientProgress>> GetByDietitianIdAsync(int dietitianId);
        Task<IList<ClientProgress>> GetLatestByClientIdAsync(int clientId, int count = 10);
        Task<ClientProgress> GetLatestByClientIdAsync(int clientId);
        Task<IList<ClientProgress>> GetByClientIdsAsync(IList<int> clientIds);
    }
}
