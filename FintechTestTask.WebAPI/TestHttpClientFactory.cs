using Microsoft.AspNetCore.Mvc.Testing;

namespace FintechTestTask.WebAPI;

public class TestHttpClientFactory(HttpContext context) : IHttpClientFactory
{
    public HttpClient CreateClient(string name)
    {
        var client = new HttpClient();
        client.BaseAddress = new Uri(context.Request.Host.Value ??
                                     throw new NullReferenceException("The Base Address is null."));
        
        return client;
    }
}