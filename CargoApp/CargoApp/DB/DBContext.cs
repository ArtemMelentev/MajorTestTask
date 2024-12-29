using System.IO;
using System.Text.Json;
using CargoApp.Models;
using CargoApp.Utilities;
using Microsoft.EntityFrameworkCore;

namespace CargoApp.DB;

public class DBContext : DbContext
{
    private readonly string _connectionString;
    public DbSet<OrderModel> Orders { get; set; }
    
    public DBContext(string connectionString)
    {
        _connectionString = GetConnectionString();
    }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.UseNpgsql(_connectionString);
    }
    
    public override int SaveChanges()
    {
        foreach (var entry in ChangeTracker.Entries())
        {
            if (entry is { Entity: OrderModel order, State: EntityState.Added or EntityState.Modified })
            {
                order.CreationDate = order.CreationDate.ToUniversalTime();
            }
        }

        return base.SaveChanges();
    }

    public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        foreach (var entry in ChangeTracker.Entries())
        {
            if (entry is { Entity: OrderModel order, State: EntityState.Added or EntityState.Modified })
            {
                order.CreationDate = order.CreationDate.ToUniversalTime();
            }
        }

        return await base.SaveChangesAsync(cancellationToken);
    }

    private static string GetConnectionString()
    {
        if (!File.Exists(GlobalConstants.DBJsonFilePath))
        {
            return String.Empty;
        }
        
        var config = JsonSerializer.Deserialize<DbConfig>(GlobalConstants.DBJsonFilePath);
        if (config is null)
        {
            return String.Empty;
        }
        
        var connectionString = $"Host={config.Host};Port={config.Port};Username={config.Username};" +
                               $"Password={config.Password};Database={config.DatabaseName};";

        return connectionString;
    }
}
