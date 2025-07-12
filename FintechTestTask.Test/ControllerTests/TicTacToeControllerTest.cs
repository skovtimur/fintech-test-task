using System.Net;
using System.Net.Http.Json;
using FintechTestTask.Domain.Dtos;
using FintechTestTask.Infrastructure;
using FintechTestTask.Test.WebApplicationFactories;
using Microsoft.Extensions.DependencyInjection;

namespace FintechTestTask.Test.ControllerTests;

public class TicTacToeControllerTest
{
    private readonly TestWebApplicationFactory factory = new(disableAuth: true);

    [Fact]
    public async Task Get_Games_Test_Should_Be_Ok_And_Returns_Game()
    {
        // Act
        var httpClient = factory.CreateClient();
        var responseOfAvailableGames = await httpClient.GetAsync($"/tic-tac-toe");

        // Assert
        Assert.Equal(HttpStatusCode.OK, responseOfAvailableGames.StatusCode);
        var games = await responseOfAvailableGames.Content.ReadFromJsonAsync<List<GameDto>>();
        Assert.NotNull(games);
        Assert.True(games.Count >= 0);

        if (games.Count > 0)
        {
            var response = await httpClient.GetAsync($"/tic-tac-toe/{games[0].Id}");
            var game = await response.Content.ReadFromJsonAsync<GameDto>();
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            Assert.NotNull(game);
        }
    }

    public async Task Create_Game_Test_Should_Be_Created()
    {
        using var scope = factory.Services.CreateScope();

        var scopedServices = scope.ServiceProvider;
        var dbContext = scopedServices.GetRequiredService<MainDbContext>();
    }
}

