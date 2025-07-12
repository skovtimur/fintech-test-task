using FakeItEasy;
using FintechTestTask.Domain.Entities;
using RandomString4Net;

namespace FintechTestTask.Test.ValidatorTests;

public class RefreshTokenValidatorTests
{
    [Theory]
    [MemberData(nameof(GetRandomToken))]
    public void RefreshToken_Validator_Should_Return_New_Token_Test(string name)
    {
        // Arrange
        var user = A.Fake<UserEntity>();
        var hash = "hash-string";
        var expiresAtUtc = DateTime.UtcNow.AddDays(30);

        // Act
        RefreshTokenEntity? newToken = RefreshTokenEntity.Create(user, hash, expiresAtUtc);

        // Assert
        Assert.NotNull(newToken);
        Assert.NotEmpty(newToken.TokenHash);
        Assert.True(newToken.ExpiresAtUtc > DateTime.UtcNow);
        Assert.True(Guid.TryParse(newToken.UserId.ToString(), out Guid guid));
    }

    [Fact]
    public void RefreshToken_Validator_Should_Return_Null_Test()
    {
        // Arrange
        var user = A.Fake<UserEntity>();
        var hash = string.Empty;
        var expiresAtUtc = new DateTime(1889, 4, 20);

        // Act
        RefreshTokenEntity? newToken = RefreshTokenEntity.Create(user, hash, expiresAtUtc);

        // Assert
        Assert.Null(newToken);
    }

    public static IEnumerable<object[]> GetRandomToken()
    {
        yield return
        [
            RandomString.GetString(Types.ALPHABET_MIXEDCASE, randomLength: false, maxLength: 10),
        ];
    }
}