using Instancer.Server.Controllers;
using Instancer.Server.Dtos;
using Instancer.Server.Persistence;
using Instancer.Server.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace Instancer.Server.Tests;

public class ControllersTests
{
    private InstancerDbContext CreateDbContext()
    {
        var options = new DbContextOptionsBuilder<InstancerDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;
        return new InstancerDbContext(options);
    }

    [Fact]
    public async Task StackController_Create_InvalidRequest_ReturnsBadRequest()
    {
        using var db = CreateDbContext();
        var controller = new StackController(new StackService(db, new FakeOrchestrator()));

        var result = await controller.Create(new CreateStackRequest { Name = "", Template = "" });

        Assert.IsType<BadRequestObjectResult>(result);
    }

    [Fact]
    public async Task StackController_Create_ValidRequest_ReturnsOk()
    {
        using var db = CreateDbContext();
        var controller = new StackController(new StackService(db, new FakeOrchestrator()));

        var result = await controller.Create(new CreateStackRequest { Name = "test", Template = "fastapi-template" });

        var ok = Assert.IsType<OkObjectResult>(result);
        Assert.Equal(1, db.StackInstances.Count());
        Assert.NotNull(ok.Value);
    }

    [Fact]
    public void TemplatesController_GetAll_ReturnsOk()
    {
        var originalDir = Directory.GetCurrentDirectory();
        var serverDir = Path.Combine(originalDir, "Instancer.Server");
        if (Directory.Exists(serverDir))
            Directory.SetCurrentDirectory(serverDir);
        var templateService = new TemplateService();
        var controller = new TemplatesController(templateService);

        var result = controller.GetAll();

        Directory.SetCurrentDirectory(originalDir);

        var ok = Assert.IsType<OkObjectResult>(result);
        var templates = Assert.IsAssignableFrom<IEnumerable<object>>(ok.Value!);
        Assert.NotEmpty(templates);
    }
}
