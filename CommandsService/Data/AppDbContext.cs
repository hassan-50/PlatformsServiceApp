using CommandsService.Models;
using Microsoft.EntityFrameworkCore;

namespace CommandsService.Data;
public class AppDbContext : DbContext  {
    public AppDbContext(DbContextOptions<AppDbContext> opt): base(opt)
    {
        
    }
    public DbSet<Platform> Platforms { get; set; } = null!;
    public DbSet<Command> Commands { get; set; } = null!;   

    protected override void OnModelCreating(ModelBuilder builder)
    {
        builder
        .Entity<Platform>()
        .HasMany(p => p.Commands)
        .WithOne(p => p.Platform!)
        .HasForeignKey(p=> p.PlatformId);

        builder
        .Entity<Command>()
        .HasOne(p => p.Platform)
        .WithMany(p => p.Commands)
        .HasForeignKey(p=> p.PlatformId);


    }

}