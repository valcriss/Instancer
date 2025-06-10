using Instancer.Server.Services.Orchestrators;
using System.Diagnostics;

namespace Instancer.Server.Tests;

public class DockerOrchestratorTests
{
    [Fact]
    public async Task DeleteStack_ComposeFileExists_RemovesFile()
    {
        var orchestrator = new DockerOrchestrator();
        var originalDir = Directory.GetCurrentDirectory();
        var serverDir = Path.Combine(originalDir, "Instancer.Server");
        if (Directory.Exists(serverDir))
            Directory.SetCurrentDirectory(serverDir);

        var instanceId = Guid.NewGuid();
        var generatedDir = Path.Combine("generated");
        Directory.CreateDirectory(generatedDir);
        var composeFile = Path.Combine(generatedDir, $"stack-{instanceId}.yml");
        File.WriteAllText(composeFile, "version: '3'");

        // Create fake docker-compose executable
        var tempDir = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString());
        Directory.CreateDirectory(tempDir);
        var script = Path.Combine(tempDir, "docker-compose");
        await File.WriteAllTextAsync(script, "#!/bin/sh\nexit 0\n");
        Process.Start("chmod", $"+x {script}")!.WaitForExit();
        var originalPath = Environment.GetEnvironmentVariable("PATH");
        Environment.SetEnvironmentVariable("PATH", tempDir + Path.PathSeparator + originalPath);

        try
        {
            var result = await orchestrator.DeleteStack(instanceId);
            Assert.True(result);
            Assert.False(File.Exists(composeFile));
        }
        finally
        {
            Environment.SetEnvironmentVariable("PATH", originalPath);
            Directory.SetCurrentDirectory(originalDir);
        }
    }

    [Fact]
    public async Task DeleteStack_ComposeFileMissing_ReturnsFalse()
    {
        var orchestrator = new DockerOrchestrator();
        var result = await orchestrator.DeleteStack(Guid.NewGuid());
        Assert.False(result);
    }
}

