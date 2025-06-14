using DietaCore.Business.Abstract;
using DietaCore.Business.Utilities.Abstract;
using DietaCore.Business.Utilities.Concrete;
using DietaCore.DataAccess.Abstract;
using DietaCore.Dto.DietPlanDTOs;
using DietaCore.Dto.MealDTOs;
using DietaCore.Entities.Concrete;
using DietaCore.Shared.Common.Responses.ComplexTypes;
using DietaCore.Shared.Common.Responses.Concrete;
using Microsoft.AspNetCore.Http;

namespace DietaCore.Business.Concrete
{
    public class DietPlanService : IDietPlanService
    {
        private readonly IDietPlanDal _dietPlanDal;
        private readonly IClientDal _clientDal;
        private readonly IDietitianDal _dietitianDal;
        private readonly IMealDal _mealDal;
        private readonly IUserContextHelper _userContextHelper;

        public DietPlanService(IDietPlanDal dietPlanDal, IClientDal clientDal, IDietitianDal dietitianDal, IMealDal mealDal, IUserContextHelper userContextHelper)
        {
            _dietPlanDal = dietPlanDal;
            _clientDal = clientDal;
            _dietitianDal = dietitianDal;
            _mealDal = mealDal;
            _userContextHelper = userContextHelper;
        }
        public async Task<Response<IList<DietPlanResponseDto>>> GetByDietitianUserIdAsync()
        {
            var dietitianUserId = _userContextHelper.GetCurrentUserId();
            var dietitian = await _dietitianDal.GetByUserIdAsync(dietitianUserId);
            if (dietitian == null)
            {
                return new Response<IList<DietPlanResponseDto>>(ResponseCode.NotFound, "Dietitian not found.");
            }

            var dietPlans = await _dietPlanDal.GetByDietitianIdAsync(dietitian.Id);
            var dietPlanDtos = new List<DietPlanResponseDto>();

            foreach (var dietPlan in dietPlans)
            {
                var meals = await _mealDal.GetByDietPlanIdAsync(dietPlan.Id);
                dietPlanDtos.Add(MapToDietPlanResponseDto(dietPlan, meals));
            }

            return new Response<IList<DietPlanResponseDto>>(ResponseCode.Success, dietPlanDtos, "Dietitian diet plans retrieved successfully.");
        }
        public async Task<Response<DietPlanResponseDto>> GetByIdForDietitianAsync(int id)
        {
            var dietitianUserId = _userContextHelper.GetCurrentUserId();
            var dietPlan = await _dietPlanDal.GetByIdWithDetailsAsync(id);
            if (dietPlan == null)
            {
                return new Response<DietPlanResponseDto>(ResponseCode.NotFound, "Diet plan not found.");
            }

            var dietitian = await _dietitianDal.GetByUserIdAsync(dietitianUserId);
            if (dietitian == null)
            {
                return new Response<DietPlanResponseDto>(ResponseCode.NotFound, "Dietitian not found.");
            }

            if (dietPlan.CreatedByDietitianId != dietitian.Id)
            {
                return new Response<DietPlanResponseDto>(ResponseCode.Forbidden, "You don't have permission to access this diet plan.");
            }

            var meals = await _mealDal.GetByDietPlanIdAsync(id);
            var dietPlanDto = MapToDietPlanResponseDto(dietPlan, meals);

            return new Response<DietPlanResponseDto>(ResponseCode.Success, dietPlanDto, "Diet plan retrieved successfully.");
        }
        public async Task<Response<IList<DietPlanResponseDto>>> GetByClientIdForDietitianAsync(int clientId)
        {
            var dietitianUserId = _userContextHelper.GetCurrentUserId();
            var client = await _clientDal.GetByIdWithUserAsync(clientId);
            if (client == null)
            {
                return new Response<IList<DietPlanResponseDto>>(ResponseCode.NotFound, "Client not found.");
            }

            var dietitian = await _dietitianDal.GetByUserIdAsync(dietitianUserId);
            if (dietitian == null)
            {
                return new Response<IList<DietPlanResponseDto>>(ResponseCode.NotFound, "Dietitian not found.");
            }

            if (client.DietitianId != dietitian.Id)
            {
                return new Response<IList<DietPlanResponseDto>>(ResponseCode.Forbidden, "You don't have permission to access this client's diet plans.");
            }

            var dietPlans = await _dietPlanDal.GetByClientIdAsync(clientId);
            var dietPlanDtos = new List<DietPlanResponseDto>();

            foreach (var dietPlan in dietPlans)
            {
                var meals = await _mealDal.GetByDietPlanIdAsync(dietPlan.Id);
                dietPlanDtos.Add(MapToDietPlanResponseDto(dietPlan, meals));
            }

            return new Response<IList<DietPlanResponseDto>>(ResponseCode.Success, dietPlanDtos, "Client diet plans retrieved successfully.");
        }
        public async Task<Response<DietPlanResponseDto>> CreateByDietitianAsync(DietPlanRequestDto dietPlanDto)
        {
            var dietitianUserId = _userContextHelper.GetCurrentUserId();
            var client = await _clientDal.GetByIdWithUserAsync(dietPlanDto.ClientId);
            if (client == null)
            {
                return new Response<DietPlanResponseDto>(ResponseCode.NotFound, "Client not found.");
            }

            var dietitian = await _dietitianDal.GetByUserIdAsync(dietitianUserId);
            if (dietitian == null)
            {
                return new Response<DietPlanResponseDto>(ResponseCode.NotFound, "Dietitian not found.");
            }

            if (client.DietitianId != dietitian.Id)
            {
                return new Response<DietPlanResponseDto>(ResponseCode.Forbidden, "You don't have permission to create diet plan for this client.");
            }

            var dietPlan = new DietPlan
            {
                Title = dietPlanDto.Title,
                Description = dietPlanDto.Description,
                StartDate = dietPlanDto.StartDate,
                EndDate = dietPlanDto.EndDate,
                InitialWeight = dietPlanDto.InitialWeight,
                TargetWeight = dietPlanDto.TargetWeight,
                ClientId = dietPlanDto.ClientId,
                CreatedByDietitianId = dietitian.Id
            };
            await _dietPlanDal.AddAsync(dietPlan);

            var createdDietPlan = await _dietPlanDal.GetByIdWithDetailsAsync(dietPlan.Id);
            var meals = await _mealDal.GetByDietPlanIdAsync(dietPlan.Id);

            var responseDto = MapToDietPlanResponseDto(createdDietPlan, meals);

            return new Response<DietPlanResponseDto>(ResponseCode.Success, responseDto, "Diet plan created successfully.");
        }
        public async Task<Response<DietPlanResponseDto>> UpdateByDietitianAsync(DietPlanUpdateDto dietPlanDto)
        {
            var dietitianUserId = _userContextHelper.GetCurrentUserId();
            var dietPlan = await _dietPlanDal.GetByIdWithDetailsAsync(dietPlanDto.Id);
            if (dietPlan == null)
            {
                return new Response<DietPlanResponseDto>(ResponseCode.NotFound, "Diet plan not found.");
            }

            var dietitian = await _dietitianDal.GetByUserIdAsync(dietitianUserId);
            if (dietitian == null)
            {
                return new Response<DietPlanResponseDto>(ResponseCode.NotFound, "Dietitian not found.");
            }

            if (dietPlan.CreatedByDietitianId != dietitian.Id)
            {
                return new Response<DietPlanResponseDto>(ResponseCode.Forbidden, "You don't have permission to update this diet plan.");
            }

            dietPlan.Title = dietPlanDto.Title;
            dietPlan.Description = dietPlanDto.Description;
            dietPlan.StartDate = dietPlanDto.StartDate;
            dietPlan.EndDate = dietPlanDto.EndDate;
            dietPlan.TargetWeight = dietPlanDto.TargetWeight;

            await _dietPlanDal.UpdateAsync(dietPlan);

            var updatedDietPlan = await _dietPlanDal.GetByIdWithDetailsAsync(dietPlan.Id);
            var meals = await _mealDal.GetByDietPlanIdAsync(dietPlan.Id);

            var responseDto = MapToDietPlanResponseDto(updatedDietPlan, meals);

            return new Response<DietPlanResponseDto>(ResponseCode.Success, responseDto, "Diet plan updated successfully.");
        }
        public async Task<Response<bool>> DeleteByDietitianAsync(int id)
        {
            var dietitianUserId = _userContextHelper.GetCurrentUserId();
            var dietPlan = await _dietPlanDal.GetByIdWithDetailsAsync(id);
            if (dietPlan == null)
            {
                return new Response<bool>(ResponseCode.NotFound, "Diet plan not found.");
            }

            var dietitian = await _dietitianDal.GetByUserIdAsync(dietitianUserId);
            if (dietitian == null)
            {
                return new Response<bool>(ResponseCode.NotFound, "Dietitian not found.");
            }

            if (dietPlan.CreatedByDietitianId != dietitian.Id)
            {
                return new Response<bool>(ResponseCode.Forbidden, "You don't have permission to delete this diet plan.");
            }

            await _dietPlanDal.DeleteAsync(id);

            return new Response<bool>(ResponseCode.Success, true, "Diet plan deleted successfully.");
        }
        public async Task<Response<IList<DietPlanResponseDto>>> GetByClientUserIdAsync()
        {
            var clientUserId = _userContextHelper.GetCurrentUserId();
            var client = await _clientDal.GetByUserIdAsync(clientUserId);
            if (client == null)
            {
                return new Response<IList<DietPlanResponseDto>>(ResponseCode.NotFound, "Client not found.");
            }

            var dietPlans = await _dietPlanDal.GetByClientIdAsync(client.Id);
            var dietPlanDtos = new List<DietPlanResponseDto>();

            foreach (var dietPlan in dietPlans)
            {
                var meals = await _mealDal.GetByDietPlanIdAsync(dietPlan.Id);
                dietPlanDtos.Add(MapToDietPlanResponseDto(dietPlan, meals));
            }

            return new Response<IList<DietPlanResponseDto>>(ResponseCode.Success, dietPlanDtos, "Client diet plans retrieved successfully.");
        }

        public async Task<Response<DietPlanResponseDto>> GetByIdForClientAsync(int dietPlanId)
        {
            var clientUserId = _userContextHelper.GetCurrentUserId();

            var client = await _clientDal.GetByUserIdAsync(clientUserId);
            if (client == null)
            {
                return new Response<DietPlanResponseDto>(ResponseCode.NotFound, "Client not found.");
            }

            var dietPlan = await _dietPlanDal.GetByIdWithDetailsAsync(dietPlanId);
            if (dietPlan == null)
            {
                return new Response<DietPlanResponseDto>(ResponseCode.NotFound, "Diet plan not found.");
            }

            if (dietPlan.ClientId != client.Id)
            {
                return new Response<DietPlanResponseDto>(ResponseCode.Forbidden, "You don't have permission to access this diet plan.");
            }

            var meals = await _mealDal.GetByDietPlanIdAsync(dietPlanId);
            var dietPlanDto = MapToDietPlanResponseDto(dietPlan, meals);

            return new Response<DietPlanResponseDto>(ResponseCode.Success, dietPlanDto, "Diet plan retrieved successfully.");
        }

        private DietPlanResponseDto MapToDietPlanResponseDto(DietPlan dietPlan, IList<Meal> meals)
        {
            var mealDtos = meals.Select(m => new MealResponseDto
            {
                Id = m.Id,
                Title = m.Title,
                StartTime = m.StartTime,
                EndTime = m.EndTime,
                Description = m.Description,
                Contents = m.Contents,
                Calories = m.Calories,
                Proteins = m.Proteins,
                Carbohydrates = m.Carbohydrates,
                Fats = m.Fats,
                DietPlanId = m.DietPlanId,
                CreatedAt = m.CreatedAt,
                UpdatedAt = m.UpdatedAt
            }).ToList();

            return new DietPlanResponseDto
            {
                Id = dietPlan.Id,
                Title = dietPlan.Title,
                Description = dietPlan.Description,
                StartDate = dietPlan.StartDate,
                EndDate = dietPlan.EndDate,
                InitialWeight = dietPlan.InitialWeight,
                TargetWeight = dietPlan.TargetWeight,
                ClientId = dietPlan.ClientId,
                ClientName = $"{dietPlan.Client.User.FirstName} {dietPlan.Client.User.LastName}",
                CreatedByDietitianId = dietPlan.CreatedByDietitianId,
                DietitianName = $"{dietPlan.CreatedByDietitian.User.FirstName} {dietPlan.CreatedByDietitian.User.LastName}",
                Meals = mealDtos,
                CreatedAt = dietPlan.CreatedAt,
                UpdatedAt = dietPlan.UpdatedAt
            };
        }
    }
}