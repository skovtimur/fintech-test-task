using FintechTestTask.Application.Abstractions.Hashing;
using FintechTestTask.Application.Services;
using RandomString4Net;

namespace FintechTestTask.Test.ServiceTests;

public class HashingManagerServiceTests
{
    [Theory]
    [MemberData(nameof(RandomString))]
    public void Verify_Hashed_Strings_Should_Be_Succesfully_Test(string randomStr)
    {
        var hashingManager = new HashingManagerService();
        IHasher hasher = hashingManager;
        IHashVerify hashVerify = hashingManager;

        var randomStrHash = hasher.Hashing(randomStr);
        Assert.True(hashVerify.Verify(str: randomStr, hashStr: randomStrHash));
    }

    public static IEnumerable<object[]> RandomString()
    {
        yield return
        [
            RandomString4Net.RandomString.GetString(Types.ALPHABET_MIXEDCASE, randomLength: true),
        ];
    }
}