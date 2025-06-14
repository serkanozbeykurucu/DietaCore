using DietaCore.Dto.ClientProgressDTOs;
using DietaCore.Shared.Common.Responses.Concrete;

namespace DietaCore.Business.Abstract
{
    public interface IClientProgressService
    {
        Task<Response<List<ProgressDto>>> GetClientProgressByDietitianAsync(int clientId);
        Task<Response<ProgressSummary>> GetClientProgressSummaryByDietitianAsync(int clientId);
        Task<Response<ProgressDto>> CreateProgressByDietitianAsync(ProgressDto dto);
        Task<Response<ProgressDto>> UpdateProgressByDietitianAsync(ProgressDto dto);
        Task<Response<bool>> DeleteProgressByDietitianAsync(int id);
    }
}
