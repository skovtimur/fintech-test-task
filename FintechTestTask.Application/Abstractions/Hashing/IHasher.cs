namespace server_app.Application.Abstractions.Hashing;

public interface IHashVerify
{
    public bool Verify(string str, string hashStr);
}