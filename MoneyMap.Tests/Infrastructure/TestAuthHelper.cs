using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json;
using System.Text.Json.Serialization;
using MoneyMap.Api.DTOs.Auth;

namespace MoneyMap.Tests.Infrastructure;

internal static class TestAuthHelper
{
    private static readonly JsonSerializerOptions JsonOptions = new(JsonSerializerDefaults.Web)
    {
        Converters = { new JsonStringEnumConverter() }
    };

    public static async Task<AuthResponseDto> RegisterAsync(
        HttpClient client,
        string email,
        string password = "Password123!",
        string fullName = "Test User")
    {
        var response = await client.PostAsJsonAsync("/api/auth/register", new
        {
            fullName,
            email,
            password
        });

        response.EnsureSuccessStatusCode();
        return await response.Content.ReadFromJsonAsync<AuthResponseDto>(JsonOptions)
            ?? throw new InvalidOperationException("Register response was empty.");
    }

    public static async Task AuthenticateAsync(
        HttpClient client,
        string email,
        string password = "Password123!",
        string fullName = "Test User")
    {
        var authResponse = await RegisterAsync(client, email, password, fullName);
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", authResponse.AccessToken);
    }
}
