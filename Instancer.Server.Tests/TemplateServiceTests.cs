using Instancer.Server.Services;
using Xunit;

namespace Instancer.Server.Tests;

public class TemplateServiceTests
{
    [Fact]
    public void GetTemplates_ReturnsAtLeastOneTemplate()
    {
        // Arrange
        var originalDir = Directory.GetCurrentDirectory();
        var serverDir = Path.Combine(originalDir, "Instancer.Server");
        if (Directory.Exists(serverDir))
        {
            Directory.SetCurrentDirectory(serverDir);
        }
        var service = new TemplateService();

        // Act
        var templates = service.GetTemplates().ToList();

        // Cleanup
        Directory.SetCurrentDirectory(originalDir);

        // Assert
        Assert.True(templates.Count > 0);
        Assert.All(templates, t => Assert.False(string.IsNullOrWhiteSpace(t.Id)));
    }
}
