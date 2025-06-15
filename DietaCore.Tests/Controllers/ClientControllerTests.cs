using DietaCore.Api.Controllers;
using DietaCore.Business.Abstract;
using DietaCore.Dto.ClientDTOs;
using DietaCore.Dto.DietPlanDTOs;
using DietaCore.Dto.MealDTOs;
using DietaCore.Shared.Common.Responses.ComplexTypes;
using DietaCore.Shared.Common.Responses.Concrete;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;
using System.Security.Claims;

namespace DietaCore.Tests;

public class ClientControllerTests
{
    private readonly Mock<IClientService> _clientServiceMock;
    private readonly Mock<IDietPlanService> _dietPlanServiceMock;
    private readonly Mock<IMealService> _mealServiceMock;
    private readonly ClientController _controller;

    public ClientControllerTests()
    {
        _clientServiceMock = new Mock<IClientService>();
        _dietPlanServiceMock = new Mock<IDietPlanService>();
        _mealServiceMock = new Mock<IMealService>();

        _controller = new ClientController(
            _clientServiceMock.Object,
            _dietPlanServiceMock.Object,
            _mealServiceMock.Object);

        SetupControllerContext();
    }

    private void SetupControllerContext()
    {
        var claims = new List<Claim>
        {
            new Claim(ClaimTypes.NameIdentifier, "test-user-id"),
            new Claim(ClaimTypes.Email, "test@example.com"),
            new Claim(ClaimTypes.Role, "Client")
        };

        var identity = new ClaimsIdentity(claims, "TestAuthType");
        var principal = new ClaimsPrincipal(identity);

        _controller.ControllerContext = new ControllerContext()
        {
            HttpContext = new DefaultHttpContext()
            {
                User = principal
            }
        };
    }

    [Fact]
    public async Task GetProfile_WhenClientExists_ReturnsSuccessResponse()
    {
        var expectedClient = new ClientResponseDto
        {
            Id = 1,
            FirstName = "John",
            LastName = "Doe",
            Email = "john.doe@example.com",
            PhoneNumber = "1234567890",
            Age = 30,
            Gender = "Male",
            Height = 175.5m,
            InitialWeight = 80.0m,
            CurrentWeight = 78.0m
        };

        var serviceResponse = new Response<ClientResponseDto>(ResponseCode.Success, expectedClient, "Client profile retrieved successfully.");

        _clientServiceMock.Setup(x => x.GetClientProfileAsync()).ReturnsAsync(serviceResponse);

        var result = await _controller.GetProfile();

        result.Should().NotBeNull();
        result.ResponseCode.Should().Be(ResponseCode.Success);
        result.Data.Should().NotBeNull();
        result.Data.Id.Should().Be(expectedClient.Id);
        result.Data.FirstName.Should().Be(expectedClient.FirstName);
        result.Data.LastName.Should().Be(expectedClient.LastName);
        result.Data.Email.Should().Be(expectedClient.Email);
        result.Message.Should().Be("Client profile retrieved successfully.");

        _clientServiceMock.Verify(x => x.GetClientProfileAsync(), Times.Once);
    }

    [Fact]
    public async Task GetProfile_WhenClientNotFound_ReturnsNotFoundResponse()
    {
        var serviceResponse = new Response<ClientResponseDto>(ResponseCode.NotFound, "Client not found.");

        _clientServiceMock.Setup(x => x.GetClientProfileAsync()).ReturnsAsync(serviceResponse);

        var result = await _controller.GetProfile();

        result.Should().NotBeNull();
        result.ResponseCode.Should().Be(ResponseCode.NotFound);
        result.Data.Should().BeNull();
        result.Message.Should().Be("Client not found.");

        _clientServiceMock.Verify(x => x.GetClientProfileAsync(), Times.Once);
    }

    [Fact]
    public async Task GetDietPlans_WhenDietPlansExist_ReturnsSuccessResponse()
    {
        var expectedDietPlans = new List<DietPlanResponseDto>
            {
                new DietPlanResponseDto
                {
                    Id = 1,
                    Title = "Weight Loss Plan",
                    Description = "A plan to lose weight",
                    StartDate = DateTime.Now,
                    EndDate = DateTime.Now.AddDays(30),
                    InitialWeight = 80.0m,
                    TargetWeight = 75.0m,
                    ClientId = 1,
                    ClientName = "John Doe",
                    CreatedByDietitianId = 1,
                    DietitianName = "Dr. Smith",
                    Meals = new List<MealResponseDto>()
                },
                new DietPlanResponseDto
                {
                    Id = 2,
                    Title = "Muscle Gain Plan",
                    Description = "A plan to gain muscle",
                    StartDate = DateTime.Now.AddDays(30),
                    EndDate = DateTime.Now.AddDays(60),
                    InitialWeight = 75.0m,
                    TargetWeight = 80.0m,
                    ClientId = 1,
                    ClientName = "John Doe",
                    CreatedByDietitianId = 1,
                    DietitianName = "Dr. Smith",
                    Meals = new List<MealResponseDto>()
                }
            };

        var serviceResponse = new Response<IList<DietPlanResponseDto>>(ResponseCode.Success, expectedDietPlans, "Diet plans retrieved successfully.");

        _dietPlanServiceMock.Setup(x => x.GetByClientUserIdAsync()).ReturnsAsync(serviceResponse);

        var result = await _controller.GetDietPlans();

        result.Should().NotBeNull();
        result.ResponseCode.Should().Be(ResponseCode.Success);
        result.Data.Should().NotBeNull();
        result.Data.Should().HaveCount(2);
        result.Data.First().Title.Should().Be("Weight Loss Plan");
        result.Data.Last().Title.Should().Be("Muscle Gain Plan");
        result.Message.Should().Be("Diet plans retrieved successfully.");

        _dietPlanServiceMock.Verify(x => x.GetByClientUserIdAsync(), Times.Once);
    }

    [Fact]
    public async Task GetDietPlans_WhenNoDietPlansExist_ReturnsEmptyList()
    {
        var emptyDietPlans = new List<DietPlanResponseDto>();
        var serviceResponse = new Response<IList<DietPlanResponseDto>>(ResponseCode.Success, emptyDietPlans, "No diet plans found.");

        _dietPlanServiceMock.Setup(x => x.GetByClientUserIdAsync()).ReturnsAsync(serviceResponse);

        var result = await _controller.GetDietPlans();

        result.Should().NotBeNull();
        result.ResponseCode.Should().Be(ResponseCode.Success);
        result.Data.Should().NotBeNull();
        result.Data.Should().BeEmpty();

        _dietPlanServiceMock.Verify(x => x.GetByClientUserIdAsync(), Times.Once);
    }

    [Fact]
    public async Task GetDietPlanById_WhenDietPlanExists_ReturnsSuccessResponse()
    {
        int dietPlanId = 1;
        var expectedDietPlan = new DietPlanResponseDto
        {
            Id = dietPlanId,
            Title = "Weight Loss Plan",
            Description = "A comprehensive weight loss plan",
            StartDate = DateTime.Now,
            EndDate = DateTime.Now.AddDays(30),
            InitialWeight = 80.0m,
            TargetWeight = 75.0m,
            ClientId = 1,
            ClientName = "John Doe",
            CreatedByDietitianId = 1,
            DietitianName = "Dr. Smith",
            Meals = new List<MealResponseDto>()
        };

        var serviceResponse = new Response<DietPlanResponseDto>(ResponseCode.Success, expectedDietPlan, "Diet plan retrieved successfully.");

        _dietPlanServiceMock.Setup(x => x.GetByIdForClientAsync(dietPlanId)).ReturnsAsync(serviceResponse);

        var result = await _controller.GetDietPlanById(dietPlanId);

        result.Should().NotBeNull();
        result.ResponseCode.Should().Be(ResponseCode.Success);
        result.Data.Should().NotBeNull();
        result.Data.Id.Should().Be(dietPlanId);
        result.Data.Title.Should().Be("Weight Loss Plan");
        result.Message.Should().Be("Diet plan retrieved successfully.");

        _dietPlanServiceMock.Verify(x => x.GetByIdForClientAsync(dietPlanId), Times.Once);
    }

    [Fact]
    public async Task GetDietPlanById_WhenDietPlanNotFound_ReturnsNotFoundResponse()
    {
        int dietPlanId = 999;
        var serviceResponse = new Response<DietPlanResponseDto>(ResponseCode.NotFound, "Diet plan not found.");

        _dietPlanServiceMock.Setup(x => x.GetByIdForClientAsync(dietPlanId)).ReturnsAsync(serviceResponse);

        var result = await _controller.GetDietPlanById(dietPlanId);

        result.Should().NotBeNull();
        result.ResponseCode.Should().Be(ResponseCode.NotFound);
        result.Data.Should().BeNull();
        result.Message.Should().Be("Diet plan not found.");

        _dietPlanServiceMock.Verify(x => x.GetByIdForClientAsync(dietPlanId), Times.Once);
    }

    [Fact]
    public async Task GetMealsByDietPlanId_WhenMealsExist_ReturnsSuccessResponse()
    {
        int dietPlanId = 1;
        var expectedMeals = new List<MealResponseDto>
            {
                new MealResponseDto
                {
                    Id = 1,
                    Title = "Breakfast",
                    StartTime = new TimeSpan(8, 0, 0),
                    EndTime = new TimeSpan(9, 0, 0),
                    Description = "Healthy breakfast",
                    Contents = "Oatmeal, fruits, nuts",
                    Calories = 350,
                    Proteins = 15.5m,
                    Carbohydrates = 45.0m,
                    Fats = 12.0m,
                    DietPlanId = dietPlanId
                },
                new MealResponseDto
                {
                    Id = 2,
                    Title = "Lunch",
                    StartTime = new TimeSpan(12, 0, 0),
                    EndTime = new TimeSpan(13, 0, 0),
                    Description = "Balanced lunch",
                    Contents = "Grilled chicken, vegetables, rice",
                    Calories = 450,
                    Proteins = 35.0m,
                    Carbohydrates = 40.0m,
                    Fats = 15.0m,
                    DietPlanId = dietPlanId
                }
            };

        var serviceResponse = new Response<IList<MealResponseDto>>(ResponseCode.Success, expectedMeals, "Meals retrieved successfully.");

        _mealServiceMock.Setup(x => x.GetByDietPlanIdForClientAsync(dietPlanId)).ReturnsAsync(serviceResponse);

        var result = await _controller.GetMealsByDietPlanId(dietPlanId);

        result.Should().NotBeNull();
        result.ResponseCode.Should().Be(ResponseCode.Success);
        result.Data.Should().NotBeNull();
        result.Data.Should().HaveCount(2);
        result.Data.First().Title.Should().Be("Breakfast");
        result.Data.Last().Title.Should().Be("Lunch");
        result.Message.Should().Be("Meals retrieved successfully.");

        _mealServiceMock.Verify(x => x.GetByDietPlanIdForClientAsync(dietPlanId), Times.Once);
    }

    [Fact]
    public async Task GetMealsByDietPlanId_WhenNoMealsExist_ReturnsEmptyList()
    {
        int dietPlanId = 1;
        var emptyMeals = new List<MealResponseDto>();
        var serviceResponse = new Response<IList<MealResponseDto>>(ResponseCode.Success, emptyMeals, "No meals found for this diet plan.");

        _mealServiceMock.Setup(x => x.GetByDietPlanIdForClientAsync(dietPlanId)).ReturnsAsync(serviceResponse);

        var result = await _controller.GetMealsByDietPlanId(dietPlanId);

        result.Should().NotBeNull();
        result.ResponseCode.Should().Be(ResponseCode.Success);
        result.Data.Should().NotBeNull();
        result.Data.Should().BeEmpty();

        _mealServiceMock.Verify(x => x.GetByDietPlanIdForClientAsync(dietPlanId), Times.Once);
    }

    [Fact]
    public async Task GetMealsByDietPlanId_WhenUnauthorized_ReturnsForbiddenResponse()
    {
        int dietPlanId = 1;
        var serviceResponse = new Response<IList<MealResponseDto>>(ResponseCode.Forbidden, "You don't have permission to access meals from this diet plan.");

        _mealServiceMock.Setup(x => x.GetByDietPlanIdForClientAsync(dietPlanId)).ReturnsAsync(serviceResponse);

        var result = await _controller.GetMealsByDietPlanId(dietPlanId);

        result.Should().NotBeNull();
        result.ResponseCode.Should().Be(ResponseCode.Forbidden);
        result.Data.Should().BeNull();
        result.Message.Should().Contain("permission");

        _mealServiceMock.Verify(x => x.GetByDietPlanIdForClientAsync(dietPlanId), Times.Once);
    }
}
