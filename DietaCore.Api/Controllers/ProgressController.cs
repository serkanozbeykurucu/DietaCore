using DietaCore.Business.Abstract;
using DietaCore.Dto.ClientProgressDTOs;
using DietaCore.Shared.Common.Responses.Concrete;
using DietaCore.Shared.Constants;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DietaCore.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize(Roles = RoleConstants.Dietitian + "," + RoleConstants.Admin)]

    public class ProgressController : ControllerBase
    {
        private readonly IClientProgressService _service;
        public ProgressController(IClientProgressService service)
        {
            _service = service;
        }

        [HttpGet]
        [Route("GetProgress")]
        public async Task<Response<List<ProgressDto>>> GetProgress(int clientId)
        {
            return await _service.GetClientProgressByDietitianAsync(clientId);
        }

        [HttpGet]
        [Route("GetSummary")]

        public async Task<Response<ProgressSummary>> GetSummary(int clientId)
        {
            return await _service.GetClientProgressSummaryByDietitianAsync(clientId);
        }

        [HttpPost]
        [Route("CreateProgress")]
        public async Task<Response<ProgressDto>> CreateProgress([FromBody] ProgressDto dto)
        {
            return await _service.CreateProgressByDietitianAsync(dto);
        }

        [HttpPut]
        [Route("UpdateProgress")]
        public async Task<Response<ProgressDto>> UpdateProgress([FromBody] ProgressDto dto)
        {
            return await _service.UpdateProgressByDietitianAsync(dto);
        }

        [HttpDelete]
        [Route("DeleteProgress")]
        public async Task<Response<bool>> DeleteProgress(int id)
        {
            return await _service.DeleteProgressByDietitianAsync(id);
        }
    }
}
