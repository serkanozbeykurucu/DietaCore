using DietaCore.Business.Abstract;
using DietaCore.Dto.ClientDTOs;
using DietaCore.Dto.DietitianDTOs;
using DietaCore.Dto.DietPlanDTOs;
using DietaCore.Shared.Common.Responses.Concrete;
using DietaCore.Shared.Constants;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DietaCore.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize(Roles = RoleConstants.Dietitian + "," + RoleConstants.Admin)]

    public class DietitianController : ControllerBase
    {
        private readonly IDietitianService _dietitianService;
        private readonly IClientService _clientService;
        private readonly IDietPlanService _dietPlanService;
        public DietitianController(IDietitianService dietitianService,IClientService clientService,IDietPlanService dietPlanService)
        {
            _dietitianService = dietitianService;
            _clientService = clientService;
            _dietPlanService = dietPlanService;
        }

        [HttpGet]
        [Route("GetProfile")]
        public async Task<Response<DietitianResponseDto>> GetProfile()
        {
            return await _dietitianService.GetByUserIdAsync();
        }
        [HttpGet]
        [Route("GetClients")]
        public async Task<Response<IList<ClientResponseDto>>> GetClients()
        {
            return await _clientService.GetClientsByDietitianAsync();
        }

        [HttpGet]
        [Route("GetClientById")]
        public async Task<Response<ClientResponseDto>> GetClientById(int id)
        {
            return await _clientService.GetClientByIdForDietitianAsync(id);
        }

        [HttpPost]
        [Route("CreateClient")]
        public async Task<Response<ClientResponseDto>> CreateClient([FromBody] ClientRequestDto clientDto)
        {
            return await _clientService.CreateClientForDietitianAsync(clientDto);
        }

        [HttpPut]
        [Route("UpdateClient")]
        public async Task<Response<ClientResponseDto>> UpdateClient([FromBody] ClientUpdateDto clientDto)
        {
            return await _clientService.UpdateClientForDietitianAsync(clientDto);
        }

        [HttpGet]
        [Route("GetDietPlans")]
        public async Task<Response<IList<DietPlanResponseDto>>> GetDietPlans()
        {
            return await _dietPlanService.GetByDietitianUserIdAsync();
        }

        [HttpGet]
        [Route("GetDietPlanById")]
        public async Task<Response<DietPlanResponseDto>> GetDietPlanById(int id)
        {
            return await _dietPlanService.GetByIdForDietitianAsync(id);
        }

        [HttpGet]
        [Route("GetClientDietPlans")]
        public async Task<Response<IList<DietPlanResponseDto>>> GetClientDietPlans(int clientId)
        {
            return await _dietPlanService.GetByClientIdForDietitianAsync(clientId);
        }

        [HttpPost]
        [Route("CreateDietPlan")]
        public async Task<Response<DietPlanResponseDto>> CreateDietPlan([FromBody] DietPlanRequestDto dietPlanDto)
        {
            return await _dietPlanService.CreateByDietitianAsync(dietPlanDto);
        }

        [HttpPut]
        [Route("UpdateDietPlan")]
        public async Task<Response<DietPlanResponseDto>> UpdateDietPlan([FromBody] DietPlanUpdateDto dietPlanDto)
        {
            return await _dietPlanService.UpdateByDietitianAsync(dietPlanDto);
        }

        [HttpDelete]
        [Route("DeleteDietPlan")]
        public async Task<Response<bool>> DeleteDietPlan(int id)
        {
            return await _dietPlanService.DeleteByDietitianAsync(id);
        }
    }
}
