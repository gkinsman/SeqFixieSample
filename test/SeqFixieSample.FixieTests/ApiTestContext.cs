using Microsoft.AspNetCore.Mvc.Testing;
using Serilog;
using Serilog.Events;
using Serilog.Sinks.SystemConsole.Themes;

namespace SeqFixieSample.Tests;

public class ApiTestContext : IDisposable
{
    public ApiTestContext()
    {
        var app = new WebApplicationFactory<Program>()
            .WithWebHostBuilder(builder => { });
        Client = app.CreateClient();

        Logger = Log.ForContext<ApiTestContext>();
    }

    public HttpClient Client { get; }

    public ILogger Logger { get; }

    public void Dispose()
    {
        Client.Dispose();
    }
}