namespace FintechTestTask.Application.Abstractions;

public interface ICriticalFailureGenerator
{
    public void ApplicationShutdown(string errorText, params object[] objects);
    public void ShutdownIfNullOrEmpty(object? value, string errorText, params object[] objects);
}