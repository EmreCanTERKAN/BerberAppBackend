using BerberApp_Backend.Domain.Abstractions;
using BerberApp_Backend.Domain.Employees;
using GenericRepository;
using Microsoft.EntityFrameworkCore;

namespace BerberApp_Backend.Infrastructure.Context;
internal sealed class ApplicationDbContext : DbContext, IUnitOfWork
{
    public ApplicationDbContext()
    {
    }

    public ApplicationDbContext(DbContextOptions options) : base(options)
    {
    }

    public DbSet<Employee> Employees { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(ApplicationDbContext).Assembly);
    }

    public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        var entries = ChangeTracker.Entries<Entity>();

        foreach (var entry in entries)
        {
            if (entry.State == EntityState.Added)
            {
                entry.Property(p => p.CreateAt).CurrentValue = DateTimeOffset.Now;
            }

            if (entry.State == EntityState.Modified)
            {
                if (entry.Property(p => p.IsDeleted).CurrentValue == true)
                {
                    entry.Property(p => p.DeleteAt).CurrentValue = DateTimeOffset.Now;
                }
                else
                {
                    entry.Property(p => p.UpdateAt).CurrentValue = DateTimeOffset.Now;
                }
            }

            if (entry.State == EntityState.Deleted)
            {
                entry.State = EntityState.Modified; // Soft delete
                entry.Property(p => p.IsDeleted).CurrentValue = true;
                entry.Property(p => p.DeleteAt).CurrentValue = DateTimeOffset.Now;
            }
        }

        return base.SaveChangesAsync(cancellationToken);
    }
}
