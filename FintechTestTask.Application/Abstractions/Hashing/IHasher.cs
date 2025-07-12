namespace FintechTestTask.Application.Abstractions.Hashing;

public interface IHashVerify
{
    public bool Verify(string str, string hashStr);
}