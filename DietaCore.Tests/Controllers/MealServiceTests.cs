using DietaCore.Business.Concrete;
using DietaCore.Business.Utilities.Abstract;
using DietaCore.DataAccess.Abstract;
using DietaCore.Dto.MealDTOs;
using DietaCore.Entities.Concrete;
using DietaCore.Shared.Common.Responses.ComplexTypes;
using Moq;

namespace DietaCore.Tests;

public class MealServiceTests
{
    private readonly Mock<IMealDal> _mockMealDal;
    private readonly Mock<IDietPlanDal> _mockDietPlanDal;
    private readonly Mock<IDietitianDal> _mockDietitianDal;
    private readonly Mock<IClientDal> _mockClientDal;
    private readonly Mock<IUserContextHelper> _mockUserContextHelper;
    private readonly MealService _mealService;

    public MealServiceTests()
    {
        _mockMealDal = new Mock<IMealDal>();
        _mockDietPlanDal = new Mock<IDietPlanDal>();
        _mockDietitianDal = new Mock<IDietitianDal>();
        _mockClientDal = new Mock<IClientDal>();
        _mockUserContextHelper = new Mock<IUserContextHelper>();

        _mealService = new MealService(
            _mockMealDal.Object,
            _mockDietPlanDal.Object,
            _mockClientDal.Object,
            _mockDietitianDal.Object,
            _mockUserContextHelper.Object
        );
    }

    [Fact]
    public async Task GetByDietPlanIdForClientAsync_WhenClientNotFound_ReturnsNotFoundResponse()
    {
        var userId = 1;
        var dietPlanId = 1;

        _mockUserContextHelper.Setup(x => x.GetCurrentUserId()).Returns(userId);
        _mockClientDal.Setup(x => x.GetByUserIdAsync(userId)).ReturnsAsync((Client)null);

        var result = await _mealService.GetByDietPlanIdForClientAsync(dietPlanId);

        Assert.Equal(ResponseCode.NotFound, result.ResponseCode);
        Assert.Equal("Client not found.", result.Message);
    }

    [Fact]
    public async Task GetByDietPlanIdForClientAsync_WhenDietPlanNotFound_ReturnsNotFoundResponse()
    {
        var userId = 1;
        var dietPlanId = 1;
        var client = new Client { Id = 1, UserId = userId };

        _mockUserContextHelper.Setup(x => x.GetCurrentUserId()).Returns(userId);
        _mockClientDal.Setup(x => x.GetByUserIdAsync(userId)).ReturnsAsync(client);
        _mockDietPlanDal.Setup(x => x.GetByIdAsync(dietPlanId)).ReturnsAsync((DietPlan)null);

        var result = await _mealService.GetByDietPlanIdForClientAsync(dietPlanId);

        Assert.Equal(ResponseCode.NotFound, result.ResponseCode);
        Assert.Equal("Diet plan not found.", result.Message);
    }

    [Fact]
    public async Task GetByDietPlanIdForClientAsync_WhenClientNotOwnerOfDietPlan_ReturnsForbiddenResponse()
    {
        var userId = 1;
        var dietPlanId = 1;
        var client = new Client { Id = 1, UserId = userId };
        var dietPlan = new DietPlan { Id = dietPlanId, ClientId = 2 };

        _mockUserContextHelper.Setup(x => x.GetCurrentUserId()).Returns(userId);
        _mockClientDal.Setup(x => x.GetByUserIdAsync(userId)).ReturnsAsync(client);
        _mockDietPlanDal.Setup(x => x.GetByIdAsync(dietPlanId)).ReturnsAsync(dietPlan);

        var result = await _mealService.GetByDietPlanIdForClientAsync(dietPlanId);

        Assert.Equal(ResponseCode.Forbidden, result.ResponseCode);
        Assert.Equal("You don't have permission to access meals from this diet plan.", result.Message);
    }

    [Fact]
    public async Task GetByDietPlanIdForClientAsync_WhenValidRequest_ReturnsSuccessWithMeals()
    {
        var userId = 1;
        var dietPlanId = 1;
        var client = new Client { Id = 1, UserId = userId };
        var dietPlan = new DietPlan { Id = dietPlanId, ClientId = 1 };
        var meals = new List<Meal>
        {
            new Meal
            {
                Id = 1,
                Title = "Breakfast",
                StartTime = new TimeSpan(8, 0, 0),
                EndTime = new TimeSpan(9, 0, 0),
                Description = "Morning meal",
                Contents = "Oatmeal",
                Calories = 300,
                Proteins = 10,
                Carbohydrates = 50,
                Fats = 5,
                DietPlanId = dietPlanId,
                CreatedAt = DateTime.Now
            }
        };

        _mockUserContextHelper.Setup(x => x.GetCurrentUserId()).Returns(userId);
        _mockClientDal.Setup(x => x.GetByUserIdAsync(userId)).ReturnsAsync(client);
        _mockDietPlanDal.Setup(x => x.GetByIdAsync(dietPlanId)).ReturnsAsync(dietPlan);
        _mockMealDal.Setup(x => x.GetByDietPlanIdAsync(dietPlanId)).ReturnsAsync(meals);

        var result = await _mealService.GetByDietPlanIdForClientAsync(dietPlanId);

        Assert.Equal(ResponseCode.Success, result.ResponseCode);
        Assert.Single(result.Data);
        Assert.Equal("Breakfast", result.Data.First().Title);
        Assert.Equal("Meals retrieved successfully.", result.Message);
    }

    [Fact]
    public async Task GetByIdForDietitianAsync_WhenMealNotFound_ReturnsNotFoundResponse()
    {
        var mealId = 1;
        var dietitianUserId = 1;

        _mockUserContextHelper.Setup(x => x.GetCurrentUserId()).Returns(dietitianUserId);
        _mockMealDal.Setup(x => x.GetByIdAsync(mealId)).ReturnsAsync((Meal)null);

        var result = await _mealService.GetByIdForDietitianAsync(mealId);

        Assert.Equal(ResponseCode.NotFound, result.ResponseCode);
        Assert.Equal("Meal not found.", result.Message);
    }

    [Fact]
    public async Task GetByIdForDietitianAsync_WhenDietitianNotAuthorized_ReturnsForbiddenResponse()
    {
        var mealId = 1;
        var dietitianUserId = 1;
        var meal = new Meal { Id = mealId, DietPlanId = 1 };
        var dietPlan = new DietPlan { Id = 1, CreatedByDietitianId = 2 };
        var dietitian = new Dietitian { Id = 1, UserId = dietitianUserId };

        _mockUserContextHelper.Setup(x => x.GetCurrentUserId()).Returns(dietitianUserId);
        _mockMealDal.Setup(x => x.GetByIdAsync(mealId)).ReturnsAsync(meal);
        _mockDietPlanDal.Setup(x => x.GetByIdWithDetailsAsync(meal.DietPlanId)).ReturnsAsync(dietPlan);
        _mockDietitianDal.Setup(x => x.GetByUserIdAsync(dietitianUserId)).ReturnsAsync(dietitian);

        var result = await _mealService.GetByIdForDietitianAsync(mealId);

        Assert.Equal(ResponseCode.Forbidden, result.ResponseCode);
        Assert.Equal("You don't have permission to access this meal.", result.Message);
    }

    [Fact]
    public async Task GetByIdForDietitianAsync_WhenValidRequest_ReturnsSuccessWithMeal()
    {
        var mealId = 1;
        var dietitianUserId = 1;
        var meal = new Meal
        {
            Id = mealId,
            Title = "Lunch",
            StartTime = new TimeSpan(12, 0, 0),
            EndTime = new TimeSpan(13, 0, 0),
            Description = "Midday meal",
            Contents = "Chicken salad",
            Calories = 400,
            Proteins = 30,
            Carbohydrates = 20,
            Fats = 15,
            DietPlanId = 1,
            CreatedAt = DateTime.Now
        };
        var dietPlan = new DietPlan { Id = 1, CreatedByDietitianId = 1 };
        var dietitian = new Dietitian { Id = 1, UserId = dietitianUserId };

        _mockUserContextHelper.Setup(x => x.GetCurrentUserId()).Returns(dietitianUserId);
        _mockMealDal.Setup(x => x.GetByIdAsync(mealId)).ReturnsAsync(meal);
        _mockDietPlanDal.Setup(x => x.GetByIdWithDetailsAsync(meal.DietPlanId)).ReturnsAsync(dietPlan);
        _mockDietitianDal.Setup(x => x.GetByUserIdAsync(dietitianUserId)).ReturnsAsync(dietitian);

        var result = await _mealService.GetByIdForDietitianAsync(mealId);

        Assert.Equal(ResponseCode.Success, result.ResponseCode);
        Assert.Equal("Lunch", result.Data.Title);
        Assert.Equal("Meal retrieved successfully.", result.Message);
    }

    [Fact]
    public async Task CreateByDietitianAsync_WhenDietPlanNotFound_ReturnsNotFoundResponse()
    {
        var dietitianUserId = 1;
        var mealRequest = new MealRequestDto
        {
            Title = "Dinner",
            StartTime = new TimeSpan(19, 0, 0),
            EndTime = new TimeSpan(20, 0, 0),
            DietPlanId = 1
        };

        _mockUserContextHelper.Setup(x => x.GetCurrentUserId()).Returns(dietitianUserId);
        _mockDietPlanDal.Setup(x => x.GetByIdWithDetailsAsync(mealRequest.DietPlanId)).ReturnsAsync((DietPlan)null);

        var result = await _mealService.CreateByDietitianAsync(mealRequest);

        Assert.Equal(ResponseCode.NotFound, result.ResponseCode);
        Assert.Equal("Diet plan not found.", result.Message);
    }

    [Fact]
    public async Task CreateByDietitianAsync_WhenEndTimeBeforeStartTime_ReturnsBadRequestResponse()
    {
        var dietitianUserId = 1;
        var mealRequest = new MealRequestDto
        {
            Title = "Dinner",
            StartTime = new TimeSpan(20, 0, 0),
            EndTime = new TimeSpan(19, 0, 0),
            DietPlanId = 1
        };
        var dietPlan = new DietPlan { Id = 1, CreatedByDietitianId = 1 };
        var dietitian = new Dietitian { Id = 1, UserId = dietitianUserId };

        _mockUserContextHelper.Setup(x => x.GetCurrentUserId()).Returns(dietitianUserId);
        _mockDietPlanDal.Setup(x => x.GetByIdWithDetailsAsync(mealRequest.DietPlanId)).ReturnsAsync(dietPlan);
        _mockDietitianDal.Setup(x => x.GetByUserIdAsync(dietitianUserId)).ReturnsAsync(dietitian);

        var result = await _mealService.CreateByDietitianAsync(mealRequest);

        Assert.Equal(ResponseCode.BadRequest, result.ResponseCode);
        Assert.Equal("End time must be after start time.", result.Message);
    }

    [Fact]
    public async Task CreateByDietitianAsync_WhenValidRequest_ReturnsSuccessWithCreatedMeal()
    {
        var dietitianUserId = 1;
        var mealRequest = new MealRequestDto
        {
            Title = "Dinner",
            StartTime = new TimeSpan(19, 0, 0),
            EndTime = new TimeSpan(20, 0, 0),
            Description = "Evening meal",
            Contents = "Grilled fish",
            Calories = 500,
            Proteins = 40,
            Carbohydrates = 30,
            Fats = 20,
            DietPlanId = 1
        };
        var dietPlan = new DietPlan { Id = 1, CreatedByDietitianId = 1 };
        var dietitian = new Dietitian { Id = 1, UserId = dietitianUserId };
        var createdMeal = new Meal
        {
            Id = 1,
            Title = mealRequest.Title,
            StartTime = mealRequest.StartTime,
            EndTime = mealRequest.EndTime,
            Description = mealRequest.Description,
            Contents = mealRequest.Contents,
            Calories = mealRequest.Calories,
            Proteins = mealRequest.Proteins,
            Carbohydrates = mealRequest.Carbohydrates,
            Fats = mealRequest.Fats,
            DietPlanId = mealRequest.DietPlanId,
            CreatedAt = DateTime.Now
        };

        _mockUserContextHelper.Setup(x => x.GetCurrentUserId()).Returns(dietitianUserId);
        _mockDietPlanDal.Setup(x => x.GetByIdWithDetailsAsync(mealRequest.DietPlanId)).ReturnsAsync(dietPlan);
        _mockDietitianDal.Setup(x => x.GetByUserIdAsync(dietitianUserId)).ReturnsAsync(dietitian);
        _mockMealDal.Setup(x => x.AddAsync(It.IsAny<Meal>())).Callback<Meal>(m => m.Id = 1).Returns(Task.FromResult(It.IsAny<Meal>()));
        _mockMealDal.Setup(x => x.GetByIdAsync(1)).ReturnsAsync(createdMeal);

        var result = await _mealService.CreateByDietitianAsync(mealRequest);

        Assert.Equal(ResponseCode.Success, result.ResponseCode);
        Assert.Equal("Dinner", result.Data.Title);
        Assert.Equal("Meal created successfully.", result.Message);
        _mockMealDal.Verify(x => x.AddAsync(It.IsAny<Meal>()), Times.Once);
    }

    [Fact]
    public async Task UpdateByDietitianAsync_WhenMealNotFound_ReturnsNotFoundResponse()
    {
        var dietitianUserId = 1;
        var mealUpdate = new MealUpdateDto
        {
            Id = 1,
            Title = "Updated Meal",
            StartTime = new TimeSpan(8, 0, 0),
            EndTime = new TimeSpan(9, 0, 0)
        };

        _mockUserContextHelper.Setup(x => x.GetCurrentUserId()).Returns(dietitianUserId);
        _mockMealDal.Setup(x => x.GetByIdAsync(mealUpdate.Id)).ReturnsAsync((Meal)null);

        var result = await _mealService.UpdateByDietitianAsync(mealUpdate);

        Assert.Equal(ResponseCode.NotFound, result.ResponseCode);
        Assert.Equal("Meal not found.", result.Message);
    }

    [Fact]
    public async Task UpdateByDietitianAsync_WhenValidRequest_ReturnsSuccessWithUpdatedMeal()
    {
        var dietitianUserId = 1;
        var mealUpdate = new MealUpdateDto
        {
            Id = 1,
            Title = "Updated Meal",
            StartTime = new TimeSpan(8, 0, 0),
            EndTime = new TimeSpan(9, 0, 0),
            Description = "Updated description",
            Contents = "Updated contents",
            Calories = 350,
            Proteins = 15,
            Carbohydrates = 45,
            Fats = 10
        };
        var existingMeal = new Meal { Id = 1, DietPlanId = 1 };
        var dietPlan = new DietPlan { Id = 1, CreatedByDietitianId = 1 };
        var dietitian = new Dietitian { Id = 1, UserId = dietitianUserId };
        var updatedMeal = new Meal
        {
            Id = 1,
            Title = mealUpdate.Title,
            StartTime = mealUpdate.StartTime,
            EndTime = mealUpdate.EndTime,
            Description = mealUpdate.Description,
            Contents = mealUpdate.Contents,
            Calories = mealUpdate.Calories,
            Proteins = mealUpdate.Proteins,
            Carbohydrates = mealUpdate.Carbohydrates,
            Fats = mealUpdate.Fats,
            DietPlanId = 1,
            CreatedAt = DateTime.Now.AddDays(-1),
            UpdatedAt = DateTime.Now
        };

        _mockUserContextHelper.Setup(x => x.GetCurrentUserId()).Returns(dietitianUserId);
        _mockMealDal.Setup(x => x.GetByIdAsync(mealUpdate.Id)).ReturnsAsync(existingMeal);
        _mockDietPlanDal.Setup(x => x.GetByIdWithDetailsAsync(existingMeal.DietPlanId)).ReturnsAsync(dietPlan);
        _mockDietitianDal.Setup(x => x.GetByUserIdAsync(dietitianUserId)).ReturnsAsync(dietitian);
        _mockMealDal.Setup(x => x.UpdateAsync(It.IsAny<Meal>())).Returns(Task.CompletedTask);
        _mockMealDal.SetupSequence(x => x.GetByIdAsync(1)).ReturnsAsync(existingMeal).ReturnsAsync(updatedMeal);

        var result = await _mealService.UpdateByDietitianAsync(mealUpdate);

        Assert.Equal(ResponseCode.Success, result.ResponseCode);
        Assert.Equal("Updated Meal", result.Data.Title);
        Assert.Equal("Meal updated successfully.", result.Message);
        _mockMealDal.Verify(x => x.UpdateAsync(It.IsAny<Meal>()), Times.Once);
    }

    [Fact]
    public async Task DeleteByDietitianAsync_WhenMealNotFound_ReturnsNotFoundResponse()
    {
        var dietitianUserId = 1;
        var mealId = 1;

        _mockUserContextHelper.Setup(x => x.GetCurrentUserId()).Returns(dietitianUserId);
        _mockMealDal.Setup(x => x.GetByIdAsync(mealId)).ReturnsAsync((Meal)null);

        var result = await _mealService.DeleteByDietitianAsync(mealId);

        Assert.Equal(ResponseCode.NotFound, result.ResponseCode);
        Assert.Equal("Meal not found.", result.Message);
    }

    [Fact]
    public async Task DeleteByDietitianAsync_WhenDietitianNotAuthorized_ReturnsForbiddenResponse()
    {
        var dietitianUserId = 1;
        var mealId = 1;
        var meal = new Meal { Id = mealId, DietPlanId = 1 };
        var dietPlan = new DietPlan { Id = 1, CreatedByDietitianId = 2 };
        var dietitian = new Dietitian { Id = 1, UserId = dietitianUserId };

        _mockUserContextHelper.Setup(x => x.GetCurrentUserId()).Returns(dietitianUserId);
        _mockMealDal.Setup(x => x.GetByIdAsync(mealId)).ReturnsAsync(meal);
        _mockDietPlanDal.Setup(x => x.GetByIdWithDetailsAsync(meal.DietPlanId)).ReturnsAsync(dietPlan);
        _mockDietitianDal.Setup(x => x.GetByUserIdAsync(dietitianUserId)).ReturnsAsync(dietitian);

        var result = await _mealService.DeleteByDietitianAsync(mealId);

        Assert.Equal(ResponseCode.Forbidden, result.ResponseCode);
        Assert.Equal("You don't have permission to delete this meal.", result.Message);
    }

    [Fact]
    public async Task DeleteByDietitianAsync_WhenValidRequest_ReturnsSuccessResponse()
    {
        var dietitianUserId = 1;
        var mealId = 1;
        var meal = new Meal { Id = mealId, DietPlanId = 1 };
        var dietPlan = new DietPlan { Id = 1, CreatedByDietitianId = 1 };
        var dietitian = new Dietitian { Id = 1, UserId = dietitianUserId };

        _mockUserContextHelper.Setup(x => x.GetCurrentUserId()).Returns(dietitianUserId);
        _mockMealDal.Setup(x => x.GetByIdAsync(mealId)).ReturnsAsync(meal);
        _mockDietPlanDal.Setup(x => x.GetByIdWithDetailsAsync(meal.DietPlanId)).ReturnsAsync(dietPlan);
        _mockDietitianDal.Setup(x => x.GetByUserIdAsync(dietitianUserId)).ReturnsAsync(dietitian);
        _mockMealDal.Setup(x => x.DeleteAsync(mealId)).Returns(Task.CompletedTask);

        var result = await _mealService.DeleteByDietitianAsync(mealId);

        Assert.Equal(ResponseCode.Success, result.ResponseCode);
        Assert.True(result.Data);
        Assert.Equal("Meal deleted successfully.", result.Message);
        _mockMealDal.Verify(x => x.DeleteAsync(mealId), Times.Once);
    }
}