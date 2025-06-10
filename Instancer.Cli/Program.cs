using System.CommandLine;
using System.Net;
using System.Net.Http.Json;
using System.Net.Sockets;
using System.Text.RegularExpressions;

public class Program
{
    internal static Func<HttpClient> CreateServerClient = () => new HttpClient { BaseAddress = new Uri("http://localhost:5000") };
    internal static Func<HttpClient> CreateDaemonClient = () => new HttpClient { BaseAddress = new Uri("http://localhost:5151") };

    public static Task<int> Main(string[] args)
    {
        var root = BuildRootCommand();
        return root.InvokeAsync(args);
    }

    internal static RootCommand BuildRootCommand()
    {
        var root = new RootCommand("Instancer CLI");

        var up = new Command("up", "Deploy a stack");
        var nameOpt = new Option<string>("--name") { IsRequired = true };
        var fileOpt = new Option<string>("--file") { IsRequired = true };
        up.AddOption(nameOpt);
        up.AddOption(fileOpt);
        up.SetHandler(async (string name, string file) => await Up(name, file), nameOpt, fileOpt);

        var down = new Command("down", "Delete a stack");
        var idOpt = new Option<Guid>("--id") { IsRequired = true };
        down.AddOption(idOpt);
        down.SetHandler(async (Guid id) => await Down(id), idOpt);

        var status = new Command("status", "List stacks");
        status.SetHandler(async () => await Status());

        var proxy = new Command("proxy", "Start a local proxy");
        var portOpt = new Option<int>("--port") { IsRequired = true };
        var targetOpt = new Option<int>("--target") { IsRequired = true };
        proxy.AddOption(portOpt);
        proxy.AddOption(targetOpt);
        proxy.SetHandler(async (int port, int target) => await Proxy(port, target), portOpt, targetOpt);

        root.AddCommand(up);
        root.AddCommand(down);
        root.AddCommand(status);
        root.AddCommand(proxy);

        return root;
    }

    static IEnumerable<int> GetHostPorts(string compose)
    {
        foreach (Match m in Regex.Matches(compose, "(\\d+):\\d+"))
            yield return int.Parse(m.Groups[1].Value);
    }

    static bool IsPortAvailable(int port)
    {
        try
        {
            var l = new TcpListener(IPAddress.Loopback, port);
            l.Start();
            l.Stop();
            return true;
        }
        catch
        {
            return false;
        }
    }

    internal static async Task Up(string name, string file)
    {
        var compose = File.ReadAllText(file);
        foreach (var port in GetHostPorts(compose))
        {
            if (!IsPortAvailable(port))
            {
                Console.WriteLine($"Port {port} unavailable");
                return;
            }
        }

        var client = CreateServerClient();
        var resp = await client.PostAsJsonAsync("/api/stacks", new { Name = name, Compose = compose });
        resp.EnsureSuccessStatusCode();
        var text = await resp.Content.ReadAsStringAsync();
        Console.WriteLine(text);
    }

    internal static async Task Down(Guid id)
    {
        var client = CreateServerClient();
        var resp = await client.DeleteAsync($"/api/stacks/{id}");
        if (resp.IsSuccessStatusCode)
            Console.WriteLine("Stack removed");
        else
            Console.WriteLine("Failed to remove stack");
    }

    internal static async Task Status()
    {
        var client = CreateServerClient();
        var resp = await client.GetAsync("/api/stacks");
        resp.EnsureSuccessStatusCode();
        var text = await resp.Content.ReadAsStringAsync();
        Console.WriteLine(text);
    }

    internal static async Task Proxy(int port, int target)
    {
        var client = CreateDaemonClient();
        var resp = await client.PostAsJsonAsync("/start-proxy", new { local = port, remote = target });
        if (resp.IsSuccessStatusCode)
            Console.WriteLine("Proxy started");
        else
            Console.WriteLine("Proxy failed");
    }
}
