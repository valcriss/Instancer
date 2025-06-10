using Instancer.Server.Models;
using Instancer.Server.Orchestrators;

namespace Instancer.Server.Tests;

public class FakeOrchestrator : IOrchestrator
{
    public int GetAvailablePort(int minPort = 10000, int maxPort = 60000) => 1234;

    public Task<string> DeployStack(StackInstance instance, string compose)
        => Task.FromResult($"http://localhost:{instance.Port}");

    public Task<bool> DeleteStack(Guid instanceId) => Task.FromResult(true);
}
