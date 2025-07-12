using FintechTestTask.Application.Abstractions;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace FintechTestTask.Application.Services;

public class CriticalFailureGenerator(
    ILogger<CriticalFailureGenerator> logger,
    IHostApplicationLifetime applicationLifetime) : ICriticalFailureGenerator
{
    public void ApplicationShutdown(string errorText, params object[] objects)
    {
        logger.LogCritical(errorText, objects);
        logger.LogInformation("Application shutdown");

        applicationLifetime.StopApplication();
    }

    public void ShutdownIfNullOrEmpty(object? value, string errorText, params object[] objects)
    {
        if (value == null || (value is string str && string.IsNullOrEmpty(str)))
            ApplicationShutdown(errorText, objects);
    }
}