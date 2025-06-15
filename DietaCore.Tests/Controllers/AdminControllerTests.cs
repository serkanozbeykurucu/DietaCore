using DietaCore.Api.Controllers;
using DietaCore.Business.Abstract;
using DietaCore.Dto.ClientDTOs;
using DietaCore.Dto.DietitianDTOs;
using DietaCore.Shared.Common.Responses.ComplexTypes;
using DietaCore.Shared.Common.Responses.Concrete;
using FluentAssertions;
using Moq;

namespace DietaCore.Tests;

public class AdminControllerTests
{
    private readonly Mock<IDietitianService> _mockDietitianService;
    private readonly Mock<IClientService> _mockClientService;
    private readonly AdminController _controller;

    public AdminControllerTests()
    {
        _mockDietitianService = new Mock<IDietitianService>();
        _mockClientService = new Mock<IClientService>();
        _controller = new AdminController(_mockDietitianService.Object, _mockClientService.Object);
    }

    [Fact]
    public async Task GetAllDietitians_WhenCalled_ReturnsResponseWithDietitianList()
    {
        var expectedDietitians = new List<DietitianResponseDto>
        {
            new DietitianResponseDto { Id = 1, FirstName = "John", LastName = "Doe", Email = "john@test.com" },
            new DietitianResponseDto { Id = 2, FirstName = "Jane", LastName = "Smith", Email = "jane@test.com" }
        };
        var expectedResponse = new Response<IList<DietitianResponseDto>>(ResponseCode.Success, expectedDietitians);

        _mockDietitianService.Setup(x => x.GetAllAsync()).ReturnsAsync(expectedResponse);

        var result = await _controller.GetAllDietitians();

        result.Should().NotBeNull();
        result.Should().Be(expectedResponse);
        result.Data.Should().HaveCount(2);
        result.Data.First().FirstName.Should().Be("John");
        _mockDietitianService.Verify(x => x.GetAllAsync(), Times.Once);
    }

    [Fact]
    public async Task GetDietitianById_WithValidId_ReturnsResponseWithDietitian()
    {
        var dietitianId = 1;
        var expectedDietitian = new DietitianResponseDto
        {
            Id = dietitianId,
            FirstName = "John",
            LastName = "Doe",
            Email = "john@test.com"
        };
        var expectedResponse = new Response<DietitianResponseDto>(ResponseCode.Success, expectedDietitian);

        _mockDietitianService.Setup(x => x.GetByIdAsync(dietitianId)).ReturnsAsync(expectedResponse);

        var result = await _controller.GetDietitianById(dietitianId);

        result.Should().NotBeNull();
        result.Should().Be(expectedResponse);
        result.Data.Id.Should().Be(dietitianId);
        result.Data.FirstName.Should().Be("John");
        _mockDietitianService.Verify(x => x.GetByIdAsync(dietitianId), Times.Once);
    }

    [Fact]
    public async Task GetDietitianById_WithInvalidId_ReturnsNotFoundResponse()
    {
        var dietitianId = 999;
        var expectedResponse = new Response<DietitianResponseDto>(ResponseCode.NotFound, "Dietitian not found.");

        _mockDietitianService.Setup(x => x.GetByIdAsync(dietitianId)).ReturnsAsync(expectedResponse);

        var result = await _controller.GetDietitianById(dietitianId);

        result.Should().NotBeNull();
        result.ResponseCode.Should().Be(ResponseCode.NotFound);
        result.Message.Should().Be("Dietitian not found.");
        _mockDietitianService.Verify(x => x.GetByIdAsync(dietitianId), Times.Once);
    }

    [Fact]
    public async Task CreateDietitian_WithValidData_ReturnsSuccessResponse()
    {
        var dietitianRequest = new DietitianRequestDto
        {
            FirstName = "John",
            LastName = "Doe",
            Email = "john@test.com",
            Password = "Password123!",
            PhoneNumber = "1234567890",
            Specialization = "Weight Loss",
            LicenseNumber = "LIC123",
            Education = "PhD Nutrition",
            Biography = "Experienced dietitian"
        };

        var expectedDietitian = new DietitianResponseDto
        {
            Id = 1,
            FirstName = "John",
            LastName = "Doe",
            Email = "john@test.com",
            PhoneNumber = "1234567890",
            Specialization = "Weight Loss",
            LicenseNumber = "LIC123",
            Education = "PhD Nutrition",
            Biography = "Experienced dietitian"
        };

        var expectedResponse = new Response<DietitianResponseDto>(ResponseCode.Success, expectedDietitian);

        _mockDietitianService.Setup(x => x.CreateAsync(dietitianRequest)).ReturnsAsync(expectedResponse);

        var result = await _controller.CreateDietitian(dietitianRequest);

        result.Should().NotBeNull();
        result.ResponseCode.Should().Be(ResponseCode.Success);
        result.Data.FirstName.Should().Be("John");
        result.Data.Email.Should().Be("john@test.com");
        _mockDietitianService.Verify(x => x.CreateAsync(dietitianRequest), Times.Once);
    }

    [Fact]
    public async Task UpdateDietitian_WithValidData_ReturnsSuccessResponse()
    {
        var dietitianUpdate = new DietitianUpdateDto
        {
            Id = 1,
            FirstName = "John Updated",
            LastName = "Doe",
            Email = "john.updated@test.com",
            PhoneNumber = "1234567890",
            Specialization = "Weight Loss Updated",
            LicenseNumber = "LIC123",
            Education = "PhD Nutrition",
            Biography = "Updated biography"
        };

        var expectedDietitian = new DietitianResponseDto
        {
            Id = 1,
            FirstName = "John Updated",
            LastName = "Doe",
            Email = "john.updated@test.com",
            PhoneNumber = "1234567890",
            Specialization = "Weight Loss Updated",
            LicenseNumber = "LIC123",
            Education = "PhD Nutrition",
            Biography = "Updated biography"
        };

        var expectedResponse = new Response<DietitianResponseDto>(ResponseCode.Success, expectedDietitian);

        _mockDietitianService.Setup(x => x.UpdateAsync(dietitianUpdate)).ReturnsAsync(expectedResponse);

        var result = await _controller.UpdateDietitian(dietitianUpdate);

        result.Should().NotBeNull();
        result.ResponseCode.Should().Be(ResponseCode.Success);
        result.Data.FirstName.Should().Be("John Updated");
        result.Data.Specialization.Should().Be("Weight Loss Updated");
        _mockDietitianService.Verify(x => x.UpdateAsync(dietitianUpdate), Times.Once);
    }

    [Fact]
    public async Task DeleteDietitian_WithValidId_ReturnsSuccessResponse()
    {
        var dietitianId = 1;
        var expectedResponse = new Response<bool>(ResponseCode.Success, true, "Dietitian deleted successfully.");

        _mockDietitianService.Setup(x => x.DeleteAsync(dietitianId)).ReturnsAsync(expectedResponse);

        var result = await _controller.DeleteDietitian(dietitianId);

        result.Should().NotBeNull();
        result.ResponseCode.Should().Be(ResponseCode.Success);
        result.Data.Should().BeTrue();
        result.Message.Should().Be("Dietitian deleted successfully.");
        _mockDietitianService.Verify(x => x.DeleteAsync(dietitianId), Times.Once);
    }

    [Fact]
    public async Task GetAllClients_WhenCalled_ReturnsResponseWithClientList()
    {
        var expectedClients = new List<ClientResponseDto>
        {
            new ClientResponseDto { Id = 1, FirstName = "Alice", LastName = "Johnson", Email = "alice@test.com" },
            new ClientResponseDto { Id = 2, FirstName = "Bob", LastName = "Wilson", Email = "bob@test.com" }
        };
        var expectedResponse = new Response<IList<ClientResponseDto>>(ResponseCode.Success, expectedClients);

        _mockClientService.Setup(x => x.GetAllClientsForAdminAsync()).ReturnsAsync(expectedResponse);

        var result = await _controller.GetAllClients();

        result.Should().NotBeNull();
        result.Should().Be(expectedResponse);
        result.Data.Should().HaveCount(2);
        result.Data.First().FirstName.Should().Be("Alice");
        _mockClientService.Verify(x => x.GetAllClientsForAdminAsync(), Times.Once);
    }

    [Fact]
    public async Task GetClientById_WithValidId_ReturnsResponseWithClient()
    {
        var clientId = 1;
        var expectedClient = new ClientResponseDto
        {
            Id = clientId,
            FirstName = "Alice",
            LastName = "Johnson",
            Email = "alice@test.com"
        };
        var expectedResponse = new Response<ClientResponseDto>(ResponseCode.Success, expectedClient);

        _mockClientService.Setup(x => x.GetClientByIdForAdminAsync(clientId)).ReturnsAsync(expectedResponse);

        var result = await _controller.GetClientById(clientId);

        result.Should().NotBeNull();
        result.Should().Be(expectedResponse);
        result.Data.Id.Should().Be(clientId);
        result.Data.FirstName.Should().Be("Alice");
        _mockClientService.Verify(x => x.GetClientByIdForAdminAsync(clientId), Times.Once);
    }

    [Fact]
    public async Task CreateClient_WithValidData_ReturnsSuccessResponse()
    {
        var clientRequest = new ClientRequestDto
        {
            FirstName = "Alice",
            LastName = "Johnson",
            Email = "alice@test.com",
            Password = "Password123!",
            PhoneNumber = "1234567890",
            DietitianId = 1,
            DateOfBirth = new DateTime(1990, 1, 1),
            Gender = "Female",
            Height = 165.5m,
            InitialWeight = 70.0m,
            MedicalConditions = "None",
            Allergies = "None"
        };

        var expectedClient = new ClientResponseDto
        {
            Id = 1,
            FirstName = "Alice",
            LastName = "Johnson",
            Email = "alice@test.com",
            PhoneNumber = "1234567890",
            DietitianId = 1,
            DateOfBirth = new DateTime(1990, 1, 1),
            Gender = "Female",
            Height = 165.5m,
            InitialWeight = 70.0m,
            CurrentWeight = 70.0m,
            MedicalConditions = "None",
            Allergies = "None"
        };

        var expectedResponse = new Response<ClientResponseDto>(ResponseCode.Success, expectedClient);

        _mockClientService.Setup(x => x.CreateClientForAdminAsync(clientRequest)).ReturnsAsync(expectedResponse);

        var result = await _controller.CreateClient(clientRequest);

        result.Should().NotBeNull();
        result.ResponseCode.Should().Be(ResponseCode.Success);
        result.Data.FirstName.Should().Be("Alice");
        result.Data.Email.Should().Be("alice@test.com");
        _mockClientService.Verify(x => x.CreateClientForAdminAsync(clientRequest), Times.Once);
    }

    [Fact]
    public async Task UpdateClient_WithValidData_ReturnsSuccessResponse()
    {
        var clientUpdate = new ClientUpdateDto
        {
            Id = 1,
            FirstName = "Alice Updated",
            LastName = "Johnson",
            Email = "alice.updated@test.com",
            PhoneNumber = "1234567890",
            DietitianId = 1,
            DateOfBirth = new DateTime(1990, 1, 1),
            Gender = "Female",
            Height = 165.5m,
            CurrentWeight = 68.0m,
            MedicalConditions = "Updated conditions",
            Allergies = "Updated allergies"
        };

        var expectedClient = new ClientResponseDto
        {
            Id = 1,
            FirstName = "Alice Updated",
            LastName = "Johnson",
            Email = "alice.updated@test.com",
            PhoneNumber = "1234567890",
            DietitianId = 1,
            DateOfBirth = new DateTime(1990, 1, 1),
            Gender = "Female",
            Height = 165.5m,
            CurrentWeight = 68.0m,
            MedicalConditions = "Updated conditions",
            Allergies = "Updated allergies"
        };

        var expectedResponse = new Response<ClientResponseDto>(ResponseCode.Success, expectedClient);

        _mockClientService.Setup(x => x.UpdateClientForAdminAsync(clientUpdate)).ReturnsAsync(expectedResponse);

        var result = await _controller.UpdateClient(clientUpdate);

        result.Should().NotBeNull();
        result.ResponseCode.Should().Be(ResponseCode.Success);
        result.Data.FirstName.Should().Be("Alice Updated");
        result.Data.CurrentWeight.Should().Be(68.0m);
        _mockClientService.Verify(x => x.UpdateClientForAdminAsync(clientUpdate), Times.Once);
    }

    [Fact]
    public async Task DeleteClient_WithValidId_ReturnsSuccessResponse()
    {
        var clientId = 1;
        var expectedResponse = new Response<bool>(ResponseCode.Success, true, "Client deleted successfully.");

        _mockClientService.Setup(x => x.DeleteClientAsync(clientId)).ReturnsAsync(expectedResponse);

        var result = await _controller.DeleteClient(clientId);

        result.Should().NotBeNull();
        result.ResponseCode.Should().Be(ResponseCode.Success);
        result.Data.Should().BeTrue();
        result.Message.Should().Be("Client deleted successfully.");
        _mockClientService.Verify(x => x.DeleteClientAsync(clientId), Times.Once);
    }

    [Fact]
    public async Task AssignClientToDietitian_WithValidIds_ReturnsSuccessResponse()
    {
        var clientId = 1;
        var dietitianId = 2;
        var expectedResponse = new Response<bool>(ResponseCode.Success, true, "Client assigned to dietitian successfully.");

        _mockClientService.Setup(x => x.AssignClientToDietitianAsync(clientId, dietitianId)).ReturnsAsync(expectedResponse);

        var result = await _controller.AssignClientToDietitian(clientId, dietitianId);

        result.Should().NotBeNull();
        result.ResponseCode.Should().Be(ResponseCode.Success);
        result.Data.Should().BeTrue();
        result.Message.Should().Be("Client assigned to dietitian successfully.");
        _mockClientService.Verify(x => x.AssignClientToDietitianAsync(clientId, dietitianId), Times.Once);
    }

    [Fact]
    public async Task AssignClientToDietitian_WithInvalidClientId_ReturnsNotFoundResponse()
    {
        var clientId = 999;
        var dietitianId = 1;
        var expectedResponse = new Response<bool>(ResponseCode.NotFound, "Client not found.");

        _mockClientService.Setup(x => x.AssignClientToDietitianAsync(clientId, dietitianId)).ReturnsAsync(expectedResponse);

        var result = await _controller.AssignClientToDietitian(clientId, dietitianId);

        result.Should().NotBeNull();
        result.ResponseCode.Should().Be(ResponseCode.NotFound);
        result.Message.Should().Be("Client not found.");
        _mockClientService.Verify(x => x.AssignClientToDietitianAsync(clientId, dietitianId), Times.Once);
    }

    [Fact]
    public async Task AssignClientToDietitian_WithInvalidDietitianId_ReturnsNotFoundResponse()
    {
        var clientId = 1;
        var dietitianId = 999;
        var expectedResponse = new Response<bool>(ResponseCode.NotFound, "Dietitian not found.");

        _mockClientService.Setup(x => x.AssignClientToDietitianAsync(clientId, dietitianId)).ReturnsAsync(expectedResponse);

        var result = await _controller.AssignClientToDietitian(clientId, dietitianId);

        result.Should().NotBeNull();
        result.ResponseCode.Should().Be(ResponseCode.NotFound);
        result.Message.Should().Be("Dietitian not found.");
        _mockClientService.Verify(x => x.AssignClientToDietitianAsync(clientId, dietitianId), Times.Once);
    }

    [Fact]
    public async Task RemoveClientFromDietitian_WithValidId_ReturnsSuccessResponse()
    {
        var clientId = 1;
        var expectedResponse = new Response<bool>(ResponseCode.Success, true, "Client removed from dietitian successfully.");

        _mockClientService.Setup(x => x.RemoveClientFromDietitianAsync(clientId)).ReturnsAsync(expectedResponse);

        var result = await _controller.RemoveClientFromDietitian(clientId);

        result.Should().NotBeNull();
        result.ResponseCode.Should().Be(ResponseCode.Success);
        result.Data.Should().BeTrue();
        result.Message.Should().Be("Client removed from dietitian successfully.");
        _mockClientService.Verify(x => x.RemoveClientFromDietitianAsync(clientId), Times.Once);
    }

    [Fact]
    public async Task RemoveClientFromDietitian_WithInvalidClientId_ReturnsNotFoundResponse()
    {
        var clientId = 999;
        var expectedResponse = new Response<bool>(ResponseCode.NotFound, "Client not found.");

        _mockClientService.Setup(x => x.RemoveClientFromDietitianAsync(clientId)).ReturnsAsync(expectedResponse);

        var result = await _controller.RemoveClientFromDietitian(clientId);

        result.Should().NotBeNull();
        result.ResponseCode.Should().Be(ResponseCode.NotFound);
        result.Message.Should().Be("Client not found.");
        _mockClientService.Verify(x => x.RemoveClientFromDietitianAsync(clientId), Times.Once);
    }
}