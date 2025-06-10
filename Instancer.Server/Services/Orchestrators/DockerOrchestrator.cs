using Instancer.Server.Models;
using Instancer.Server.Orchestrators;
using System.Diagnostics;
using System.Net;
using System.Net.Sockets;

namespace Instancer.Server.Services.Orchestrators
{
    public class DockerOrchestrator : IOrchestrator
    {
        public async Task<string> DeployStack(StackInstance instance)
        {
            var templateDir = Path.Combine("templates", instance.Template);
            var templateFile = Path.Combine(templateDir, "docker-compose.template.yml");

            var variables = new Dictionary<string, string>
            {
                ["PORT"] = GetAvailablePort().ToString(),
                ["APP_ENV"] = "development"
            };

            var composedFile = RenderTemplate(templateFile, variables, instance.Id.ToString());

            // Appel docker-compose
            var psi = new ProcessStartInfo
            {
                FileName = "docker-compose",
                Arguments = $"-f {composedFile} up -d",
                RedirectStandardOutput = true,
                RedirectStandardError = true
            };

            var proc = Process.Start(psi);
            await proc.WaitForExitAsync();

            return $"http://localhost:{variables["PORT"]}";
        }


        public Task<bool> DeleteStack(Guid instanceId)
        {
            // TODO: remove docker stack
            return Task.FromResult(true);
        }

        private string RenderTemplate(string templatePath, Dictionary<string, string> variables, string instanceId)
        {
            var template = File.ReadAllText(templatePath);

            foreach (var (key, value) in variables)
            {
                template = template.Replace($"${{{key}}}", value);
            }

            // Fichier de sortie
            var outputPath = Path.Combine("generated", $"stack-{instanceId}.yml");
            Directory.CreateDirectory("generated");
            File.WriteAllText(outputPath, template);
            return outputPath;
        }

        public int GetAvailablePort(int minPort = 10000, int maxPort = 60000)
        {
            var listener = new TcpListener(IPAddress.Loopback, 0);
            listener.Start();
            int port = ((IPEndPoint)listener.LocalEndpoint).Port;
            listener.Stop();

            // Facultatif : s'assurer qu’il est bien dans la plage souhaitée
            if (port < minPort || port > maxPort)
            {
                return GetAvailablePort(minPort, maxPort); // récursif si en dehors de la plage
            }

            return port;
        }
    }
}
