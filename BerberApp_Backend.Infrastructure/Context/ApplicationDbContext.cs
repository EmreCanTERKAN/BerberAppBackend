using BerberApp_Backend.Domain.Employees;
using Microsoft.EntityFrameworkCore;

namespace BerberApp_Backend.Infrastructure.Context;
internal sealed class ApplicationDbContext : DbContext
{
    public ApplicationDbContext()
    {
    }

    public ApplicationDbContext(DbContextOptions options) : base(options)
    {
    }

    public DbSet<Employee> Employees { get; set; }
}
