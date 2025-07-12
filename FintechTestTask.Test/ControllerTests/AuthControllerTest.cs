using System.Net;
using System.Net.Http.Json;
using FintechTestTask.Domain.Dtos;
using FintechTestTask.Test.WebApplicationFactories;
using FintechTestTask.WebAPI.Queries;
using RandomString4Net;

namespace FintechTestTask.Test.ControllerTests;

public class AuthControllerTest
{
    private readonly TestWebApplicationFactory factory = new();
    
    [Theory]
    [MemberData(nameof(GetData))]
    public async Task Create_New_Account_Test_Should_Be_Ok_And_Returns_Tokens(string randomName, string randomPassword)
    {
        //Arrange
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

        var tokens = await response.Content.ReadFromJsonAsync<JwtTokensDto>();

        Assert.NotNull(tokens);
        Assert.False(string.IsNullOrWhiteSpace(tokens.AccessToken));
        Assert.False(string.IsNullOrWhiteSpace(tokens.RefreshToken));
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