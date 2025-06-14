using DietaCore.Business.Abstract;
using DietaCore.Business.Utilities.Abstract;
using DietaCore.DataAccess.Abstract;
using DietaCore.DataAccess.Concrete.EntityFramewokCore;
using DietaCore.Dto.MealDTOs;
using DietaCore.Entities.Concrete;
using DietaCore.Shared.Common.Responses.ComplexTypes;
using DietaCore.Shared.Common.Responses.Concrete;
using Microsoft.AspNetCore.Http;

namespace DietaCore.Business.Concrete
{
    public class MealService : IMealService
    {
        private readonly IMealDal _mealDal;
        private readonly IDietPlanDal _dietPlanDal;
        private readonly IDietitianDal _dietitianDal;
        private readonly IClientDal _clientDal;
        private readonly IUserContextHelper _userContextHelper;

        public MealService(IMealDal mealDal, IDietPlanDal dietPlanDal, IClientDal clientDal,
            IDietitianDal dietitianDal, IUserContextHelper userContextHelper)
        {
            _mealDal = mealDal;
            _dietPlanDal = dietPlanDal;
            _clientDal = clientDal;
            _dietitianDal = dietitianDal;
            _userContextHelper = userContextHelper;
        }

        public async Task<Response<IList<MealResponseDto>>> GetByDietPlanIdForClientAsync(int dietPlanId)
        {
            var userId = _userContextHelper.GetCurrentUserId();

            var client = await _clientDal.GetByUserIdAsync(userId);
            if (client == null)
            {
                return new Response<IList<MealResponseDto>>(ResponseCode.NotFound, "Client not found.");
            }

            var dietPlan = await _dietPlanDal.GetByIdAsync(dietPlanId);
            if (dietPlan == null)
            {
                return new Response<IList<MealResponseDto>>(ResponseCode.NotFound, "Diet plan not found.");
            }

            if (dietPlan.ClientId != client.Id)
            {
                return new Response<IList<MealResponseDto>>(ResponseCode.Forbidden, "You don't have permission to access meals from this diet plan.");
            }

            var meals = await _mealDal.GetByDietPlanIdAsync(dietPlanId);

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

            return new Response<IList<MealResponseDto>>(ResponseCode.Success, mealDtos, "Meals retrieved successfully.");
        }

        public async Task<Response<MealResponseDto>> GetByIdForDietitianAsync(int id)
        {
            var dietitianUserId = _userContextHelper.GetCurrentUserId();

            var meal = await _mealDal.GetByIdAsync(id);
            if (meal == null)
            {
                return new Response<MealResponseDto>(ResponseCode.NotFound, "Meal not found.");
            }

            var dietPlan = await _dietPlanDal.GetByIdWithDetailsAsync(meal.DietPlanId);
            if (dietPlan == null)
            {
                return new Response<MealResponseDto>(ResponseCode.NotFound, "Diet plan not found.");
            }

            var dietitian = await _dietitianDal.GetByUserIdAsync(dietitianUserId);
            if (dietitian == null)
            {
                return new Response<MealResponseDto>(ResponseCode.NotFound, "Dietitian not found.");
            }

            if (dietPlan.CreatedByDietitianId != dietitian.Id)
            {
                return new Response<MealResponseDto>(ResponseCode.Forbidden, "You don't have permission to access this meal.");
            }

            var mealDto = new MealResponseDto
            {
                Id = meal.Id,
                Title = meal.Title,
                StartTime = meal.StartTime,
                EndTime = meal.EndTime,
                Description = meal.Description,
                Contents = meal.Contents,
                Calories = meal.Calories,
                Proteins = meal.Proteins,
                Carbohydrates = meal.Carbohydrates,
                Fats = meal.Fats,
                DietPlanId = meal.DietPlanId,
                CreatedAt = meal.CreatedAt,
                UpdatedAt = meal.UpdatedAt
            };

            return new Response<MealResponseDto>(ResponseCode.Success, mealDto, "Meal retrieved successfully.");
        }

        public async Task<Response<IList<MealResponseDto>>> GetByDietPlanIdForDietitianAsync(int dietPlanId)
        {
            var dietitianUserId = _userContextHelper.GetCurrentUserId();

            var dietPlan = await _dietPlanDal.GetByIdWithDetailsAsync(dietPlanId);
            if (dietPlan == null)
            {
                return new Response<IList<MealResponseDto>>(ResponseCode.NotFound, "Diet plan not found.");
            }

            var dietitian = await _dietitianDal.GetByUserIdAsync(dietitianUserId);
            if (dietitian == null)
            {
                return new Response<IList<MealResponseDto>>(ResponseCode.NotFound, "Dietitian not found.");
            }

            if (dietPlan.CreatedByDietitianId != dietitian.Id)
            {
                return new Response<IList<MealResponseDto>>(ResponseCode.Forbidden, "You don't have permission to access meals from this diet plan.");
            }

            var meals = await _mealDal.GetByDietPlanIdAsync(dietPlanId);

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

            return new Response<IList<MealResponseDto>>(ResponseCode.Success, mealDtos, "Meals retrieved successfully.");
        }

        public async Task<Response<MealResponseDto>> CreateByDietitianAsync(MealRequestDto mealDto)
        {
            var dietitianUserId = _userContextHelper.GetCurrentUserId();

            var dietPlan = await _dietPlanDal.GetByIdWithDetailsAsync(mealDto.DietPlanId);
            if (dietPlan == null)
            {
                return new Response<MealResponseDto>(ResponseCode.NotFound, "Diet plan not found.");
            }

            var dietitian = await _dietitianDal.GetByUserIdAsync(dietitianUserId);
            if (dietitian == null)
            {
                return new Response<MealResponseDto>(ResponseCode.NotFound, "Dietitian not found.");
            }

            if (dietPlan.CreatedByDietitianId != dietitian.Id)
            {
                return new Response<MealResponseDto>(ResponseCode.Forbidden, "You don't have permission to create meal for this diet plan.");
            }

            if (mealDto.EndTime <= mealDto.StartTime)
            {
                return new Response<MealResponseDto>(ResponseCode.BadRequest, "End time must be after start time.");
            }

            var meal = new Meal
            {
                Title = mealDto.Title,
                StartTime = mealDto.StartTime,
                EndTime = mealDto.EndTime,
                Description = mealDto.Description,
                Contents = mealDto.Contents,
                Calories = mealDto.Calories,
                Proteins = mealDto.Proteins,
                Carbohydrates = mealDto.Carbohydrates,
                Fats = mealDto.Fats,
                DietPlanId = mealDto.DietPlanId
            };

            await _mealDal.AddAsync(meal);

            var createdMeal = await _mealDal.GetByIdAsync(meal.Id);

            var responseDto = new MealResponseDto
            {
                Id = createdMeal.Id,
                Title = createdMeal.Title,
                StartTime = createdMeal.StartTime,
                EndTime = createdMeal.EndTime,
                Description = createdMeal.Description,
                Contents = createdMeal.Contents,
                Calories = createdMeal.Calories,
                Proteins = createdMeal.Proteins,
                Carbohydrates = createdMeal.Carbohydrates,
                Fats = createdMeal.Fats,
                DietPlanId = createdMeal.DietPlanId,
                CreatedAt = createdMeal.CreatedAt,
                UpdatedAt = createdMeal.UpdatedAt
            };

            return new Response<MealResponseDto>(ResponseCode.Success, responseDto, "Meal created successfully.");
        }

        public async Task<Response<MealResponseDto>> UpdateByDietitianAsync(MealUpdateDto mealDto)
        {
            var dietitianUserId = _userContextHelper.GetCurrentUserId();

            var meal = await _mealDal.GetByIdAsync(mealDto.Id);
            if (meal == null)
            {
                return new Response<MealResponseDto>(ResponseCode.NotFound, "Meal not found.");
            }

            var dietPlan = await _dietPlanDal.GetByIdWithDetailsAsync(meal.DietPlanId);
            if (dietPlan == null)
            {
                return new Response<MealResponseDto>(ResponseCode.NotFound, "Diet plan not found.");
            }

            var dietitian = await _dietitianDal.GetByUserIdAsync(dietitianUserId);
            if (dietitian == null)
            {
                return new Response<MealResponseDto>(ResponseCode.NotFound, "Dietitian not found.");
            }

            if (dietPlan.CreatedByDietitianId != dietitian.Id)
            {
                return new Response<MealResponseDto>(ResponseCode.Forbidden, "You don't have permission to update this meal.");
            }

            if (mealDto.EndTime <= mealDto.StartTime)
            {
                return new Response<MealResponseDto>(ResponseCode.BadRequest, "End time must be after start time.");
            }

            meal.Title = mealDto.Title;
            meal.StartTime = mealDto.StartTime;
            meal.EndTime = mealDto.EndTime;
            meal.Description = mealDto.Description;
            meal.Contents = mealDto.Contents;
            meal.Calories = mealDto.Calories;
            meal.Proteins = mealDto.Proteins;
            meal.Carbohydrates = mealDto.Carbohydrates;
            meal.Fats = mealDto.Fats;

            await _mealDal.UpdateAsync(meal);

            var updatedMeal = await _mealDal.GetByIdAsync(meal.Id);

            var responseDto = new MealResponseDto
            {
                Id = updatedMeal.Id,
                Title = updatedMeal.Title,
                StartTime = updatedMeal.StartTime,
                EndTime = updatedMeal.EndTime,
                Description = updatedMeal.Description,
                Contents = updatedMeal.Contents,
                Calories = updatedMeal.Calories,
                Proteins = updatedMeal.Proteins,
                Carbohydrates = updatedMeal.Carbohydrates,
                Fats = updatedMeal.Fats,
                DietPlanId = updatedMeal.DietPlanId,
                CreatedAt = updatedMeal.CreatedAt,
                UpdatedAt = updatedMeal.UpdatedAt
            };

            return new Response<MealResponseDto>(ResponseCode.Success, responseDto, "Meal updated successfully.");
        }

        public async Task<Response<bool>> DeleteByDietitianAsync(int id)
        {
            var dietitianUserId = _userContextHelper.GetCurrentUserId();

            var meal = await _mealDal.GetByIdAsync(id);
            if (meal == null)
            {
                return new Response<bool>(ResponseCode.NotFound, "Meal not found.");
            }

            var dietPlan = await _dietPlanDal.GetByIdWithDetailsAsync(meal.DietPlanId);
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
                return new Response<bool>(ResponseCode.Forbidden, "You don't have permission to delete this meal.");
            }

            await _mealDal.DeleteAsync(id);

            return new Response<bool>(ResponseCode.Success, true, "Meal deleted successfully.");
        }
    }
}
