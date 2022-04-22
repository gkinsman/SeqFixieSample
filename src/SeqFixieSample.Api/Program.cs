using Serilog;
using Serilog.Events;
using Serilog.Sinks.SystemConsole.Themes;
using SerilogTimings;

var builder = WebApplication.CreateBuilder(args);

Log.Logger = new LoggerConfiguration()
    .WriteTo.Console(theme: AnsiConsoleTheme.Code)
    .Enrich.FromLogContext()
    .Enrich.WithProperty("AppName", "SeqFixieSample.Api")
    .WriteTo.Seq("http://localhost:5341")
    .MinimumLevel.Override("Microsoft.AspNetCore", LogEventLevel.Warning)
    .CreateLogger();

builder.Host.UseSerilog();

var app = builder.Build();

app.UseSerilogRequestLogging();
app.UseRouting();

app.MapGet("/", async context => {
    using (Operation.Time("Loading welcome message!")) {
        await Task.Delay(Random.Shared.Next(10, 50));
        await context.Response.WriteAsync("Hello World!");
    }
});

app.Run();