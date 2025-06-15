using DietaCore.Api.Controllers;
using DietaCore.Business.Abstract;
using DietaCore.Dto.ClientDTOs;
using DietaCore.Dto.DietitianDTOs;
using DietaCore.Dto.DietPlanDTOs;
using DietaCore.Shared.Common.Responses.ComplexTypes;
using DietaCore.Shared.Common.Responses.Concrete;
using Moq;

namespace DietaCore.Tests;

public class DietitianControllerTests
{
    private readonly Mock<IDietitianService> _mockDietitianService;
    private readonly Mock<IClientService> _mockClientService;
    private readonly Mock<IDietPlanService> _mockDietPlanService;
    private readonly DietitianController _controller;

    public DietitianControllerTests()
    {
        _mockDietitianService = new Mock<IDietitianService>();
        _mockClientService = new Mock<IClientService>();
        _mockDietPlanService = new Mock<IDietPlanService>();

        _controller = new DietitianController(
            _mockDietitianService.Object,
            _mockClientService.Object,
            _mockDietPlanService.Object
        );
    }

    [Fact]
    public async Task GetProfile_ReturnsSuccessResponse_WhenDietitianExists()
    {
        var expectedResponse = new Response<DietitianResponseDto>(ResponseCode.Success,
            new DietitianResponseDto
            {
                Id = 1,
                FirstName = "John",
                LastName = "Doe",
                Email = "john.doe@example.com",
                Specialization = "Weight Management"
            }, "Success"
        );

        _mockDietitianService.Setup(x => x.GetByUserIdAsync()).ReturnsAsync(expectedResponse);

        var result = await _controller.GetProfile();

        Assert.NotNull(result);
        Assert.Equal(ResponseCode.Success, result.ResponseCode);
        Assert.Equal("John", result.Data.FirstName);
        Assert.Equal("Doe", result.Data.LastName);
        _mockDietitianService.Verify(x => x.GetByUserIdAsync(), Times.Once);
    }

    [Fact]
    public async Task GetProfile_ReturnsNotFoundResponse_WhenDietitianNotExists()
    {
        var expectedResponse = new Response<DietitianResponseDto>(ResponseCode.NotFound, "Dietitian not found.");

        _mockDietitianService.Setup(x => x.GetByUserIdAsync()).ReturnsAsync(expectedResponse);

        var result = await _controller.GetProfile();

        Assert.NotNull(result);
        Assert.Equal(ResponseCode.NotFound, result.ResponseCode);
        Assert.Null(result.Data);
        _mockDietitianService.Verify(x => x.GetByUserIdAsync(), Times.Once);
    }

    [Fact]
    public async Task GetClients_ReturnsSuccessResponse_WithClientsList()
    {
        var clientsList = new List<ClientResponseDto>
        {
            new ClientResponseDto
            {
                Id = 1,
                FirstName = "Alice",
                LastName = "Smith",
                Email = "alice@example.com"
            },
            new ClientResponseDto
            {
                Id = 2,
                FirstName = "Bob",
                LastName = "Johnson",
                Email = "bob@example.com"
            }
        };

        var expectedResponse = new Response<IList<ClientResponseDto>>(ResponseCode.Success, clientsList, "Clients retrieved successfully.");

        _mockClientService.Setup(x => x.GetClientsByDietitianAsync()).ReturnsAsync(expectedResponse);

        var result = await _controller.GetClients();

        Assert.NotNull(result);
        Assert.Equal(ResponseCode.Success, result.ResponseCode);
        Assert.Equal(2, result.Data.Count);
        Assert.Equal("Alice", result.Data.First().FirstName);
        _mockClientService.Verify(x => x.GetClientsByDietitianAsync(), Times.Once);
    }

    [Fact]
    public async Task GetClients_ReturnsEmptyList_WhenNoClientsFound()
    {
        var expectedResponse = new Response<IList<ClientResponseDto>>(ResponseCode.Success, new List<ClientResponseDto>(), "No clients found.");

        _mockClientService.Setup(x => x.GetClientsByDietitianAsync()).ReturnsAsync(expectedResponse);

        var result = await _controller.GetClients();

        Assert.NotNull(result);
        Assert.Equal(ResponseCode.Success, result.ResponseCode);
        Assert.Empty(result.Data);
        _mockClientService.Verify(x => x.GetClientsByDietitianAsync(), Times.Once);
    }

    [Fact]
    public async Task GetClientById_ReturnsSuccessResponse_WhenClientExists()
    {
        var clientId = 1;
        var expectedResponse = new Response<ClientResponseDto>(ResponseCode.Success,
            new ClientResponseDto
            {
                Id = clientId,
                FirstName = "Alice",
                LastName = "Smith",
                Email = "alice@example.com"
            }, "Client retrieved successfully."
        );

        _mockClientService.Setup(x => x.GetClientByIdForDietitianAsync(clientId)).ReturnsAsync(expectedResponse);

        var result = await _controller.GetClientById(clientId);

        Assert.NotNull(result);
        Assert.Equal(ResponseCode.Success, result.ResponseCode);
        Assert.Equal(clientId, result.Data.Id);
        Assert.Equal("Alice", result.Data.FirstName);
        _mockClientService.Verify(x => x.GetClientByIdForDietitianAsync(clientId), Times.Once);
    }

    [Fact]
    public async Task GetClientById_ReturnsNotFoundResponse_WhenClientNotExists()
    {
        var clientId = 999;
        var expectedResponse = new Response<ClientResponseDto>(ResponseCode.NotFound, "Client not found.");

        _mockClientService.Setup(x => x.GetClientByIdForDietitianAsync(clientId)).ReturnsAsync(expectedResponse);

        var result = await _controller.GetClientById(clientId);

        Assert.NotNull(result);
        Assert.Equal(ResponseCode.NotFound, result.ResponseCode);
        Assert.Null(result.Data);
        _mockClientService.Verify(x => x.GetClientByIdForDietitianAsync(clientId), Times.Once);
    }

    [Fact]
    public async Task GetClientById_ReturnsForbiddenResponse_WhenUnauthorizedAccess()
    {
        var clientId = 1;
        var expectedResponse = new Response<ClientResponseDto>(ResponseCode.Forbidden, "You don't have permission to access this client.");

        _mockClientService.Setup(x => x.GetClientByIdForDietitianAsync(clientId)).ReturnsAsync(expectedResponse);

        var result = await _controller.GetClientById(clientId);

        Assert.NotNull(result);
        Assert.Equal(ResponseCode.Forbidden, result.ResponseCode);
        Assert.Null(result.Data);
        _mockClientService.Verify(x => x.GetClientByIdForDietitianAsync(clientId), Times.Once);
    }

    [Fact]
    public async Task CreateClient_ReturnsSuccessResponse_WhenValidClientData()
    {
        var clientRequest = new ClientRequestDto
        {
            FirstName = "John",
            LastName = "Doe",
            Email = "john.doe@example.com",
            Password = "Password123!",
            PhoneNumber = "+1234567890",
            DateOfBirth = new DateTime(1990, 1, 1),
            Gender = "Male",
            Height = 180.5m,
            InitialWeight = 80.0m
        };

        var expectedResponse = new Response<ClientResponseDto>(ResponseCode.Success,
            new ClientResponseDto
            {
                Id = 1,
                FirstName = clientRequest.FirstName,
                LastName = clientRequest.LastName,
                Email = clientRequest.Email
            }, "Client created successfully."
        );

        _mockClientService.Setup(x => x.CreateClientForDietitianAsync(It.IsAny<ClientRequestDto>())).ReturnsAsync(expectedResponse);

        var result = await _controller.CreateClient(clientRequest);

        Assert.NotNull(result);
        Assert.Equal(ResponseCode.Success, result.ResponseCode);
        Assert.Equal(clientRequest.FirstName, result.Data.FirstName);
        Assert.Equal(clientRequest.LastName, result.Data.LastName);
        _mockClientService.Verify(x => x.CreateClientForDietitianAsync(It.IsAny<ClientRequestDto>()), Times.Once);
    }

    [Fact]
    public async Task CreateClient_ReturnsBadRequestResponse_WhenInvalidData()
    {
        var clientRequest = new ClientRequestDto
        {
            FirstName = "",
            LastName = "",
            Email = "invalid-email"
        };

        var expectedResponse = new Response<ClientResponseDto>(ResponseCode.BadRequest, "Validation failed.");

        _mockClientService.Setup(x => x.CreateClientForDietitianAsync(It.IsAny<ClientRequestDto>())).ReturnsAsync(expectedResponse);

        var result = await _controller.CreateClient(clientRequest);

        Assert.NotNull(result);
        Assert.Equal(ResponseCode.BadRequest, result.ResponseCode);
        Assert.Null(result.Data);
        _mockClientService.Verify(x => x.CreateClientForDietitianAsync(It.IsAny<ClientRequestDto>()), Times.Once);
    }

    [Fact]
    public async Task UpdateClient_ReturnsSuccessResponse_WhenValidUpdateData()
    {
        var clientUpdate = new ClientUpdateDto
        {
            Id = 1,
            FirstName = "John Updated",
            LastName = "Doe Updated",
            Email = "john.updated@example.com",
            PhoneNumber = "+1234567890",
            DateOfBirth = new DateTime(1990, 1, 1),
            Gender = "Male",
            Height = 180.5m,
            CurrentWeight = 75.0m
        };

        var expectedResponse = new Response<ClientResponseDto>(ResponseCode.Success,
            new ClientResponseDto
            {
                Id = clientUpdate.Id,
                FirstName = clientUpdate.FirstName,
                LastName = clientUpdate.LastName,
                Email = clientUpdate.Email
            }, "Client updated successfully."
        );

        _mockClientService.Setup(x => x.UpdateClientForDietitianAsync(It.IsAny<ClientUpdateDto>())).ReturnsAsync(expectedResponse);

        var result = await _controller.UpdateClient(clientUpdate);

        Assert.NotNull(result);
        Assert.Equal(ResponseCode.Success, result.ResponseCode);
        Assert.Equal(clientUpdate.FirstName, result.Data.FirstName);
        _mockClientService.Verify(x => x.UpdateClientForDietitianAsync(It.IsAny<ClientUpdateDto>()), Times.Once);
    }

    [Fact]
    public async Task UpdateClient_ReturnsNotFoundResponse_WhenClientNotExists()
    {
        var clientUpdate = new ClientUpdateDto { Id = 999 };
        var expectedResponse = new Response<ClientResponseDto>(ResponseCode.NotFound, "Client not found.");

        _mockClientService.Setup(x => x.UpdateClientForDietitianAsync(It.IsAny<ClientUpdateDto>())).ReturnsAsync(expectedResponse);

        var result = await _controller.UpdateClient(clientUpdate);

        Assert.NotNull(result);
        Assert.Equal(ResponseCode.NotFound, result.ResponseCode);
        Assert.Null(result.Data);
        _mockClientService.Verify(x => x.UpdateClientForDietitianAsync(It.IsAny<ClientUpdateDto>()), Times.Once);
    }

    [Fact]
    public async Task GetDietPlans_ReturnsSuccessResponse_WithDietPlansList()
    {
        var dietPlansList = new List<DietPlanResponseDto>
        {
            new DietPlanResponseDto
            {
                Id = 1,
                Title = "Weight Loss Plan",
                Description = "Basic weight loss diet plan",
                ClientName = "John Doe"
            },
            new DietPlanResponseDto
            {
                Id = 2,
                Title = "Muscle Gain Plan",
                Description = "Protein rich diet plan",
                ClientName = "Jane Smith"
            }
        };

        var expectedResponse = new Response<IList<DietPlanResponseDto>>(ResponseCode.Success, dietPlansList, "Diet plans retrieved successfully.");

        _mockDietPlanService.Setup(x => x.GetByDietitianUserIdAsync()).ReturnsAsync(expectedResponse);

        var result = await _controller.GetDietPlans();

        Assert.NotNull(result);
        Assert.Equal(ResponseCode.Success, result.ResponseCode);
        Assert.Equal(2, result.Data.Count);
        Assert.Equal("Weight Loss Plan", result.Data.First().Title);
        _mockDietPlanService.Verify(x => x.GetByDietitianUserIdAsync(), Times.Once);
    }

    [Fact]
    public async Task GetDietPlanById_ReturnsSuccessResponse_WhenDietPlanExists()
    {
        var dietPlanId = 1;
        var expectedResponse = new Response<DietPlanResponseDto>(ResponseCode.Success,
            new DietPlanResponseDto
            {
                Id = dietPlanId,
                Title = "Weight Loss Plan",
                Description = "Basic weight loss diet plan"
            }, "Diet plan retrieved successfully."
        );

        _mockDietPlanService.Setup(x => x.GetByIdForDietitianAsync(dietPlanId)).ReturnsAsync(expectedResponse);

        var result = await _controller.GetDietPlanById(dietPlanId);

        Assert.NotNull(result);
        Assert.Equal(ResponseCode.Success, result.ResponseCode);
        Assert.Equal(dietPlanId, result.Data.Id);
        Assert.Equal("Weight Loss Plan", result.Data.Title);
        _mockDietPlanService.Verify(x => x.GetByIdForDietitianAsync(dietPlanId), Times.Once);
    }

    [Fact]
    public async Task GetDietPlanById_ReturnsNotFoundResponse_WhenDietPlanNotExists()
    {
        var dietPlanId = 999;
        var expectedResponse = new Response<DietPlanResponseDto>(ResponseCode.NotFound, "Diet plan not found.");

        _mockDietPlanService.Setup(x => x.GetByIdForDietitianAsync(dietPlanId)).ReturnsAsync(expectedResponse);

        var result = await _controller.GetDietPlanById(dietPlanId);

        Assert.NotNull(result);
        Assert.Equal(ResponseCode.NotFound, result.ResponseCode);
        Assert.Null(result.Data);
        _mockDietPlanService.Verify(x => x.GetByIdForDietitianAsync(dietPlanId), Times.Once);
    }

    [Fact]
    public async Task GetClientDietPlans_ReturnsSuccessResponse_WhenClientHasDietPlans()
    {
        var clientId = 1;
        var dietPlansList = new List<DietPlanResponseDto>
        {
            new DietPlanResponseDto
            {
                Id = 1,
                Title = "Weight Loss Plan",
                ClientId = clientId
            }
        };

        var expectedResponse = new Response<IList<DietPlanResponseDto>>(ResponseCode.Success, dietPlansList, "Client diet plans retrieved successfully.");

        _mockDietPlanService.Setup(x => x.GetByClientIdForDietitianAsync(clientId)).ReturnsAsync(expectedResponse);

        var result = await _controller.GetClientDietPlans(clientId);

        Assert.NotNull(result);
        Assert.Equal(ResponseCode.Success, result.ResponseCode);
        Assert.Single(result.Data);
        Assert.Equal(clientId, result.Data.First().ClientId);
        _mockDietPlanService.Verify(x => x.GetByClientIdForDietitianAsync(clientId), Times.Once);
    }

    [Fact]
    public async Task CreateDietPlan_ReturnsSuccessResponse_WhenValidDietPlanData()
    {
        var dietPlanRequest = new DietPlanRequestDto
        {
            Title = "New Diet Plan",
            Description = "New diet plan description",
            StartDate = DateTime.Today,
            EndDate = DateTime.Today.AddDays(30),
            InitialWeight = 80.0m,
            TargetWeight = 75.0m,
            ClientId = 1
        };

        var expectedResponse = new Response<DietPlanResponseDto>(ResponseCode.Success,
            new DietPlanResponseDto
            {
                Id = 1,
                Title = dietPlanRequest.Title,
                Description = dietPlanRequest.Description,
                ClientId = dietPlanRequest.ClientId
            }, "Diet plan created successfully.");

        _mockDietPlanService.Setup(x => x.CreateByDietitianAsync(It.IsAny<DietPlanRequestDto>())).ReturnsAsync(expectedResponse);

        var result = await _controller.CreateDietPlan(dietPlanRequest);

        Assert.NotNull(result);
        Assert.Equal(ResponseCode.Success, result.ResponseCode);
        Assert.Equal(dietPlanRequest.Title, result.Data.Title);
        _mockDietPlanService.Verify(x => x.CreateByDietitianAsync(It.IsAny<DietPlanRequestDto>()), Times.Once);
    }

    [Fact]
    public async Task UpdateDietPlan_ReturnsSuccessResponse_WhenValidUpdateData()
    {
        var dietPlanUpdate = new DietPlanUpdateDto
        {
            Id = 1,
            Title = "Updated Diet Plan",
            Description = "Updated description",
            StartDate = DateTime.Today,
            EndDate = DateTime.Today.AddDays(30),
            TargetWeight = 70.0m
        };

        var expectedResponse = new Response<DietPlanResponseDto>(ResponseCode.Success,
            new DietPlanResponseDto
            {
                Id = dietPlanUpdate.Id,
                Title = dietPlanUpdate.Title,
                Description = dietPlanUpdate.Description
            }, "Diet plan updated successfully.");

        _mockDietPlanService.Setup(x => x.UpdateByDietitianAsync(It.IsAny<DietPlanUpdateDto>())).ReturnsAsync(expectedResponse);

        var result = await _controller.UpdateDietPlan(dietPlanUpdate);

        Assert.NotNull(result);
        Assert.Equal(ResponseCode.Success, result.ResponseCode);
        Assert.Equal(dietPlanUpdate.Title, result.Data.Title);
        _mockDietPlanService.Verify(x => x.UpdateByDietitianAsync(It.IsAny<DietPlanUpdateDto>()), Times.Once);
    }

    [Fact]
    public async Task DeleteDietPlan_ReturnsSuccessResponse_WhenDietPlanExists()
    {
        var dietPlanId = 1;
        var expectedResponse = new Response<bool>(ResponseCode.Success, true, "Diet plan deleted successfully.");

        _mockDietPlanService.Setup(x => x.DeleteByDietitianAsync(dietPlanId)).ReturnsAsync(expectedResponse);

        var result = await _controller.DeleteDietPlan(dietPlanId);

        Assert.NotNull(result);
        Assert.Equal(ResponseCode.Success, result.ResponseCode);
        Assert.True(result.Data);
        _mockDietPlanService.Verify(x => x.DeleteByDietitianAsync(dietPlanId), Times.Once);
    }

    [Fact]
    public async Task DeleteDietPlan_ReturnsNotFoundResponse_WhenDietPlanNotExists()
    {
        var dietPlanId = 999;
        var expectedResponse = new Response<bool>(ResponseCode.NotFound, "Diet plan not found.");

        _mockDietPlanService.Setup(x => x.DeleteByDietitianAsync(dietPlanId)).ReturnsAsync(expectedResponse);

        var result = await _controller.DeleteDietPlan(dietPlanId);

        Assert.NotNull(result);
        Assert.Equal(ResponseCode.NotFound, result.ResponseCode);
        Assert.False(result.Data);
        _mockDietPlanService.Verify(x => x.DeleteByDietitianAsync(dietPlanId), Times.Once);
    }

    [Fact]
    public async Task DeleteDietPlan_ReturnsForbiddenResponse_WhenUnauthorizedAccess()
    {
        var dietPlanId = 1;
        var expectedResponse = new Response<bool>(ResponseCode.Forbidden, "You don't have permission to delete this diet plan.");

        _mockDietPlanService.Setup(x => x.DeleteByDietitianAsync(dietPlanId)).ReturnsAsync(expectedResponse);

        var result = await _controller.DeleteDietPlan(dietPlanId);

        Assert.NotNull(result);
        Assert.Equal(ResponseCode.Forbidden, result.ResponseCode);
        Assert.False(result.Data);
        _mockDietPlanService.Verify(x => x.DeleteByDietitianAsync(dietPlanId), Times.Once);
    }
}