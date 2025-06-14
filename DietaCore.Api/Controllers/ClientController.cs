using DietaCore.Business.Abstract;
using DietaCore.Dto.ClientDTOs;
using DietaCore.Dto.DietPlanDTOs;
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
    [Authorize(Roles = RoleConstants.Client + "," + RoleConstants.Admin)]

    public class ClientController : ControllerBase
    {
        private readonly IClientService _clientService;
        private readonly IDietPlanService _dietPlanService;
        private readonly IMealService _mealService;
        public ClientController(IClientService clientService, IDietPlanService dietPlanService, IMealService mealService)
        {
            _clientService = clientService;
            _dietPlanService = dietPlanService;
            _mealService = mealService;
        }

        [HttpGet]
        [Route("GetProfile")]
        public async Task<Response<ClientResponseDto>> GetProfile()
        {
            return await _clientService.GetClientProfileAsync();
        }

        [HttpGet]
        [Route("GetDietPlans")]
        public async Task<Response<IList<DietPlanResponseDto>>> GetDietPlans()
        {
            return await _dietPlanService.GetByClientUserIdAsync();
        }

        [HttpGet]
        [Route("GetDietPlanById")]
        public async Task<Response<DietPlanResponseDto>> GetDietPlanById(int id)
        {
            return await _dietPlanService.GetByIdForClientAsync(id);
        }

        [HttpGet]
        [Route("GetMealsByDietPlanId")]
        public async Task<Response<IList<MealResponseDto>>> GetMealsByDietPlanId(int dietPlanId)
        {
            return await _mealService.GetByDietPlanIdForClientAsync(dietPlanId);
        }
    }
}
