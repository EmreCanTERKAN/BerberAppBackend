using BerberApp_Backend.Domain.Employees;
using GenericRepository;
using Microsoft.EntityFrameworkCore;

namespace BerberApp_Backend.Infrastructure.Context;
internal sealed class ApplicationDbContext : DbContext,IUnitOfWork
{
    public ApplicationDbContext()
    {
    }

    public ApplicationDbContext(DbContextOptions options) : base(options)
    {
    }

    public DbSet<Employee> Employees { get; set; }
}
