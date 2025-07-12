using System.ComponentModel.DataAnnotations;
using AutoMapper;
using FintechTestTask.Application.Abstractions;
using FintechTestTask.Application.Abstractions.Hashing;
using FintechTestTask.Application.Services;
using FintechTestTask.Domain.Dtos;
using FintechTestTask.Domain.Entities;
using FintechTestTask.Domain.Models;
using FintechTestTask.Infrastructure.Services;
using FintechTestTask.WebAPI.Filters;
using FintechTestTask.WebAPI.Queries;
using Microsoft.AspNetCore.Mvc;

namespace FintechTestTask.WebAPI.Controllers;

[ApiController]
[Route("/auth")]
public class AuthController(
    ILogger<AuthController> logger,
    IUserService userService,
    JwtService jwtService,
    IJwtTokensService jwtTokensService,
    CriticalFailureGenerator criticalFailureGenerator,
    IMapper mapper,
    IHasher hasher) : ControllerBase
{
    /// <summary>
    /// Create new account 
    /// </summary>
    /// <remarks>
    /// Simple request:
    ///     POST / CreateAccountQuery
    ///     {
    ///         "name": "MyName",
    ///         "password": "qwerty123"
    ///     }
    /// </remarks>
    /// <response code="400"> The Name is already in use </response>
    /// <response code="200"> Create and return new account</response> 
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ValidationFilter, OnlyForAnnonymousFilter,
     HttpPost("create-account")]
    public async Task<IActionResult> CreateAccount([Required, FromForm] CreateAccountQuery query)
    {
        var nameIsFree = await userService.NameIsFree(query.Name);

        if (nameIsFree == false)
            return BadRequest("This Name is already in use");

        var newUser = UserEntity.Create(query.Name, hasher.Hashing(query.Password));
        criticalFailureGenerator.ShutdownIfNullOrEmpty(newUser,
            "User may send invalid data, new user shouldn't be null");

        await userService.Create(newUser);
        var tokens = CreateTokens(newUser);

        var newRefreshToken =
            RefreshTokenEntity.Create(newUser, hasher.Hashing(tokens.RefreshToken), tokens.ExpiresAtUtc);
        criticalFailureGenerator.ShutdownIfNullOrEmpty(newRefreshToken, "Refresh token shouldn't be null");

        await jwtTokensService.AddRefreshToken(newRefreshToken);
        return Ok(mapper.Map<JwtTokensDto>(tokens));
    }


    // [ValidationFilter, OnlyForAnnonymousFilter,
    //  HttpGet("login")]
    // public async Task<IActionResult> Login([Required, FromForm] LoginQuery query)
    // {
    //     TODO
    // }

    /// <summary>
    /// Update JWT tokens by old refresh token
    /// </summary>
    /// <response code="404"> Token not found </response>
    /// <response code="400"> Refresh token has expired </response>
    /// <response code="200"> Create and return new tokens</response> 
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ValidationFilter, OnlyForAnnonymousFilter,
     HttpPut("update-token/{oldRefreshToken}")]
    public async Task<IActionResult> UpdateToken([Required] string oldRefreshToken)
    {
        var hashedOldRefreshToken = hasher.Hashing(oldRefreshToken);
        criticalFailureGenerator.ShutdownIfNullOrEmpty(oldRefreshToken, "Old RefreshToken may not be null");

        var tokenEntity = await jwtTokensService.Get(hashedOldRefreshToken);

        if (tokenEntity is null)
            return NotFound("Token not found");

        if (tokenEntity.ExpiresAtUtc <= DateTime.UtcNow)
            return BadRequest("Your refresh token has expired");

        var tokens = CreateTokens(tokenEntity.User);
        var hashedNewRefreshToken = hasher.Hashing(tokens.RefreshToken);

        await jwtTokensService.UpdateRefreshToken(tokenEntity, hashedNewRefreshToken, tokens.ExpiresAtUtc);
        return Ok(mapper.Map<JwtTokensDto>(tokens));
    }

    private JwtTokens CreateTokens(UserEntity user)
    {
        var tokens = jwtService.GenerateTokens(user);

        const string emptyTokenText = "The token is empty, so check JwtService";
        criticalFailureGenerator.ShutdownIfNullOrEmpty(tokens.RefreshToken, emptyTokenText);
        criticalFailureGenerator.ShutdownIfNullOrEmpty(tokens.AccessToken, emptyTokenText);

        return new JwtTokens(refreshToken: tokens.RefreshToken,
            accessToken: tokens.AccessToken, tokens.ExpiresAtUtc);
    }
}