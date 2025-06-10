using Instancer.Server.Models;
using Instancer.Server.Orchestrators;
using Instancer.Server.Persistence;
using Instancer.Server.Services.Orchestrators;

namespace Instancer.Server.Services
{
    public class StackService
    {
        private readonly InstancerDbContext _db;
        private readonly IOrchestrator _orchestrator;

        public StackService(InstancerDbContext db, IOrchestrator orchestrator)
        {
            _db = db;
            _orchestrator = orchestrator;
        }

        public IEnumerable<StackInstance> GetAll() => _db.StackInstances.ToList();

        public async Task<StackInstance> CreateAsync(string name, string compose, int port)
        {
            var instance = new StackInstance
            {
                Id = Guid.NewGuid(),
                Name = name,
                Compose = compose,
                Port = port,
                CreatedAt = DateTime.UtcNow
            };

            _db.StackInstances.Add(instance);
            await _db.SaveChangesAsync();

            return instance;
        }


        public async Task<string> CreateAndDeployAsync(string name, string compose)
        {
            int port = _orchestrator is DockerOrchestrator docker
                ? docker.GetAvailablePort()
                : new Random().Next(10000, 60000); // fallback

            var instance = await CreateAsync(name, compose, port);
            var url = await _orchestrator.DeployStack(instance, compose);
            return url;
        }

    }
}
