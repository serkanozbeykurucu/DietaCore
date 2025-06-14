using DietaCore.Dto.DietitianDTOs;
using DietaCore.Shared.Common.Responses.Concrete;

namespace DietaCore.Business.Abstract
{
    public interface IDietitianService
    {
        Task<Response<IList<DietitianResponseDto>>> GetAllAsync();
        Task<Response<DietitianResponseDto>> GetByIdAsync(int id);
        Task<Response<DietitianResponseDto>> GetByUserIdAsync();
        Task<Response<DietitianResponseDto>> CreateAsync(DietitianRequestDto dietitianDto);
        Task<Response<DietitianResponseDto>> UpdateAsync(DietitianUpdateDto dietitianDto);
        Task<Response<bool>> DeleteAsync(int id);
    }
}
