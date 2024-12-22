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
}
