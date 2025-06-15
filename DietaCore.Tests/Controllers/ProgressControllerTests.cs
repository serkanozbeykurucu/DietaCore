using DietaCore.Api.Controllers;
using DietaCore.Business.Abstract;
using DietaCore.Dto.ClientProgressDTOs;
using DietaCore.Shared.Common.Responses.ComplexTypes;
using DietaCore.Shared.Common.Responses.Concrete;
using Moq;

namespace DietaCore.Tests;

public class ProgressControllerTests
{
    private readonly Mock<IClientProgressService> _mockService;
    private readonly ProgressController _controller;

    public ProgressControllerTests()
    {
        _mockService = new Mock<IClientProgressService>();
        _controller = new ProgressController(_mockService.Object);
    }

    [Fact]
    public async Task GetProgress_ValidClientId_ReturnsSuccessResponse()
    {
        int clientId = 1;
        var expectedResponse = new Response<List<ProgressDto>>(ResponseCode.Success, new List<ProgressDto>
        {
            new ProgressDto { Id = 1, ClientId = clientId, Weight = 75.5m, Date = DateTime.Now }
        });

        _mockService.Setup(s => s.GetClientProgressByDietitianAsync(clientId)).ReturnsAsync(expectedResponse);

        var result = await _controller.GetProgress(clientId);

        Assert.NotNull(result);
        Assert.Equal(ResponseCode.Success, result.ResponseCode);
        Assert.Single(result.Data);
        Assert.Equal(clientId, result.Data.First().ClientId);
        _mockService.Verify(s => s.GetClientProgressByDietitianAsync(clientId), Times.Once);
    }

    [Fact]
    public async Task GetProgress_InvalidClientId_ReturnsBadRequestResponse()
    {
        int clientId = -1;
        var expectedResponse = new Response<List<ProgressDto>>(ResponseCode.BadRequest, "ClientId must be greater than zero.");

        _mockService.Setup(s => s.GetClientProgressByDietitianAsync(clientId)).ReturnsAsync(expectedResponse);

        var result = await _controller.GetProgress(clientId);

        Assert.NotNull(result);
        Assert.Equal(ResponseCode.BadRequest, result.ResponseCode);
        Assert.Equal("ClientId must be greater than zero.", result.Message);
        _mockService.Verify(s => s.GetClientProgressByDietitianAsync(clientId), Times.Once);
    }

    [Fact]
    public async Task GetProgress_ServiceThrowsException_ThrowsException()
    {
        int clientId = 1;
        _mockService.Setup(s => s.GetClientProgressByDietitianAsync(clientId)).ThrowsAsync(new Exception("Service error"));

        await Assert.ThrowsAsync<Exception>(() => _controller.GetProgress(clientId));
        _mockService.Verify(s => s.GetClientProgressByDietitianAsync(clientId), Times.Once);
    }

    [Fact]
    public async Task GetSummary_ValidClientId_ReturnsSuccessResponse()
    {
        int clientId = 1;
        var expectedResponse = new Response<ProgressSummary>(ResponseCode.Success, new ProgressSummary
        {
            ClientId = clientId,
            ClientName = "Test Client",
            StartWeight = 80m,
            CurrentWeight = 75m,
            TargetWeight = 70m,
            WeightLoss = 5m,
            Recent = new List<ProgressDto>()
        });

        _mockService.Setup(s => s.GetClientProgressSummaryByDietitianAsync(clientId)).ReturnsAsync(expectedResponse);

        var result = await _controller.GetSummary(clientId);

        Assert.NotNull(result);
        Assert.Equal(ResponseCode.Success, result.ResponseCode);
        Assert.Equal(clientId, result.Data.ClientId);
        Assert.Equal("Test Client", result.Data.ClientName);
        _mockService.Verify(s => s.GetClientProgressSummaryByDietitianAsync(clientId), Times.Once);
    }

    [Fact]
    public async Task GetSummary_InvalidClientId_ReturnsBadRequestResponse()
    {
        int clientId = 0;
        var expectedResponse = new Response<ProgressSummary>(ResponseCode.BadRequest, "ClientId must be greater than zero.");

        _mockService.Setup(s => s.GetClientProgressSummaryByDietitianAsync(clientId)).ReturnsAsync(expectedResponse);

        var result = await _controller.GetSummary(clientId);

        Assert.NotNull(result);
        Assert.Equal(ResponseCode.BadRequest, result.ResponseCode);
        Assert.Equal("ClientId must be greater than zero.", result.Message);
        _mockService.Verify(s => s.GetClientProgressSummaryByDietitianAsync(clientId), Times.Once);
    }

    [Fact]
    public async Task CreateProgress_ValidDto_ReturnsSuccessResponse()
    {
        var progressDto = new ProgressDto
        {
            ClientId = 1,
            Weight = 75.5m,
            BodyFat = 20.5m,
            Muscle = 35.2m,
            Waist = 85.0m,
            Notes = "Test progress",
            Date = DateTime.Now
        };

        var expectedResponse = new Response<ProgressDto>(ResponseCode.Success, progressDto);
        expectedResponse.Data.Id = 1;

        _mockService.Setup(s => s.CreateProgressByDietitianAsync(progressDto)).ReturnsAsync(expectedResponse);

        var result = await _controller.CreateProgress(progressDto);

        Assert.NotNull(result);
        Assert.Equal(ResponseCode.Success, result.ResponseCode);
        Assert.Equal(1, result.Data.Id);
        Assert.Equal(progressDto.ClientId, result.Data.ClientId);
        Assert.Equal(progressDto.Weight, result.Data.Weight);
        _mockService.Verify(s => s.CreateProgressByDietitianAsync(progressDto), Times.Once);
    }

    [Fact]
    public async Task CreateProgress_NullDto_CallsServiceWithNull()
    {
        ProgressDto nullDto = null;
        var expectedResponse = new Response<ProgressDto>(ResponseCode.BadRequest, "DTO cannot be null");

        _mockService.Setup(s => s.CreateProgressByDietitianAsync(nullDto)).ReturnsAsync(expectedResponse);

        var result = await _controller.CreateProgress(nullDto);

        Assert.NotNull(result);
        _mockService.Verify(s => s.CreateProgressByDietitianAsync(nullDto), Times.Once);
    }

    [Fact]
    public async Task CreateProgress_ServiceReturnsError_ReturnsErrorResponse()
    {
        var progressDto = new ProgressDto { ClientId = 999 };
        var expectedResponse = new Response<ProgressDto>(ResponseCode.NotFound, "Diyetisyen bulunamadý");

        _mockService.Setup(s => s.CreateProgressByDietitianAsync(progressDto)).ReturnsAsync(expectedResponse);

        var result = await _controller.CreateProgress(progressDto);

        Assert.NotNull(result);
        Assert.Equal(ResponseCode.NotFound, result.ResponseCode);
        Assert.Equal("Diyetisyen bulunamadý", result.Message);
        _mockService.Verify(s => s.CreateProgressByDietitianAsync(progressDto), Times.Once);
    }

    [Fact]
    public async Task UpdateProgress_ValidDto_ReturnsSuccessResponse()
    {
        var progressDto = new ProgressDto
        {
            Id = 1,
            ClientId = 1,
            Weight = 74.5m,
            BodyFat = 19.5m,
            Muscle = 36.0m,
            Waist = 84.0m,
            Notes = "Updated progress",
            Date = DateTime.Now
        };

        var expectedResponse = new Response<ProgressDto>(ResponseCode.Success, progressDto);

        _mockService.Setup(s => s.UpdateProgressByDietitianAsync(progressDto)).ReturnsAsync(expectedResponse);

        var result = await _controller.UpdateProgress(progressDto);

        Assert.NotNull(result);
        Assert.Equal(ResponseCode.Success, result.ResponseCode);
        Assert.Equal(progressDto.Id, result.Data.Id);
        Assert.Equal(progressDto.Weight, result.Data.Weight);
        _mockService.Verify(s => s.UpdateProgressByDietitianAsync(progressDto), Times.Once);
    }

    [Fact]
    public async Task UpdateProgress_NonExistentId_ReturnsNotFoundResponse()
    {
        var progressDto = new ProgressDto { Id = 999 };
        var expectedResponse = new Response<ProgressDto>(ResponseCode.NotFound, "Kayýt bulunamadý");

        _mockService.Setup(s => s.UpdateProgressByDietitianAsync(progressDto)).ReturnsAsync(expectedResponse);

        var result = await _controller.UpdateProgress(progressDto);

        Assert.NotNull(result);
        Assert.Equal(ResponseCode.NotFound, result.ResponseCode);
        Assert.Equal("Kayýt bulunamadý", result.Message);
        _mockService.Verify(s => s.UpdateProgressByDietitianAsync(progressDto), Times.Once);
    }

    [Fact]
    public async Task UpdateProgress_NullDto_CallsServiceWithNull()
    {
        ProgressDto nullDto = null;
        var expectedResponse = new Response<ProgressDto>(ResponseCode.BadRequest, "DTO cannot be null");

        _mockService.Setup(s => s.UpdateProgressByDietitianAsync(nullDto)).ReturnsAsync(expectedResponse);

        var result = await _controller.UpdateProgress(nullDto);

        Assert.NotNull(result);
        _mockService.Verify(s => s.UpdateProgressByDietitianAsync(nullDto), Times.Once);
    }

    [Fact]
    public async Task DeleteProgress_ValidId_ReturnsSuccessResponse()
    {
        int progressId = 1;
        var expectedResponse = new Response<bool>(ResponseCode.Success, true);

        _mockService.Setup(s => s.DeleteProgressByDietitianAsync(progressId)).ReturnsAsync(expectedResponse);

        var result = await _controller.DeleteProgress(progressId);

        Assert.NotNull(result);
        Assert.Equal(ResponseCode.Success, result.ResponseCode);
        Assert.True(result.Data);
        _mockService.Verify(s => s.DeleteProgressByDietitianAsync(progressId), Times.Once);
    }

    [Fact]
    public async Task DeleteProgress_InvalidId_ReturnsBadRequestResponse()
    {
        int progressId = -1;
        var expectedResponse = new Response<bool>(ResponseCode.BadRequest, "Id must be greater than zero.");

        _mockService.Setup(s => s.DeleteProgressByDietitianAsync(progressId)).ReturnsAsync(expectedResponse);

        var result = await _controller.DeleteProgress(progressId);

        Assert.NotNull(result);
        Assert.Equal(ResponseCode.BadRequest, result.ResponseCode);
        Assert.Equal("Id must be greater than zero.", result.Message);
        _mockService.Verify(s => s.DeleteProgressByDietitianAsync(progressId), Times.Once);
    }

    [Fact]
    public async Task DeleteProgress_NonExistentId_ReturnsNotFoundResponse()
    {
        int progressId = 999;
        var expectedResponse = new Response<bool>(ResponseCode.NotFound, "Kayýt bulunamadý");

        _mockService.Setup(s => s.DeleteProgressByDietitianAsync(progressId)).ReturnsAsync(expectedResponse);

        var result = await _controller.DeleteProgress(progressId);

        Assert.NotNull(result);
        Assert.Equal(ResponseCode.NotFound, result.ResponseCode);
        Assert.Equal("Kayýt bulunamadý", result.Message);
        _mockService.Verify(s => s.DeleteProgressByDietitianAsync(progressId), Times.Once);
    }

    [Theory]
    [InlineData(1)]
    [InlineData(5)]
    [InlineData(100)]
    public async Task GetProgress_MultipleValidClientIds_ReturnsSuccessResponse(int clientId)
    {
        var expectedResponse = new Response<List<ProgressDto>>(ResponseCode.Success, new List<ProgressDto>());

        _mockService.Setup(s => s.GetClientProgressByDietitianAsync(clientId)).ReturnsAsync(expectedResponse);

        var result = await _controller.GetProgress(clientId);

        Assert.NotNull(result);
        Assert.Equal(ResponseCode.Success, result.ResponseCode);
        _mockService.Verify(s => s.GetClientProgressByDietitianAsync(clientId), Times.Once);
    }

    [Theory]
    [InlineData(-1)]
    [InlineData(0)]
    [InlineData(-100)]
    public async Task GetProgress_MultipleInvalidClientIds_ReturnsBadRequestResponse(int clientId)
    {
        var expectedResponse = new Response<List<ProgressDto>>(ResponseCode.BadRequest, "ClientId must be greater than zero.");

        _mockService.Setup(s => s.GetClientProgressByDietitianAsync(clientId)).ReturnsAsync(expectedResponse);

        var result = await _controller.GetProgress(clientId);

        Assert.NotNull(result);
        Assert.Equal(ResponseCode.BadRequest, result.ResponseCode);
        _mockService.Verify(s => s.GetClientProgressByDietitianAsync(clientId), Times.Once);
    }
}