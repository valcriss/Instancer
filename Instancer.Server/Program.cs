using Instancer.Server.Orchestrators;
using Instancer.Server.Persistence;
using Instancer.Server.Services;
using Instancer.Server.Services.Orchestrators;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddDbContext<InstancerDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("Default")));

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "Instancer API",
        Version = "v1",
    });
});
builder.Services.AddSingleton<IOrchestrator, DockerOrchestrator>();
builder.Services.AddScoped<StackService>();
builder.Services.AddSingleton<TemplateService>();
var app = builder.Build();
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<InstancerDbContext>();
    db.Database.Migrate();
}
// Configure the HTTP request pipeline.

app.UseAuthorization();

app.MapControllers();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.Run();
