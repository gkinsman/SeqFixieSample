using System;
using System.Threading.Tasks;
using FluentAssertions;
using Xunit;
using Xunit.Abstractions;

namespace SeqFixieSample.XunitTests;

public class ApiTests : IDisposable
{
    readonly ApiTestContext _context;

    public ApiTests(ITestOutputHelper outputHelper)
    {
        _context = new ApiTestContext(outputHelper);
    }

    [Fact]
    public void ThisTestShouldAlwaysPass()
    {
        true.Should().BeTrue();
    }

    [Fact]
    public async Task LoadingWebRootShouldSayHello()
    {
        var result = await _context.Client.GetStringAsync("/");
        result.Should().Be("Hello World!");
    }

    public void Dispose()
    {
        _context.Dispose();
    }
}