using Instancer.Server.Models;

namespace Instancer.Server.Orchestrators
{
    public interface IOrchestrator
    {
        Task<string> DeployStack(StackInstance instance);
        Task<bool> DeleteStack(Guid instanceId);
    }
}
