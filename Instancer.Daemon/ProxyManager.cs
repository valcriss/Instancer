using System.Net;
using System.Net.Sockets;
using System.Text.Json;

namespace Instancer.Daemon;

public record ProxyInfo(int Local, int Remote, string Host);

internal class Proxy
{
    public int Local { get; private set; }
    public int Remote { get; }
    public string Host { get; }
    private TcpListener? _listener;
    private CancellationTokenSource? _cts;

    public Proxy(int local, int remote, string host)
    {
        Local = local;
        Remote = remote;
        Host = host;
    }

    public async Task StartAsync()
    {
        _listener = new TcpListener(IPAddress.Loopback, Local);
        _listener.Start();
        Local = ((IPEndPoint)_listener.LocalEndpoint).Port;
        _cts = new CancellationTokenSource();
        _ = Task.Run(() => AcceptLoop(_cts.Token));
    }

    private async Task AcceptLoop(CancellationToken token)
    {
        while (!token.IsCancellationRequested)
        {
            TcpClient client;
            try
            {
                client = await _listener!.AcceptTcpClientAsync(token);
            }
            catch
            {
                break;
            }
            _ = Task.Run(() => HandleClient(client, token));
        }
    }

    private async Task HandleClient(TcpClient localClient, CancellationToken token)
    {
        using var remoteClient = new TcpClient();
        await remoteClient.ConnectAsync(Host, Remote, token);
        using var local = localClient;
        var ls = local.GetStream();
        var rs = remoteClient.GetStream();
        var t1 = ls.CopyToAsync(rs, token);
        var t2 = rs.CopyToAsync(ls, token);
        await Task.WhenAny(t1, t2);
    }

    public void Stop()
    {
        try { _cts?.Cancel(); } catch { }
        try { _listener?.Stop(); } catch { }
    }
}

public class ProxyManager
{
    private readonly Dictionary<int, Proxy> _proxies = new();
    private readonly string _path;

    public ProxyManager(string? filePath = null)
    {
        var home = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile);
        var dir = Path.Combine(home, ".instancer");
        _path = filePath ?? Path.Combine(dir, "proxy.lock.json");
    }

    public IEnumerable<ProxyInfo> GetStatus() => _proxies.Values.Select(p => new ProxyInfo(p.Local, p.Remote, p.Host));

    private void Save()
    {
        var dir = Path.GetDirectoryName(_path)!;
        Directory.CreateDirectory(dir);
        var list = GetStatus().ToList();
        File.WriteAllText(_path, JsonSerializer.Serialize(list));
    }

    public async Task<bool> StartProxyAsync(int local, int remote, string host)
    {
        if (_proxies.ContainsKey(local))
            return false;
        var proxy = new Proxy(local, remote, host);
        await proxy.StartAsync();
        _proxies[proxy.Local] = proxy;
        Save();
        return true;
    }

    public bool StopProxy(int local)
    {
        if (!_proxies.TryGetValue(local, out var proxy))
            return false;
        proxy.Stop();
        _proxies.Remove(local);
        Save();
        return true;
    }
}
