using Entities;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Database;

public class MomentContext : DbContext
{
    DbSet<CoreMoment> Moments { get; set; }
    DbSet<MomentOwner> MomentOwners { get; set; }
    DbSet<MomentOwnership> MomentOwnerships { get; set; }
    
    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        var connectionString = Environment.GetEnvironmentVariable("SqlConnectionString");
        if (!string.IsNullOrEmpty(connectionString))
        {
            throw new InvalidOperationException("SqlConnectionString is not configured.");
        }
        
        optionsBuilder.UseSqlServer(connectionString);
    }
}