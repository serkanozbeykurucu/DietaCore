using DietaCore.Dto.ClientDTOs;
using DietaCore.Shared.Common.Responses.Concrete;

namespace DietaCore.Business.Abstract
{
    public interface IClientService
    {
        Task<Response<ClientResponseDto>> GetClientProfileAsync();

        Task<Response<IList<ClientResponseDto>>> GetClientsByDietitianAsync();
        Task<Response<ClientResponseDto>> GetClientByIdForDietitianAsync(int id);
        Task<Response<ClientResponseDto>> CreateClientForDietitianAsync(ClientRequestDto clientDto);
        Task<Response<ClientResponseDto>> UpdateClientForDietitianAsync(ClientUpdateDto clientDto);

        Task<Response<ClientResponseDto>> CreateClientForAdminAsync(ClientRequestDto clientDto);
        Task<Response<ClientResponseDto>> UpdateClientForAdminAsync(ClientUpdateDto clientDto);
        Task<Response<ClientResponseDto>> GetClientByIdForAdminAsync(int id);
        Task<Response<IList<ClientResponseDto>>> GetAllClientsForAdminAsync();
        Task<Response<bool>> DeleteClientAsync(int id);
        Task<Response<bool>> AssignClientToDietitianAsync(int clientId, int dietitianId);
        Task<Response<bool>> RemoveClientFromDietitianAsync(int clientId);
    }
}
