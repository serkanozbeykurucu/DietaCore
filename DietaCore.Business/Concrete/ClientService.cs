using DietaCore.Business.Abstract;
using DietaCore.Business.Utilities.Abstract;
using DietaCore.DataAccess.Abstract;
using DietaCore.Dto.ClientDTOs;
using DietaCore.Entities.Concrete;
using DietaCore.Shared.Common.Responses.ComplexTypes;
using DietaCore.Shared.Common.Responses.Concrete;
using DietaCore.Shared.Constants;
using Microsoft.AspNetCore.Identity;

namespace DietaCore.Business.Concrete
{
    public class ClientService : IClientService
    {
        private readonly IClientDal _clientDal;
        private readonly IDietitianDal _dietitianDal;
        private readonly UserManager<User> _userManager;
        private readonly IUserContextHelper _userContextHelper;
        public ClientService(IClientDal clientDal, IDietitianDal dietitianDal, UserManager<User> userManager, IUserContextHelper userContextHelper)
        {
            _clientDal = clientDal;
            _dietitianDal = dietitianDal;
            _userManager = userManager;
            _userContextHelper = userContextHelper;
        }
        public async Task<Response<ClientResponseDto>> GetClientProfileAsync()
        {
            var userId = _userContextHelper.GetCurrentUserId();
            var client = await _clientDal.GetByUserIdAsync(userId);

            if (client == null)
            {
                return new Response<ClientResponseDto>(ResponseCode.NotFound, "Client not found.");
            }

            var clientDto = MapToClientResponseDto(client);
            return new Response<ClientResponseDto>(ResponseCode.Success, clientDto, "Client profile retrieved successfully.");
        }
        public async Task<Response<IList<ClientResponseDto>>> GetClientsByDietitianAsync()
        {
            var dietitianUserId = _userContextHelper.GetCurrentUserId();
            var dietitian = await _dietitianDal.GetByUserIdAsync(dietitianUserId);

            if (dietitian == null)
            {
                return new Response<IList<ClientResponseDto>>(ResponseCode.NotFound, "Dietitian not found.");
            }

            var clients = await _clientDal.GetByDietitianIdAsync(dietitian.Id);
            var clientDtos = clients.Select(MapToClientResponseDto).ToList();

            return new Response<IList<ClientResponseDto>>(ResponseCode.Success, clientDtos, "Clients retrieved successfully.");
        }
        public async Task<Response<ClientResponseDto>> GetClientByIdForDietitianAsync(int id)
        {
            var dietitianUserId = _userContextHelper.GetCurrentUserId();
            var dietitian = await _dietitianDal.GetByUserIdAsync(dietitianUserId);

            if (dietitian == null)
            {
                return new Response<ClientResponseDto>(ResponseCode.NotFound, "Dietitian not found.");
            }

            var client = await _clientDal.GetByIdWithUserAsync(id);
            if (client == null)
            {
                return new Response<ClientResponseDto>(ResponseCode.NotFound, "Client not found.");
            }

            if (client.DietitianId != dietitian.Id)
            {
                return new Response<ClientResponseDto>(ResponseCode.Forbidden, "You don't have permission to access this client.");
            }

            var clientDto = MapToClientResponseDto(client);
            return new Response<ClientResponseDto>(ResponseCode.Success, clientDto, "Client retrieved successfully.");
        }
        public async Task<Response<ClientResponseDto>> CreateClientForDietitianAsync(ClientRequestDto clientDto)
        {
            var dietitianUserId = _userContextHelper.GetCurrentUserId();
            var dietitian = await _dietitianDal.GetByUserIdAsync(dietitianUserId);

            if (dietitian == null)
            {
                return new Response<ClientResponseDto>(ResponseCode.NotFound, "Dietitian not found.");
            }

            clientDto.DietitianId = dietitian.Id;
            return await CreateClientInternalAsync(clientDto);
        }
        public async Task<Response<ClientResponseDto>> UpdateClientForDietitianAsync(ClientUpdateDto clientDto)
        {
            var dietitianUserId = _userContextHelper.GetCurrentUserId();
            var dietitian = await _dietitianDal.GetByUserIdAsync(dietitianUserId);

            if (dietitian == null)
            {
                return new Response<ClientResponseDto>(ResponseCode.NotFound, "Dietitian not found.");
            }

            var client = await _clientDal.GetByIdWithUserAsync(clientDto.Id);
            if (client == null)
            {
                return new Response<ClientResponseDto>(ResponseCode.NotFound, "Client not found.");
            }

            if (client.DietitianId != dietitian.Id)
            {
                return new Response<ClientResponseDto>(ResponseCode.Forbidden, "You don't have permission to update this client.");
            }

            clientDto.DietitianId = dietitian.Id;
            return await UpdateClientInternalAsync(clientDto);
        }
        public async Task<Response<IList<ClientResponseDto>>> GetAllClientsForAdminAsync()
        {
            var clients = await _clientDal.GetAllWithUsersAsync();
            var clientDtos = clients.Select(MapToClientResponseDto).ToList();

            return new Response<IList<ClientResponseDto>>(ResponseCode.Success, clientDtos, "All clients retrieved successfully.");
        }
        public async Task<Response<ClientResponseDto>> GetClientByIdForAdminAsync(int id)
        {
            var client = await _clientDal.GetByIdWithUserAsync(id);
            if (client == null)
            {
                return new Response<ClientResponseDto>(ResponseCode.NotFound, "Client not found.");
            }

            var clientDto = MapToClientResponseDto(client);
            return new Response<ClientResponseDto>(ResponseCode.Success, clientDto, "Client retrieved successfully.");
        }
        public async Task<Response<ClientResponseDto>> CreateClientForAdminAsync(ClientRequestDto clientDto)
        {
            if (clientDto.DietitianId.HasValue)
            {
                var dietitianExists = await _dietitianDal.ExistsAsync(clientDto.DietitianId.Value);
                if (!dietitianExists)
                {
                    return new Response<ClientResponseDto>(ResponseCode.NotFound, "Dietitian not found.");
                }
            }

            return await CreateClientInternalAsync(clientDto);
        }

        public async Task<Response<ClientResponseDto>> UpdateClientForAdminAsync(ClientUpdateDto clientDto)
        {
            if (clientDto.DietitianId.HasValue)
            {
                var dietitianExists = await _dietitianDal.ExistsAsync(clientDto.DietitianId.Value);
                if (!dietitianExists)
                {
                    return new Response<ClientResponseDto>(ResponseCode.NotFound, "Dietitian not found.");
                }
            }

            return await UpdateClientInternalAsync(clientDto);
        }

        public async Task<Response<bool>> DeleteClientAsync(int id)
        {
            var client = await _clientDal.GetByIdWithUserAsync(id);
            if (client == null)
            {
                return new Response<bool>(ResponseCode.NotFound, "Client not found.");
            }

            await _clientDal.DeleteAsync(id);
            await _userManager.DeleteAsync(client.User);

            return new Response<bool>(ResponseCode.Success, true, "Client deleted successfully.");
        }

        public async Task<Response<bool>> AssignClientToDietitianAsync(int clientId, int dietitianId)
        {
            var client = await _clientDal.GetByIdAsync(clientId);
            if (client == null)
            {
                return new Response<bool>(ResponseCode.NotFound, "Client not found.");
            }

            var dietitianExists = await _dietitianDal.ExistsAsync(dietitianId);
            if (!dietitianExists)
            {
                return new Response<bool>(ResponseCode.NotFound, "Dietitian not found.");
            }

            client.DietitianId = dietitianId;
            await _clientDal.UpdateAsync(client);

            return new Response<bool>(ResponseCode.Success, true, "Client assigned to dietitian successfully.");
        }

        public async Task<Response<bool>> RemoveClientFromDietitianAsync(int clientId)
        {
            var client = await _clientDal.GetByIdAsync(clientId);
            if (client == null)
            {
                return new Response<bool>(ResponseCode.NotFound, "Client not found.");
            }

            client.DietitianId = null;
            await _clientDal.UpdateAsync(client);

            return new Response<bool>(ResponseCode.Success, true, "Client removed from dietitian successfully.");
        }

        private async Task<Response<ClientResponseDto>> CreateClientInternalAsync(ClientRequestDto clientDto)
        {
            var user = new User
            {
                UserName = clientDto.Email,
                Email = clientDto.Email,
                FirstName = clientDto.FirstName,
                LastName = clientDto.LastName,
                PhoneNumber = clientDto.PhoneNumber,
                EmailConfirmed = true,
                CreatedAt = DateTime.UtcNow
            };

            var result = await _userManager.CreateAsync(user, clientDto.Password);
            if (!result.Succeeded)
            {
                return new Response<ClientResponseDto>(ResponseCode.BadRequest,
                    string.Join(", ", result.Errors.Select(e => e.Description)));
            }

            await _userManager.AddToRoleAsync(user, RoleConstants.Client);

            var client = new Client
            {
                UserId = user.Id,
                DietitianId = clientDto.DietitianId,
                DateOfBirth = clientDto.DateOfBirth,
                Gender = clientDto.Gender,
                Height = clientDto.Height,
                InitialWeight = clientDto.InitialWeight,
                CurrentWeight = clientDto.InitialWeight,
                MedicalConditions = clientDto.MedicalConditions,
                Allergies = clientDto.Allergies
            };

            await _clientDal.AddAsync(client);

            var createdClient = await _clientDal.GetByUserIdAsync(user.Id);
            var responseDto = MapToClientResponseDto(createdClient);

            return new Response<ClientResponseDto>(ResponseCode.Success, responseDto, "Client created successfully.");
        }

        private async Task<Response<ClientResponseDto>> UpdateClientInternalAsync(ClientUpdateDto clientDto)
        {
            var client = await _clientDal.GetByIdWithUserAsync(clientDto.Id);
            if (client == null)
            {
                return new Response<ClientResponseDto>(ResponseCode.NotFound, "Client not found.");
            }

            var user = client.User;
            user.FirstName = clientDto.FirstName;
            user.LastName = clientDto.LastName;
            user.PhoneNumber = clientDto.PhoneNumber;

            if (user.Email != clientDto.Email)
            {
                user.Email = clientDto.Email;
                user.UserName = clientDto.Email;
                user.EmailConfirmed = false;
            }

            var userUpdateResult = await _userManager.UpdateAsync(user);
            if (!userUpdateResult.Succeeded)
            {
                return new Response<ClientResponseDto>(ResponseCode.BadRequest,
                    string.Join(", ", userUpdateResult.Errors.Select(e => e.Description)));
            }

            client.DietitianId = clientDto.DietitianId;
            client.DateOfBirth = clientDto.DateOfBirth;
            client.Gender = clientDto.Gender;
            client.Height = clientDto.Height;
            client.CurrentWeight = clientDto.CurrentWeight;
            client.MedicalConditions = clientDto.MedicalConditions;
            client.Allergies = clientDto.Allergies;

            await _clientDal.UpdateAsync(client);

            var updatedClient = await _clientDal.GetByIdWithUserAsync(client.Id);
            var responseDto = MapToClientResponseDto(updatedClient);

            return new Response<ClientResponseDto>(ResponseCode.Success, responseDto, "Client updated successfully.");
        }

        private ClientResponseDto MapToClientResponseDto(Client client)
        {
            return new ClientResponseDto
            {
                Id = client.Id,
                FirstName = client.User.FirstName,
                LastName = client.User.LastName,
                Email = client.User.Email,
                PhoneNumber = client.User.PhoneNumber,
                DietitianId = client.DietitianId,
                DietitianName = client.Dietitian != null ?
                    $"{client.Dietitian.User.FirstName} {client.Dietitian.User.LastName}" : null,
                DateOfBirth = client.DateOfBirth,
                Age = CalculateAge(client.DateOfBirth),
                Gender = client.Gender,
                Height = client.Height,
                InitialWeight = client.InitialWeight,
                CurrentWeight = client.CurrentWeight,
                MedicalConditions = client.MedicalConditions,
                Allergies = client.Allergies,
                CreatedAt = client.CreatedAt,
                UpdatedAt = client.UpdatedAt
            };
        }
        private int CalculateAge(DateTime dateOfBirth)
        {
            var today = DateTime.Today;
            var age = today.Year - dateOfBirth.Year;

            if (dateOfBirth.Date > today.AddYears(-age))
                age--;

            return age;
        }

    }
}
