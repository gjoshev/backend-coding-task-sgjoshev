using Microsoft.AspNetCore.Mvc.Testing;
using System.Net;
using System.Net.Http.Json;
using Xunit;

namespace Claims.Tests;

/// <summary>
/// Integration tests for the Claims API endpoint.
/// </summary>
public class ClaimsControllerTests
{
    [Fact]
    public async Task Get_Claims_ReturnsOk()
    {
        var application = new WebApplicationFactory<Program>()
            .WithWebHostBuilder(_ => { });

        var client = application.CreateClient();

        var response = await client.GetAsync("/Claims");

        response.EnsureSuccessStatusCode();
        var claims = await response.Content.ReadFromJsonAsync<List<Claim>>();
        Assert.NotNull(claims);
        Assert.Empty(claims);
    }

    [Fact]
    public async Task Get_ClaimById_NotFound()
    {
        var application = new WebApplicationFactory<Program>()
            .WithWebHostBuilder(_ => { });

        var client = application.CreateClient();

        var response = await client.GetAsync("/Claims/nonexistent-id");

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }
}
