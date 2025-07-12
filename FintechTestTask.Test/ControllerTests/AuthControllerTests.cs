using System.Net;
using FintechTestTask.Domain.Dtos;
using FintechTestTask.Domain.Models;
using FintechTestTask.Test.WebApplicationFactories;
using FintechTestTask.WebAPI.Queries;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using RandomString4Net;

namespace FintechTestTask.Test.ControllerTests;

public class AuthControllerTests
{
    private readonly TestWebApplicationFactory factory = new();

    [Theory]
    [MemberData(nameof(GetData))]
    public async Task Create_New_Account_Test_Should_Be_Ok_And_Returns_Tokens(string randomName, string randomPassword)
    {
        //Arrange
        var logger = factory.Services.GetRequiredService<ILogger<AuthControllerTests>>();
        var httpClient = factory.CreateClient();

        var formContent = new FormUrlEncodedContent([
            new KeyValuePair<string, string>(nameof(CreateAccountQuery.Name).ToLower(), randomName),
            new KeyValuePair<string, string>(nameof(CreateAccountQuery.Password).ToLower(), randomPassword)
        ]);

        //Act
        var response = await httpClient.PostAsync("/auth/create-account", formContent);

        //Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.Equal("application/json", response.Content.Headers.ContentType?.MediaType);

        var json = await response.Content.ReadAsStringAsync();
        var tokens = JsonConvert.DeserializeObject<JwtTokens>(json);

        logger.LogCritical(json);
        Assert.NotNull(tokens);
        Assert.False(string.IsNullOrEmpty(tokens.AccessToken));
        Assert.False(string.IsNullOrEmpty(tokens.RefreshToken));
        Assert.True(tokens.ExpiresAtUtc > DateTime.UtcNow);
    }

    [Fact]
    public async Task Create_New_Account_Test_Should_Returns_BadRequest()
    {
        //Arrange
        var httpClient = factory.CreateClient();

        var formContent = new FormUrlEncodedContent([
            new KeyValuePair<string, string>(nameof(CreateAccountQuery.Name).ToLower(), string.Empty),
            new KeyValuePair<string, string?>(nameof(CreateAccountQuery.Password).ToLower(), null)
        ]);

        //Act
        var response = await httpClient.PostAsync("/auth/create-account", formContent);

        //Assert
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    public static IEnumerable<object[]> GetData()
    {
        yield return
        [
            RandomString.GetString(Types.ALPHABET_MIXEDCASE, maxLength: 12, randomLength: false),
            RandomString.GetString(Types.ALPHABET_MIXEDCASE, maxLength: 22, randomLength: false)
        ];
    }
}