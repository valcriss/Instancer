using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Net.Http.Json;
using Instancer.Daemon;
using Microsoft.AspNetCore.TestHost;

namespace Instancer.Daemon.Tests;

public class ProgramTests
{
    private static int GetFreePort()
    {
        var l = new TcpListener(IPAddress.Loopback, 0);
        l.Start();
        int p = ((IPEndPoint)l.LocalEndpoint).Port;
        l.Stop();
        return p;
    }

    [Fact]
    public async Task StartStopProxy_ForwardDataAndPersist()
    {
        var tempDir = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString());
        Directory.CreateDirectory(tempDir);
        var lockFile = Path.Combine(tempDir, "proxy.lock.json");
        var manager = new ProxyManager(lockFile);

        var echo = new TcpListener(IPAddress.Loopback, 0);
        echo.Start();
        int remote = ((IPEndPoint)echo.LocalEndpoint).Port;
        var echoTask = Task.Run(async () =>
        {
            using var c = await echo.AcceptTcpClientAsync();
            var s = c.GetStream();
            var buf = new byte[2];
            int len = await s.ReadAsync(buf);
            await s.WriteAsync(buf.AsMemory(0, len));
        });

        int local = GetFreePort();
        var app = Program.BuildApp(null, manager, useTestServer: true);
        await app.StartAsync();
        var client = app.GetTestClient();

        var resp = await client.PostAsJsonAsync("/start-proxy", new ProxyRequest(local, remote, "127.0.0.1"));
        Assert.True(resp.IsSuccessStatusCode);
        Assert.True(File.Exists(lockFile));

        using var sock = new TcpClient();
        await sock.ConnectAsync(IPAddress.Loopback, local);
        var stream = sock.GetStream();
        await stream.WriteAsync(Encoding.UTF8.GetBytes("ok"));
        var buffer = new byte[2];
        await stream.ReadAsync(buffer);
        Assert.Equal("ok", Encoding.UTF8.GetString(buffer));

        resp = await client.GetAsync("/status");
        var content = await resp.Content.ReadAsStringAsync();
        Assert.Contains(local.ToString(), content);

        resp = await client.PostAsJsonAsync("/stop-proxy", new StopRequest(local));
        Assert.True(resp.IsSuccessStatusCode);

        Assert.Equal("[]", await File.ReadAllTextAsync(lockFile));
        await app.StopAsync();
        echo.Stop();
        await echoTask;
    }

    [Fact]
    public async Task StartProxy_Twice_ReturnsConflict()
    {
        var manager = new ProxyManager(Path.GetTempFileName());
        var app = Program.BuildApp(null, manager, useTestServer: true);
        await app.StartAsync();
        var client = app.GetTestClient();
        int port = GetFreePort();
        int remote = GetFreePort();
        await client.PostAsJsonAsync("/start-proxy", new ProxyRequest(port, remote, "127.0.0.1"));
        var resp = await client.PostAsJsonAsync("/start-proxy", new ProxyRequest(port, remote, "127.0.0.1"));
        Assert.Equal(HttpStatusCode.Conflict, resp.StatusCode);
        await client.PostAsJsonAsync("/stop-proxy", new StopRequest(port));
        await app.StopAsync();
    }
}
