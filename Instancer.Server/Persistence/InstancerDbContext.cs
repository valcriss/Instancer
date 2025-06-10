using Instancer.Server.Models;
using Microsoft.EntityFrameworkCore;

namespace Instancer.Server.Persistence
{
    public class InstancerDbContext : DbContext
    {
        public InstancerDbContext(DbContextOptions<InstancerDbContext> options) : base(options) { }

        public DbSet<StackInstance> StackInstances { get; set; }
    }
}
