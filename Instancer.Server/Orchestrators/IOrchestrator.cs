using Instancer.Server.Models;

namespace Instancer.Server.Orchestrators
{
    public interface IOrchestrator
    {
        Task<string> DeployStack(StackInstance instance, string compose);
        int GetAvailablePort(int minPort = 10000, int maxPort = 60000);
        Task<bool> DeleteStack(Guid instanceId);
    }
}
