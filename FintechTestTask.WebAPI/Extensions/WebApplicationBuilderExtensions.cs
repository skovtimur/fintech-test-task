using FintechTestTask.Application;
using FintechTestTask.Application.Abstractions;
using FintechTestTask.Application.Abstractions.Hashing;
using FintechTestTask.Application.Abstractions.InterfacesOfAlgorithms;
using FintechTestTask.Application.Algorithms;
using FintechTestTask.Application.Services;
using FintechTestTask.Domain.Entities;
using FintechTestTask.Infrastructure.Services;
using Microsoft.AspNetCore.TestHost;

namespace FintechTestTask.WebAPI.Extensions;

public static class WebApplicationBuilderExtensions
{
    public static void AddDependencies(this WebApplicationBuilder builder)
    {
        builder.Services.AddSingleton<CriticalFailureGenerator>();
        builder.Services.AddSingleton<IHasher, HashingManagerService>();
        builder.Services.AddSingleton<IHashVerify, HashingManagerService>();

        builder.Services.AddScoped<JwtService>();
        builder.Services.AddScoped<IUserService, UserService>();
        builder.Services.AddScoped<IJwtTokensService, JwtTokensService>();
        builder.Services.AddScoped<IGameService, GameService>();
        builder.Services.AddScoped<ICacheService, CacheService>();
        
        builder.Services.AddSingleton<ITicTacToeGameLogic, TicTacToeGameLogic>();
    }
}