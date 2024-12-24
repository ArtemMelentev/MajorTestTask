using CargoApp.Models;
using Microsoft.EntityFrameworkCore;

namespace CargoApp.DB;

public class AppContext : DbContext
{
    public DbSet<OrderModel> Orders { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.UseNpgsql(
            "Host=localhost;Port=5432;Database=ordersdb;Username=postgres;Password=99Dipidi");
    }
    
    public override int SaveChanges()
    {
        foreach (var entry in ChangeTracker.Entries())
        {
            if (entry is { Entity: OrderModel order, State: EntityState.Added })
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
            if (entry is { Entity: OrderModel order, State: EntityState.Added })
            {
                order.CreationDate = order.CreationDate.ToUniversalTime();
            }
        }

        return await base.SaveChangesAsync(cancellationToken);
    }
}
