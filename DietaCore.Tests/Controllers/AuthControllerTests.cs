using DietaCore.Api.Controllers;
using DietaCore.Business.Abstract;
using DietaCore.Dto.AuthDTOs;
using DietaCore.Shared.Common.Responses.ComplexTypes;
using DietaCore.Shared.Common.Responses.Concrete;
using Moq;

namespace DietaCore.Tests;

public class AuthControllerTests
{
    private readonly Mock<IAuthService> _mockAuthService;
    private readonly AuthController _controller;

    public AuthControllerTests()
    {
        _mockAuthService = new Mock<IAuthService>();
        _controller = new AuthController(_mockAuthService.Object);
    }

    [Fact]
    public async Task Register_ValidRequest_ReturnsSuccessResponse()
    {
        var registerRequest = new RegisterRequestDto
        {
            FirstName = "John",
            LastName = "Doe",
            Email = "john.doe@example.com",
            Password = "Password123!",
            ConfirmPassword = "Password123!",
            PhoneNumber = "1234567890"
        };

        var expectedResponse = new Response<AuthResponseDto>(ResponseCode.Success,
            new AuthResponseDto
            {
                Id = 1,
                FirstName = "John",
                LastName = "Doe",
                Email = "john.doe@example.com",
                Role = "Client",
                Token = "test-token",
                ExpiresAt = DateTime.UtcNow.AddHours(1)
            }, "User registered successfully."
        );

        _mockAuthService.Setup(x => x.RegisterAsync(It.IsAny<RegisterRequestDto>())).ReturnsAsync(expectedResponse);

        var result = await _controller.Register(registerRequest);

        Assert.NotNull(result);
        Assert.Equal(ResponseCode.Success, result.ResponseCode);
        Assert.Equal("User registered successfully.", result.Message);
        Assert.NotNull(result.Data);
        Assert.Equal("john.doe@example.com", result.Data.Email);
        _mockAuthService.Verify(x => x.RegisterAsync(registerRequest), Times.Once);
    }

    [Fact]
    public async Task Register_InvalidRequest_ReturnsBadRequestResponse()
    {
        var registerRequest = new RegisterRequestDto
        {
            FirstName = "John",
            LastName = "Doe",
            Email = "invalid-email",
            Password = "123",
            ConfirmPassword = "456",
            PhoneNumber = "1234567890"
        };

        var expectedResponse = new Response<AuthResponseDto>(ResponseCode.BadRequest, "Passwords do not match");

        _mockAuthService.Setup(x => x.RegisterAsync(It.IsAny<RegisterRequestDto>())).ReturnsAsync(expectedResponse);

        var result = await _controller.Register(registerRequest);

        Assert.NotNull(result);
        Assert.Equal(ResponseCode.BadRequest, result.ResponseCode);
        Assert.Equal("Passwords do not match", result.Message);
        Assert.Null(result.Data);
        _mockAuthService.Verify(x => x.RegisterAsync(registerRequest), Times.Once);
    }

    [Fact]
    public async Task Login_ValidCredentials_ReturnsSuccessResponse()
    {
        var loginRequest = new LoginRequestDto
        {
            Email = "john.doe@example.com",
            Password = "Password123!"
        };

        var expectedResponse = new Response<AuthResponseDto>(ResponseCode.Success,
            new AuthResponseDto
            {
                Id = 1,
                FirstName = "John",
                LastName = "Doe",
                Email = "john.doe@example.com",
                Role = "Client",
                Token = "test-token",
                ExpiresAt = DateTime.UtcNow.AddHours(1)
            }, "Login successful."
        );

        _mockAuthService.Setup(x => x.LoginAsync(It.IsAny<LoginRequestDto>())).ReturnsAsync(expectedResponse);

        var result = await _controller.Login(loginRequest);

        Assert.NotNull(result);
        Assert.Equal(ResponseCode.Success, result.ResponseCode);
        Assert.Equal("Login successful.", result.Message);
        Assert.NotNull(result.Data);
        Assert.Equal("test-token", result.Data.Token);
        _mockAuthService.Verify(x => x.LoginAsync(loginRequest), Times.Once);
    }

    [Fact]
    public async Task Login_InvalidCredentials_ReturnsNotFoundResponse()
    {
        var loginRequest = new LoginRequestDto
        {
            Email = "nonexistent@example.com",
            Password = "wrongpassword"
        };

        var expectedResponse = new Response<AuthResponseDto>(ResponseCode.NotFound, "Invalid email or password.");

        _mockAuthService.Setup(x => x.LoginAsync(It.IsAny<LoginRequestDto>())).ReturnsAsync(expectedResponse);

        var result = await _controller.Login(loginRequest);

        Assert.NotNull(result);
        Assert.Equal(ResponseCode.NotFound, result.ResponseCode);
        Assert.Equal("Invalid email or password.", result.Message);
        Assert.Null(result.Data);
        _mockAuthService.Verify(x => x.LoginAsync(loginRequest), Times.Once);
    }

    [Fact]
    public async Task Login_UnconfirmedEmail_ReturnsBadRequestResponse()
    {
        var loginRequest = new LoginRequestDto
        {
            Email = "unconfirmed@example.com",
            Password = "Password123!"
        };

        var expectedResponse = new Response<AuthResponseDto>(ResponseCode.BadRequest, "Email not confirmed.");

        _mockAuthService.Setup(x => x.LoginAsync(It.IsAny<LoginRequestDto>())).ReturnsAsync(expectedResponse);

        var result = await _controller.Login(loginRequest);

        Assert.NotNull(result);
        Assert.Equal(ResponseCode.BadRequest, result.ResponseCode);
        Assert.Equal("Email not confirmed.", result.Message);
        Assert.Null(result.Data);
        _mockAuthService.Verify(x => x.LoginAsync(loginRequest), Times.Once);
    }

    [Fact]
    public async Task ConfirmEmail_ValidToken_ReturnsSuccessResponse()
    {
        string userId = "1";
        string token = "valid-token";

        var expectedResponse = new Response<bool>(ResponseCode.Success, true, "Email confirmed successfully.");

        _mockAuthService.Setup(x => x.ConfirmEmailAsync(It.IsAny<string>(), It.IsAny<string>())).ReturnsAsync(expectedResponse);

        var result = await _controller.ConfirmEmail(userId, token);

        Assert.NotNull(result);
        Assert.Equal(ResponseCode.Success, result.ResponseCode);
        Assert.Equal("Email confirmed successfully.", result.Message);
        Assert.True(result.Data);
        _mockAuthService.Verify(x => x.ConfirmEmailAsync(userId, token), Times.Once);
    }

    [Fact]
    public async Task ConfirmEmail_InvalidToken_ReturnsBadRequestResponse()
    {
        string userId = "1";
        string token = "invalid-token";

        var expectedResponse = new Response<bool>(ResponseCode.BadRequest, "Invalid token");

        _mockAuthService.Setup(x => x.ConfirmEmailAsync(It.IsAny<string>(), It.IsAny<string>())).ReturnsAsync(expectedResponse);

        var result = await _controller.ConfirmEmail(userId, token);

        Assert.NotNull(result);
        Assert.Equal(ResponseCode.BadRequest, result.ResponseCode);
        Assert.Equal("Invalid token", result.Message);
        Assert.False(result.Data);
        _mockAuthService.Verify(x => x.ConfirmEmailAsync(userId, token), Times.Once);
    }

    [Fact]
    public async Task ConfirmEmail_UserNotFound_ReturnsNotFoundResponse()
    {
        string userId = "999";
        string token = "valid-token";

        var expectedResponse = new Response<bool>(ResponseCode.NotFound, "User not found.");

        _mockAuthService.Setup(x => x.ConfirmEmailAsync(It.IsAny<string>(), It.IsAny<string>())).ReturnsAsync(expectedResponse);

        var result = await _controller.ConfirmEmail(userId, token);

        Assert.NotNull(result);
        Assert.Equal(ResponseCode.NotFound, result.ResponseCode);
        Assert.Equal("User not found.", result.Message);
        Assert.False(result.Data);
        _mockAuthService.Verify(x => x.ConfirmEmailAsync(userId, token), Times.Once);
    }

    [Fact]
    public async Task ForgotPassword_ValidEmail_ReturnsSuccessResponse()
    {
        string email = "john.doe@example.com";

        var expectedResponse = new Response<string>(ResponseCode.Success, "reset-token", "Password reset token generated successfully.");

        _mockAuthService.Setup(x => x.GeneratePasswordResetTokenAsync(It.IsAny<string>())).ReturnsAsync(expectedResponse);

        var result = await _controller.ForgotPassword(email);

        Assert.NotNull(result);
        Assert.Equal(ResponseCode.Success, result.ResponseCode);
        Assert.Equal("Password reset token generated successfully.", result.Message);
        Assert.Equal("reset-token", result.Data);
        _mockAuthService.Verify(x => x.GeneratePasswordResetTokenAsync(email), Times.Once);
    }

    [Fact]
    public async Task ForgotPassword_UserNotFound_ReturnsNotFoundResponse()
    {
        string email = "nonexistent@example.com";

        var expectedResponse = new Response<string>(ResponseCode.NotFound, "User not found.");

        _mockAuthService.Setup(x => x.GeneratePasswordResetTokenAsync(It.IsAny<string>())).ReturnsAsync(expectedResponse);

        var result = await _controller.ForgotPassword(email);

        Assert.NotNull(result);
        Assert.Equal(ResponseCode.NotFound, result.ResponseCode);
        Assert.Equal("User not found.", result.Message);
        Assert.Null(result.Data);
        _mockAuthService.Verify(x => x.GeneratePasswordResetTokenAsync(email), Times.Once);
    }

    [Fact]
    public async Task ResetPassword_ValidRequest_ReturnsSuccessResponse()
    {
        string email = "john.doe@example.com";
        string token = "valid-reset-token";
        string newPassword = "NewPassword123!";

        var expectedResponse = new Response<bool>(ResponseCode.Success, true, "Password reset successfully.");

        _mockAuthService.Setup(x => x.ResetPasswordAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>())).ReturnsAsync(expectedResponse);

        var result = await _controller.ResetPassword(email, token, newPassword);

        Assert.NotNull(result);
        Assert.Equal(ResponseCode.Success, result.ResponseCode);
        Assert.Equal("Password reset successfully.", result.Message);
        Assert.True(result.Data);
        _mockAuthService.Verify(x => x.ResetPasswordAsync(email, token, newPassword), Times.Once);
    }

    [Fact]
    public async Task ResetPassword_InvalidToken_ReturnsBadRequestResponse()
    {
        string email = "john.doe@example.com";
        string token = "invalid-token";
        string newPassword = "NewPassword123!";

        var expectedResponse = new Response<bool>(ResponseCode.BadRequest, "Invalid token");

        _mockAuthService.Setup(x => x.ResetPasswordAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>())).ReturnsAsync(expectedResponse);

        var result = await _controller.ResetPassword(email, token, newPassword);

        Assert.NotNull(result);
        Assert.Equal(ResponseCode.BadRequest, result.ResponseCode);
        Assert.Equal("Invalid token", result.Message);
        Assert.False(result.Data);
        _mockAuthService.Verify(x => x.ResetPasswordAsync(email, token, newPassword), Times.Once);
    }

    [Fact]
    public async Task ResetPassword_UserNotFound_ReturnsNotFoundResponse()
    {
        string email = "nonexistent@example.com";
        string token = "valid-token";
        string newPassword = "NewPassword123!";

        var expectedResponse = new Response<bool>(ResponseCode.NotFound, "User not found.");

        _mockAuthService.Setup(x => x.ResetPasswordAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>())).ReturnsAsync(expectedResponse);

        var result = await _controller.ResetPassword(email, token, newPassword);

        Assert.NotNull(result);
        Assert.Equal(ResponseCode.NotFound, result.ResponseCode);
        Assert.Equal("User not found.", result.Message);
        Assert.False(result.Data);
        _mockAuthService.Verify(x => x.ResetPasswordAsync(email, token, newPassword), Times.Once);
    }

    [Fact]
    public async Task Constructor_WithNullAuthService_ThrowsNullReferenceExceptionOnMethodCall()
    {
        var controller = new AuthController(null);
        var registerRequest = new RegisterRequestDto
        {
            FirstName = "John",
            LastName = "Doe",
            Email = "john.doe@example.com",
            Password = "Password123!",
            ConfirmPassword = "Password123!",
            PhoneNumber = "1234567890"
        };

        await Assert.ThrowsAsync<NullReferenceException>(() => controller.Register(registerRequest));
    }

    [Theory]
    [InlineData("")]
    [InlineData(null)]
    public async Task ConfirmEmail_EmptyOrNullUserId_CallsServiceWithProvidedValue(string userId)
    {
        string token = "valid-token";

        var expectedResponse = new Response<bool>(ResponseCode.BadRequest, "Invalid user ID");

        _mockAuthService.Setup(x => x.ConfirmEmailAsync(It.IsAny<string>(), It.IsAny<string>())).ReturnsAsync(expectedResponse);

        var result = await _controller.ConfirmEmail(userId, token);

        Assert.NotNull(result);
        _mockAuthService.Verify(x => x.ConfirmEmailAsync(userId, token), Times.Once);
    }

    [Theory]
    [InlineData("")]
    [InlineData(null)]
    public async Task ForgotPassword_EmptyOrNullEmail_CallsServiceWithProvidedValue(string email)
    {
        var expectedResponse = new Response<string>(ResponseCode.BadRequest, "Invalid email");

        _mockAuthService.Setup(x => x.GeneratePasswordResetTokenAsync(It.IsAny<string>())).ReturnsAsync(expectedResponse);

        var result = await _controller.ForgotPassword(email);

        Assert.NotNull(result);
        _mockAuthService.Verify(x => x.GeneratePasswordResetTokenAsync(email), Times.Once);
    }
}
