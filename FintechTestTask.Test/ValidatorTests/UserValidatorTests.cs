using FintechTestTask.Domain.Entities;
using RandomString4Net;

namespace FintechTestTask.Test.ValidatorTests;

public class UserValidatorTests
{
    [Theory]
    [MemberData(nameof(GetRandomUser))]
    public void User_Validator_Should_Return_New_User_Test(string name)
    {
        // Act
        var hash = "hashed-password";
        UserEntity? newUser = UserEntity.Create(name, hash);
        
        // Assert
        Assert.NotNull(newUser);
        Assert.True(Guid.TryParse(newUser.Id.ToString(), out Guid guid));
        Assert.NotEmpty(newUser.Name);
        Assert.NotEmpty(newUser.HashPassword);
    }
    
    [Fact]
    public void User_Validator_Should_Return_Null_Test()
    {
        // Act
        UserEntity? newUser = UserEntity.Create("1", string.Empty);
        
        // Assert
        Assert.Null(newUser);
    }

    public static IEnumerable<object[]> GetRandomUser()
    {
        yield return
        [
            RandomString.GetString(Types.ALPHABET_MIXEDCASE, randomLength: false, maxLength: 10),
        ];
    }
}