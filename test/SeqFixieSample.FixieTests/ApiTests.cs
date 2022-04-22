using System.Net;
using FluentAssertions;

namespace SeqFixieSample.Tests;

public class ApiTests
{
    readonly ApiTestContext _context;

    public ApiTests(ApiTestContext context)
    {
        _context = context;
    }
    
    public void ThisTestShouldAlwaysPass()
    {
        true.Should().BeTrue();
    }

    public void ThisTestShouldAlwaysFail()
    {
        true.Should().BeFalse();
    }

    public async Task LoadingANonExistentRouteShouldFail()
    {
        var result = await _context.Client.GetAsync("/non-existent");
        result.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    public async Task LoadingWebRootShouldSayHello()
    {
        var result = await _context.Client.GetStringAsync("/");
        result.Should().Be("Hello World!");
    }
}