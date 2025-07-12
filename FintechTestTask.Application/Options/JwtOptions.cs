using System.ComponentModel.DataAnnotations;
using System.Security.Cryptography;
using System.Text;
using Microsoft.IdentityModel.Tokens;

namespace FintechTestTask.Application.Options;

public class JwtOptions
{
    //General 
    [Required] public string Issuer { get; set; }

    //Access
    [Required] public string AlgorithmForAccessToken { get; set; }
    [Required] public int AccessTokenExpiresMinutes { get; set; }
    [Required] public string AccessTokenNameInCookies { get; set; }
    [Required] public string AccessTokenSecretKey { get; set; }

    public SymmetricSecurityKey GetAccessSymmetricSecurityKey() =>
        new SymmetricSecurityKey(Encoding.ASCII.GetBytes(AccessTokenSecretKey));


    //Refresh
    [Required] public string AlgorithmForRefreshToken { get; set; }
    [Required] public int RefreshTokenExpiresDays { get; set; }
    [Required] public string RefreshTokenNameInCookies { get; set; }
    [Required] public string RefreshTokenSecretKey { get; set; }

    public RsaSecurityKey GetRefreshAsymmetricSecurityKey()
    {
        var rsaProvider = new RSACryptoServiceProvider(512);
        return new RsaSecurityKey(rsaProvider);
    }
}