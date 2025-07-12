using System.Security.Claims;
using System.Text.Encodings.Web;
using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace FintechTestTask.Application.Services;

public class TestAuthHandler : AuthenticationHandler<AuthenticationSchemeOptions>
{
    public TestAuthHandler(IOptionsMonitor<AuthenticationSchemeOptions> options, ILoggerFactory logger,
        UrlEncoder encoder, ISystemClock clock) : base(options, logger, encoder, clock)
    {
    }

    public TestAuthHandler(IOptionsMonitor<AuthenticationSchemeOptions> options, ILoggerFactory logger,
        UrlEncoder encoder) : base(options, logger, encoder)
    {
    }

    public static readonly Guid FirstPlayerId = Guid.NewGuid();
    public const string FirstPlayerName = "Player1";
    
    protected override Task<AuthenticateResult> HandleAuthenticateAsync()
    {
        var identity =
            new ClaimsIdentity(
                new List<Claim>
                {
                    new(nameof(JwtService.UserIdClaimType), FirstPlayerId.ToString()),
                    new(nameof(JwtService.UserNameClaimType), FirstPlayerName)
                }, "TestAuthType");

        var principal = new ClaimsPrincipal(identity);
        var ticket = new AuthenticationTicket(principal, "TestScheme");

        var result = AuthenticateResult.Success(ticket);
        return Task.FromResult(result);
    }
}