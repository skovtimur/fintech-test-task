using System.Security.Cryptography;
using System.Text;
using FintechTestTask.Application.Abstractions.Hashing;

namespace FintechTestTask.Application.Services;

public class HashingManagerService : IHasher, IHashVerify
{
    public string? Hashing(string str)
    {
        if (string.IsNullOrEmpty(str))
            return null;

        using var sha256 = SHA256.Create();
        var bytes = Encoding.UTF8.GetBytes(str);
        var hashBytes = sha256.ComputeHash(bytes);
        return Convert.ToHexString(hashBytes);
    }

    public bool Verify(string str, string hashStr)
    {
        var comparableHash = Hashing(str);
        return comparableHash == hashStr;
    }
}