using DietaCore.Business.Abstract;
using DietaCore.Dto.ClientDTOs;
using DietaCore.Dto.DietitianDTOs;
using DietaCore.Shared.Common.Responses.Concrete;
using DietaCore.Shared.Constants;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DietaCore.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize(Roles = RoleConstants.Admin)]
    public class AdminController : ControllerBase
    {
        private readonly IDietitianService _dietitianService;
        private readonly IClientService _clientService;
        public AdminController(IDietitianService dietitianService, IClientService clientService)
        {
            _dietitianService = dietitianService;
            _clientService = clientService;
        }
        [HttpGet]
        [Route("GetAllDietitians")]
        public async Task<Response<IList<DietitianResponseDto>>> GetAllDietitians()
        {
            return await _dietitianService.GetAllAsync();
        }

        [HttpGet]
        [Route("GetDietitianById")]
        public async Task<Response<DietitianResponseDto>> GetDietitianById(int id)
        {
            return await _dietitianService.GetByIdAsync(id);
        }

        [HttpPost]
        [Route("CreateDietitian")]
        public async Task<Response<DietitianResponseDto>> CreateDietitian([FromBody] DietitianRequestDto dietitianDto)
        {
            return await _dietitianService.CreateAsync(dietitianDto);
        }

        [HttpPut]
        [Route("UpdateDietitian")]
        public async Task<Response<DietitianResponseDto>> UpdateDietitian([FromBody] DietitianUpdateDto dietitianDto)
        {
            return await _dietitianService.UpdateAsync(dietitianDto);
        }

        [HttpDelete]
        [Route("DeleteDietitian")]
        public async Task<Response<bool>> DeleteDietitian(int id)
        {
            return await _dietitianService.DeleteAsync(id);
        }

        [HttpGet]
        [Route("GetAllClients")]
        public async Task<Response<IList<ClientResponseDto>>> GetAllClients()
        {
            return await _clientService.GetAllClientsForAdminAsync();
        }

        [HttpGet]
        [Route("GetClientById")]
        public async Task<Response<ClientResponseDto>> GetClientById(int id)
        {
            return await _clientService.GetClientByIdForAdminAsync(id);
        }

        [HttpPost]
        [Route("CreateClient")]
        public async Task<Response<ClientResponseDto>> CreateClient([FromBody] ClientRequestDto clientDto)
        {
            return await _clientService.CreateClientForAdminAsync(clientDto);
        }

        [HttpPut]
        [Route("UpdateClient")]
        public async Task<Response<ClientResponseDto>> UpdateClient([FromBody] ClientUpdateDto clientDto)
        {
            return await _clientService.UpdateClientForAdminAsync(clientDto);
        }

        [HttpDelete]
        [Route("DeleteClient")]
        public async Task<Response<bool>> DeleteClient(int id)
        {
            return await _clientService.DeleteClientAsync(id);
        }

        [HttpPost]
        [Route("AssignClientToDietitian")]
        public async Task<Response<bool>> AssignClientToDietitian(int clientId, int dietitianId)
        {
            return await _clientService.AssignClientToDietitianAsync(clientId, dietitianId);
        }

        [HttpPost]
        [Route("RemoveClientFromDietitian")]
        public async Task<Response<bool>> RemoveClientFromDietitian(int clientId)
        {
            return await _clientService.RemoveClientFromDietitianAsync(clientId);
        }
    }
}