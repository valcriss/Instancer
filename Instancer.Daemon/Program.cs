using Instancer.Daemon;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.TestHost;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;

public partial class Program
{
    public static WebApplication BuildApp(string[]? args = null, ProxyManager? manager = null, bool useTestServer = false)
    {
        var builder = WebApplication.CreateBuilder(args ?? Array.Empty<string>());
        if (useTestServer)
            builder.WebHost.UseTestServer();
        builder.Services.AddSingleton(manager ?? new ProxyManager());
        var app = builder.Build();

        app.MapPost("/start-proxy", async (ProxyManager pm, ProxyRequest req) =>
        {
            var ok = await pm.StartProxyAsync(req.Local, req.Remote, req.Host ?? "localhost");
            return ok ? Results.Ok() : Results.Conflict();
        });

        app.MapPost("/stop-proxy", (ProxyManager pm, StopRequest req) =>
        {
            var ok = pm.StopProxy(req.Local);
            return ok ? Results.Ok() : Results.NotFound();
        });

        app.MapGet("/status", (ProxyManager pm) => Results.Json(pm.GetStatus()));

        return app;
    }

    public static Task Main(string[] args)
    {
        var app = BuildApp(args);
        return app.RunAsync("http://localhost:5151");
    }
}

public record ProxyRequest(int Local, int Remote, string? Host);
public record StopRequest(int Local);
