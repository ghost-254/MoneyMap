using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using MoneyMap.Api.DTOs.Auth;
using MoneyMap.Tests.Infrastructure;

namespace MoneyMap.Tests.Auth;

public sealed class AuthEndpointsTests(CustomWebApplicationFactory factory) : IClassFixture<CustomWebApplicationFactory>
{
    [Fact]
    public async Task Register_And_Login_ReturnJwtToken()
    {
        await factory.ResetDatabaseAsync();
        using var client = factory.CreateClient();

        var registerResponse = await client.PostAsJsonAsync("/api/auth/register", new
        {
            fullName = "Ada Lovelace",
            email = "ada@example.com",
            password = "Password123!"
        });

        Assert.Equal(HttpStatusCode.Created, registerResponse.StatusCode);

        var registerPayload = await registerResponse.Content.ReadFromJsonAsync<AuthResponseDto>();
        Assert.NotNull(registerPayload);
        Assert.False(string.IsNullOrWhiteSpace(registerPayload!.AccessToken));
        Assert.Equal("ada@example.com", registerPayload.User.Email);

        var loginResponse = await client.PostAsJsonAsync("/api/auth/login", new
        {
            email = "ada@example.com",
            password = "Password123!"
        });

        Assert.Equal(HttpStatusCode.OK, loginResponse.StatusCode);

        var loginPayload = await loginResponse.Content.ReadFromJsonAsync<AuthResponseDto>();
        Assert.NotNull(loginPayload);
        Assert.False(string.IsNullOrWhiteSpace(loginPayload!.AccessToken));
    }

    [Fact]
    public async Task Protected_Endpoints_Require_AccessToken()
    {
        await factory.ResetDatabaseAsync();
        using var client = factory.CreateClient();

        var response = await client.GetAsync("/api/categories");

        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }
}
