using DietaCore.Shared.Common.Responses.ComplexTypes;
using DietaCore.Shared.Common.Responses.Concrete;
using System.Diagnostics;
using System.Net;
using System.Text;
using System.Text.Json;
using System.Text.RegularExpressions;

namespace DietaCore.Api.Middlewares
{
    public class GlobalExceptionMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<GlobalExceptionMiddleware> _logger;

        public GlobalExceptionMiddleware(RequestDelegate next, ILogger<GlobalExceptionMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            var stopwatch = Stopwatch.StartNew();
            var originalBodyStream = context.Response.Body;

            try
            {
                var requestBody = await ReadRequestBodyAsync(context);

                using var responseBody = new MemoryStream();
                context.Response.Body = responseBody;

                await _next(context);

                await LogRequestAsync(context, requestBody, responseBody, stopwatch, null);

                responseBody.Seek(0, SeekOrigin.Begin);
                await responseBody.CopyToAsync(originalBodyStream);
            }
            catch (Exception ex)
            {
                var requestBody = await ReadRequestBodyAsync(context);
                await LogRequestAsync(context, requestBody, null, stopwatch, ex);

                _logger.LogError(ex, "Unhandled exception occurred: {Message}", ex.Message);
                await HandleExceptionAsync(context, ex);
            }
            finally
            {
                context.Response.Body = originalBodyStream;
                stopwatch.Stop();
            }
        }

        private async Task<string> ReadRequestBodyAsync(HttpContext context)
        {
            try
            {
                context.Request.EnableBuffering();

                using var reader = new StreamReader(context.Request.Body, Encoding.UTF8, leaveOpen: true);
                var body = await reader.ReadToEndAsync();
                context.Request.Body.Position = 0;

                return body;
            }
            catch
            {
                return string.Empty;
            }
        }

        private async Task<string> ReadResponseBodyAsync(MemoryStream responseBody)
        {
            try
            {
                responseBody.Seek(0, SeekOrigin.Begin);
                using var reader = new StreamReader(responseBody, Encoding.UTF8, leaveOpen: true);
                var body = await reader.ReadToEndAsync();
                responseBody.Seek(0, SeekOrigin.Begin);

                return body;
            }
            catch
            {
                return string.Empty;
            }
        }

        private async Task LogRequestAsync(HttpContext context, string requestBody, MemoryStream? responseBody, Stopwatch stopwatch, Exception? exception)
        {
            try
            {
                var userId = GetUserId(context);
                var formattedResponseBody = responseBody != null ? await ReadResponseBodyAsync(responseBody) : "N/A";

                if (exception != null)
                {
                    formattedResponseBody = "Exception occurred - response not captured";
                }

                var logMessage = $"Request: {context.Request.Method} {context.Request.Path}{context.Request.QueryString}" +
                    $"\tBody: {MaskSensitiveData(requestBody)}" +
                    $"\tAcceptLanguage: {context.Request.Headers.AcceptLanguage}" +
                    $"\tSchema: {context.Request.Scheme}" +
                    $"\tHost: {context.Request.Host}" +
                    $"\tUserAgent: {context.Request.Headers.UserAgent}" +
                    $"\tContentType: {context.Request.ContentType}" +
                    $"\tResponse: StatusCode: {context.Response.StatusCode}" +
                    $"\tResponseContentType: {context.Response.ContentType}" +
                    $"\tResponseHeaders: {FormatHeaders(context.Response.Headers)}" +
                    $"\tResponseBody: {MaskSensitiveData(formattedResponseBody)}" +
                    $"\tDurationMs: {stopwatch.ElapsedMilliseconds}" +
                    $"\tUserId: {userId}" +
                    $"\tRemoteIpAddress: {context.Connection.RemoteIpAddress}" +
                    $"\tException: {(exception != null ? exception.GetType().Name : "None")}";

                if (exception != null)
                {
                    _logger.LogError("HTTP Request Failed: {LogMessage}", logMessage);
                }
                else if (context.Response.StatusCode >= 400)
                {
                    _logger.LogWarning("HTTP Request Warning: {LogMessage}", logMessage);
                }
                else
                {
                    _logger.LogInformation("HTTP Request: {LogMessage}", logMessage);
                }
            }
            catch (Exception logEx)
            {
                _logger.LogError(logEx, "Error occurred while logging request details");
            }
        }

        private string GetUserId(HttpContext context)
        {
            try
            {
                var userIdClaim = context.User?.FindFirst("sub") ??
                                 context.User?.FindFirst("userId") ??
                                 context.User?.FindFirst("id") ??
                                 context.User?.FindFirst("user_id") ??
                                 context.User?.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier);

                if (userIdClaim != null && !string.IsNullOrEmpty(userIdClaim.Value))
                    return userIdClaim.Value;

                if (context.Request.Headers.TryGetValue("Authorization", out var authHeader))
                {
                    var token = authHeader.ToString();
                    if (token.StartsWith("Bearer ", StringComparison.OrdinalIgnoreCase))
                    {
                        token = token.Substring(7);
                        var userId = ExtractUserIdFromJwtToken(token);
                        if (!string.IsNullOrEmpty(userId))
                            return userId;
                    }
                }
                if (context.Request.Headers.TryGetValue("X-User-Id", out var headerUserId))
                    return headerUserId.ToString();

                return "Anonymous";
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "Error extracting user ID from context");
                return "Unknown";
            }
        }

        private string MaskSensitiveData(string input)
        {
            if (string.IsNullOrEmpty(input))
                return input;

            try
            {
                var patterns = new Dictionary<string, string>
                {
                    { @"""password""\s*:\s*""[^""]*""", @"""password"":""***""" },
                    { @"""Password""\s*:\s*""[^""]*""", @"""Password"":""***""" },
                    { @"""token""\s*:\s*""[^""]*""", @"""token"":""***""" },
                    { @"""Token""\s*:\s*""[^""]*""", @"""Token"":""***""" },
                    { @"""accessToken""\s*:\s*""[^""]*""", @"""accessToken"":""***""" },
                    { @"""AccessToken""\s*:\s*""[^""]*""", @"""AccessToken"":""***""" },
                    { @"""refreshToken""\s*:\s*""[^""]*""", @"""refreshToken"":""***""" },
                    { @"""RefreshToken""\s*:\s*""[^""]*""", @"""RefreshToken"":""***""" },
                    { @"""creditCard""\s*:\s*""[^""]*""", @"""creditCard"":""***""" },
                    { @"""CreditCard""\s*:\s*""[^""]*""", @"""CreditCard"":""***""" },
                    { @"""ssn""\s*:\s*""[^""]*""", @"""ssn"":""***""" },
                    { @"""SSN""\s*:\s*""[^""]*""", @"""SSN"":""***""" }
                };

                var maskedInput = input;
                foreach (var pattern in patterns)
                {
                    maskedInput = Regex.Replace(maskedInput, pattern.Key, pattern.Value, RegexOptions.IgnoreCase);
                }
                if (maskedInput.Length > 1000)
                {
                    maskedInput = maskedInput.Substring(0, 1000) + "... [truncated]";
                }

                return maskedInput;
            }
            catch
            {
                return "[Error masking sensitive data]";
            }
        }

        private string FormatHeaders(IHeaderDictionary headers)
        {
            try
            {
                var formattedHeaders = new StringBuilder();
                foreach (var header in headers)
                {
                    if (IsSensitiveHeader(header.Key))
                    {
                        formattedHeaders.Append($"{header.Key}:*** ");
                    }
                    else
                    {
                        formattedHeaders.Append($"{header.Key}:{string.Join(",", header.Value.ToArray())} ");
                    }
                }
                return formattedHeaders.ToString().Trim();
            }
            catch
            {
                return "Error formatting headers";
            }
        }

        private bool IsSensitiveHeader(string headerName)
        {
            var sensitiveHeaders = new[]
            {
                "Authorization",
                "Cookie",
                "Set-Cookie",
                "X-API-Key",
                "X-Auth-Token",
                "Authentication"
            };

            return sensitiveHeaders.Contains(headerName, StringComparer.OrdinalIgnoreCase);
        }

        private static async Task HandleExceptionAsync(HttpContext context, Exception exception)
        {
            context.Response.ContentType = "application/json";

            var response = new Response<object>(
                data: null,
                responseCode: ResponseCode.Fail,
                message: "Internal server error occurred. Please try again later."
            );

            if (Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") == "Development")
            {
                response = new Response<object>(
                    data: null,
                    responseCode: ResponseCode.Fail,
                    message: exception.Message
                );
            }

            context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;

            var jsonResponse = JsonSerializer.Serialize(response, new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase
            });

            await context.Response.WriteAsync(jsonResponse);
        }

        private string ExtractUserIdFromJwtToken(string token)
        {
            try
            {
                if (string.IsNullOrEmpty(token))
                    return null;

                var parts = token.Split('.');
                if (parts.Length != 3)
                    return null;

                var payload = parts[1];

                switch (payload.Length % 4)
                {
                    case 2: payload += "=="; break;
                    case 3: payload += "="; break;
                }
                payload = payload.Replace('-', '+').Replace('_', '/');

                var jsonBytes = Convert.FromBase64String(payload);
                var json = Encoding.UTF8.GetString(jsonBytes);

                using var document = JsonDocument.Parse(json);
                var root = document.RootElement;

                var possibleClaims = new[] { "sub", "userId", "user_id", "id", "nameid" };

                foreach (var claim in possibleClaims)
                {
                    if (root.TryGetProperty(claim, out var element))
                    {
                        return element.GetString();
                    }
                }

                return null;
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "Error parsing JWT token for user ID");
                return null;
            }
        }
    }
}