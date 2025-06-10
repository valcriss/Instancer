using System.Net;
using System.Net.Http;
using Xunit;
using CliProgram = global::Program;

namespace Instancer.Cli.Tests;

public class ProgramTests
{
    private class StubHandler : HttpMessageHandler
    {
        public Func<HttpRequestMessage, HttpResponseMessage> Handler { get; set; } = _ => new HttpResponseMessage(HttpStatusCode.OK);
        protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken) => Task.FromResult(Handler(request));
    }

    private static async Task<string> CaptureAsync(Func<Task> func)
    {
        var sw = new StringWriter();
        var original = Console.Out;
        Console.SetOut(sw);
        try { await func(); }
        finally { Console.SetOut(original); }
        return sw.ToString();
    }

    [Fact]
    public void BuildRootCommand_DefinesCommands()
    {
        var root = CliProgram.BuildRootCommand();
        Assert.Contains(root.Children, c => c.Name == "up");
        Assert.Contains(root.Children, c => c.Name == "down");
        Assert.Contains(root.Children, c => c.Name == "status");
        Assert.Contains(root.Children, c => c.Name == "proxy");
    }

    [Fact]
    public async Task Up_PrintsServerResponse()
    {
        var handler = new StubHandler { Handler = _ => new HttpResponseMessage(HttpStatusCode.OK) { Content = new StringContent("done") } };
        CliProgram.CreateServerClient = () => new HttpClient(handler) { BaseAddress = new Uri("http://localhost") };
        var output = await CaptureAsync(() => CliProgram.Up("demo", "tpl"));
        Assert.Contains("done", output);
    }

    [Fact]
    public async Task Down_PrintsMessages()
    {
        var okHandler = new StubHandler { Handler = _ => new HttpResponseMessage(HttpStatusCode.OK) };
        CliProgram.CreateServerClient = () => new HttpClient(okHandler) { BaseAddress = new Uri("http://localhost") };
        var ok = await CaptureAsync(() => CliProgram.Down(Guid.Empty));
        Assert.Contains("Stack removed", ok);

        var failHandler = new StubHandler { Handler = _ => new HttpResponseMessage(HttpStatusCode.InternalServerError) };
        CliProgram.CreateServerClient = () => new HttpClient(failHandler) { BaseAddress = new Uri("http://localhost") };
        var fail = await CaptureAsync(() => CliProgram.Down(Guid.Empty));
        Assert.Contains("Failed to remove", fail);
    }

    [Fact]
    public async Task Status_PrintsBody()
    {
        var handler = new StubHandler { Handler = _ => new HttpResponseMessage(HttpStatusCode.OK) { Content = new StringContent("[{}]") } };
        CliProgram.CreateServerClient = () => new HttpClient(handler) { BaseAddress = new Uri("http://localhost") };
        var output = await CaptureAsync(() => CliProgram.Status());
        Assert.Contains("[{}]", output);
    }

    [Fact]
    public async Task Proxy_PrintsResult()
    {
        var handler = new StubHandler { Handler = _ => new HttpResponseMessage(HttpStatusCode.OK) };
        CliProgram.CreateDaemonClient = () => new HttpClient(handler) { BaseAddress = new Uri("http://localhost") };
        var output = await CaptureAsync(() => CliProgram.Proxy(1, 2));
        Assert.Contains("Proxy started", output);
    }

    [Fact]
    public async Task Main_ParsesArguments()
    {
        var handler = new StubHandler { Handler = _ => new HttpResponseMessage(HttpStatusCode.OK) { Content = new StringContent("[]") } };
        CliProgram.CreateServerClient = () => new HttpClient(handler) { BaseAddress = new Uri("http://localhost") };
        var output = await CaptureAsync(() => CliProgram.Main(new[] { "status" }));
        Assert.Contains("[]", output);
    }
}
