using DietaCore.Business.Abstract;
using DietaCore.Dto.MealDTOs;
using DietaCore.Shared.Common.Responses.Concrete;
using DietaCore.Shared.Constants;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace DietaCore.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize(Roles = RoleConstants.Dietitian + "," + RoleConstants.Admin)]

    public class MealController : ControllerBase
    {
        private readonly IMealService _mealService;

        public MealController(IMealService mealService)
        {
            _mealService = mealService;
        }

        [HttpGet]
        [Route("GetByIdForDietitian")]
        public async Task<Response<MealResponseDto>> GetByIdForDietitian(int id)
        {
            return await _mealService.GetByIdForDietitianAsync(id);
        }
        [HttpGet]
        [Route("GetByDietPlanId")]
        public async Task<Response<IList<MealResponseDto>>> GetByDietPlanId(int dietPlanId)
        {
            return await _mealService.GetByDietPlanIdForDietitianAsync(dietPlanId);
        }
        [HttpPost]
        [Route("CreateByDietitianAsync")]
        public async Task<Response<MealResponseDto>> CreateByDietitianAsync([FromBody] MealRequestDto mealDto)
        {
            return await _mealService.CreateByDietitianAsync(mealDto);
        }
        [HttpPut]
        [Route("UpdateByDietitianAsync")]
        public async Task<Response<MealResponseDto>> UpdateByDietitianAsync([FromBody] MealUpdateDto mealDto)
        {
            return await _mealService.UpdateByDietitianAsync(mealDto);
        }
        [HttpDelete]
        [Route("DeleteByDietitian")]
        public async Task<Response<bool>> DeleteByDietitianAsync(int id)
        {
            return await _mealService.DeleteByDietitianAsync(id);
        }
    }
}
