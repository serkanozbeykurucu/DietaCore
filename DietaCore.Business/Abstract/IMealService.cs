using DietaCore.Dto.MealDTOs;
using DietaCore.Shared.Common.Responses.Concrete;

namespace DietaCore.Business.Abstract
{
    public interface IMealService
    {
        Task<Response<IList<MealResponseDto>>> GetByDietPlanIdForDietitianAsync(int dietPlanId);
        Task<Response<IList<MealResponseDto>>> GetByDietPlanIdForClientAsync(int dietPlanId);
        Task<Response<MealResponseDto>> GetByIdForDietitianAsync(int id);
        Task<Response<MealResponseDto>> CreateByDietitianAsync(MealRequestDto mealDto);
        Task<Response<MealResponseDto>> UpdateByDietitianAsync(MealUpdateDto mealDto);
        Task<Response<bool>> DeleteByDietitianAsync(int id);
    }
}
