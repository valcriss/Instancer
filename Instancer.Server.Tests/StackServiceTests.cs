using Instancer.Server.Models;
using Instancer.Server.Persistence;
using Instancer.Server.Services;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace Instancer.Server.Tests;

public class StackServiceTests
{
    private InstancerDbContext CreateDbContext()
    {
        var options = new DbContextOptionsBuilder<InstancerDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;
        return new InstancerDbContext(options);
    }

    [Fact]
    public async Task CreateAsync_PersistsInstance()
    {
        using var db = CreateDbContext();
        var service = new StackService(db, new FakeOrchestrator());

        var instance = await service.CreateAsync("test", "fastapi-template", 12345);

        Assert.Equal(1, db.StackInstances.Count());
        Assert.Equal("test", instance.Name);
        Assert.Equal(12345, instance.Port);
    }

    [Fact]
    public async Task CreateAndDeployAsync_AddsInstanceAndReturnsUrl()
    {
        using var db = CreateDbContext();
        var service = new StackService(db, new FakeOrchestrator());

        var url = await service.CreateAndDeployAsync("name", "fastapi-template");

        Assert.Equal(1, db.StackInstances.Count());
        var instance = db.StackInstances.First();
        Assert.Contains(instance.Port.ToString(), url);
    }
}
