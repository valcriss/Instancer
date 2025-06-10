using Instancer.Server.Models;
using Instancer.Server.Orchestrators;
using System.Diagnostics;
using System.Net;
using System.Net.Sockets;

namespace Instancer.Server.Services.Orchestrators
{
    public class DockerOrchestrator : IOrchestrator
    {
        public async Task<string> DeployStack(StackInstance instance, string compose)
        {
            var composedFile = SaveCompose(compose, instance.Id.ToString());

            // Appel docker-compose
            var psi = new ProcessStartInfo
            {
                FileName = "docker-compose",
                Arguments = $"-f {composedFile} up -d",
                RedirectStandardOutput = true,
                RedirectStandardError = true
            };

            var proc = Process.Start(psi);
            if (proc != null)
            {
                await proc.WaitForExitAsync();
            }

            return $"http://localhost:{instance.Port}";
        }


        public async Task<bool> DeleteStack(Guid instanceId)
        {
            var composeFile = Path.Combine("generated", $"stack-{instanceId}.yml");
            if (!File.Exists(composeFile))
            {
                return false;
            }

            var psi = new ProcessStartInfo
            {
                FileName = "docker-compose",
                Arguments = $"-f {composeFile} down",
                RedirectStandardOutput = true,
                RedirectStandardError = true
            };

            var proc = Process.Start(psi);
            if (proc != null)
            {
                await proc.WaitForExitAsync();
                if (proc.ExitCode == 0)
                {
                    File.Delete(composeFile);
                    return true;
                }
            }

            return false;
        }

        private string SaveCompose(string content, string instanceId)
        {
            var outputPath = Path.Combine("generated", $"stack-{instanceId}.yml");
            Directory.CreateDirectory("generated");
            File.WriteAllText(outputPath, content);
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
