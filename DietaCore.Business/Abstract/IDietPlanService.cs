using DietaCore.Dto.DietPlanDTOs;
using DietaCore.Shared.Common.Responses.Concrete;

namespace DietaCore.Business.Abstract
{
    public interface IDietPlanService
    {
        Task<Response<DietPlanResponseDto>> GetByIdForDietitianAsync(int id);
        Task<Response<IList<DietPlanResponseDto>>> GetByDietitianUserIdAsync();
        Task<Response<IList<DietPlanResponseDto>>> GetByClientIdForDietitianAsync(int clientId);
        Task<Response<DietPlanResponseDto>> CreateByDietitianAsync(DietPlanRequestDto dietPlanDto);
        Task<Response<DietPlanResponseDto>> UpdateByDietitianAsync(DietPlanUpdateDto dietPlanDto);
        Task<Response<bool>> DeleteByDietitianAsync(int id);
        Task<Response<IList<DietPlanResponseDto>>> GetByClientUserIdAsync();
        Task<Response<DietPlanResponseDto>> GetByIdForClientAsync(int dietPlanId);
    }
}
