using FintechTestTask.Application.Abstractions.Hashing;
using FintechTestTask.Application.Services;
using FintechTestTask.Domain.Entities;
using FintechTestTask.Infrastructure;
using FintechTestTask.WebAPI;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace FintechTestTask.Test.WebApplicationFactories;

public class TestWebApplicationFactory(bool disableAuth = false) : WebApplicationFactory<Program>
{
    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.UseEnvironment("Testing");

        builder.ConfigureTestServices(x =>
        {
            x.AddLogging();
            
            if (disableAuth)
            {
                x.AddAuthentication("TestScheme")
                    .AddScheme<AuthenticationSchemeOptions, TestAuthHandler>("TestScheme", options => { });
            }

            var sp = x.BuildServiceProvider();
            using var scope = sp.CreateScope();

            var scopedServices = scope.ServiceProvider;
            var db = scopedServices.GetRequiredService<MainDbContext>();

            db.Database.EnsureCreated();
            AddDefaultData(db);
        });
    }

    private void AddDefaultData(MainDbContext db)
    {
        var firstPlayer = UserEntity.Create(TestAuthHandler.FirstPlayerName, "hashpassword");
        firstPlayer.Id = TestAuthHandler.FirstPlayerId;


        var users = new List<UserEntity>()
        {
            firstPlayer,
            UserEntity.Create("Player2", "hashPassword"),
            UserEntity.Create("Player3", "hashPassword"),
            UserEntity.Create("Player4", "hashPassword"),
            UserEntity.Create("Player5", "hashPassword"),
        };

        var refreshTokens = users
            .Select(user => RefreshTokenEntity.Create(user,
                $"tokenHash-{user.Id}", DateTime.UtcNow.AddDays(7))).ToList();

        var games = new List<GameEntity>()
        {
            GameEntity.Create(users[0], 3),
            GameEntity.Create(users[2], 5)
        };

        db.Users.AddRange(users);
        db.RefreshTokens.AddRange(refreshTokens);
        db.Games.AddRange(games);

        db.SaveChanges();
    }
}