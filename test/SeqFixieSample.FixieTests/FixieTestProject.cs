using Fixie;
using Microsoft.AspNetCore.Routing;
using Serilog;
using Serilog.Context;
using Serilog.Events;
using Serilog.Sinks.SystemConsole.Themes;
using SerilogTimings;

namespace SeqFixieSample.Tests;

public class AppTestProject : ITestProject
{
    public void Configure(TestConfiguration configuration, TestEnvironment environment)
    {
        configuration.Conventions.Add<DefaultDiscovery, SingleConstructionExecution>();
    }

    class SingleConstructionExecution : IExecution
    {
        public async Task Run(TestSuite testSuite)
        {
            var context = new ApiTestContext();

            var mapEnricher = new MapEnricher();

            // Comes after the context creation so that we clobber the app logger from Program. 
            Log.Logger = new LoggerConfiguration()
                .WriteTo.Console(theme: AnsiConsoleTheme.Code)
                .Enrich.FromLogContext()
                .Enrich.WithProperty("AppName", "SeqFixieSample.Api")
                .Enrich.WithProperty("TestType", "Fixie")
                .Enrich.With(mapEnricher)
                .WriteTo.Seq("http://localhost:5341")
                .MinimumLevel.Override("Microsoft.AspNetCore", LogEventLevel.Warning)
                .CreateLogger();

            var testRunId = Guid.NewGuid().ToString();
            mapEnricher.Set("TestRunId", testRunId);
            Log.Information("Beginning test run {TestRunId}", testRunId);

            try {
                foreach(var testClass in testSuite.TestClasses) {
                    object instance = testClass.Type.GetConstructors()
                        .Any(ctor => ctor.GetParameters().Any())
                        ? testClass.Construct(context)
                        : testClass.Construct();

                    foreach(var test in testClass.Tests) {
                        mapEnricher.Set("TestName", test.Name);
                        mapEnricher.Set("TestClass", testClass.Type.Name);
                        using var op =
                            Operation.At(LogEventLevel.Information, LogEventLevel.Error)
                                .Begin("Executing test {TestName}", test.Name);

                        var result = await test.Run(instance);
                        if (result is Failed failure) op.Abandon(failure.Reason);
                        else op.Complete();
                    }
                }
            }
            finally {
                Log.Information("Cleaning up test context");
                context.Dispose();
                Log.CloseAndFlush();
            }
        }
    }
}