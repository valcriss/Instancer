using Instancer.Server.Models;
using Instancer.Server.Orchestrators;

namespace Instancer.Server.Tests;

public class FakeOrchestrator : IOrchestrator
{
    public Task<string> DeployStack(StackInstance instance)
        => Task.FromResult($"http://localhost:{instance.Port}");

    public Task<bool> DeleteStack(Guid instanceId) => Task.FromResult(true);
}
