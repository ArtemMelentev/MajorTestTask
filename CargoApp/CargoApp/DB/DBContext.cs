using System.IO;
using System.Text.Json;
using CargoApp.Models;
using CargoApp.Utilities;
using CargoApp.Utilities.ExtensionClasses;
using Microsoft.EntityFrameworkCore;

namespace CargoApp.DB;

public class DBContext : DbContext
{
    private readonly string _connectionString;
    public DbSet<OrderModel> Orders { get; set; }
    
    public DBContext()
    {
        _connectionString = GetConnectionString();
    }
    
    public DBContext(string connectionString)
    {
        _connectionString = connectionString;
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
            if (entry is not { Entity: OrderModel order, State: EntityState.Added or EntityState.Modified })
            {
                continue;
            }
            if (!order.IsCorrect())
            {
                continue;
            }
            order.CreationDate = order.CreationDate.ToUniversalTime();
        }

        return await base.SaveChangesAsync(cancellationToken);
    }

    private static string GetConnectionString()
    {
        if (!File.Exists(GlobalConstants.DBJsonFilePath))
        {
            return String.Empty;
        }
        
        var json = File.ReadAllText(GlobalConstants.DBJsonFilePath);
        var config = JsonSerializer.Deserialize<DbConfig>(json);
        if (config is null)
        {
            return String.Empty;
        }
        
        var connectionString = $"Host={config.Host};Port={config.Port};Username={config.Username};" +
                               $"Password={config.Password};Database={config.DatabaseName};";

        return connectionString;
    }
}
