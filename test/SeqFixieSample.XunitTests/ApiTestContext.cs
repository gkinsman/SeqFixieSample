using System;
using System.Net.Http;
using Microsoft.AspNetCore.Mvc.Testing;
using Serilog;
using Serilog.Events;
using Serilog.Sinks.SystemConsole.Themes;
using Xunit.Abstractions;

namespace SeqFixieSample.XunitTests;

public class ApiTestContext : IDisposable
{
    public ApiTestContext(ITestOutputHelper outputHelper)
    {
        var app = new WebApplicationFactory<Program>()
            .WithWebHostBuilder(builder => { });
        Client = app.CreateClient();

        // Comes after the app build so we clobber the app logger created there
        Log.Logger = new LoggerConfiguration()
            .WriteTo.Console(theme: AnsiConsoleTheme.Code)
            .Enrich.FromLogContext()
            .Enrich.WithProperty("AppName", "SeqFixieSample.Api")
            .Enrich.WithProperty("TestType", "xUnit")
            .WriteTo.Seq("http://localhost:5341")
            .WriteTo.TestOutput(outputHelper)
            .MinimumLevel.Override("Microsoft.AspNetCore", LogEventLevel.Warning)
            .CreateLogger();
        
        Logger = Log.ForContext<ApiTestContext>();
    }

    public HttpClient Client { get;  }

    public ILogger Logger { get; }

    public void Dispose()
    {
        Client.Dispose();
        Log.CloseAndFlush();
    }
}