using BerberApp_Backend.Domain.Employees;
using BerberApp_Backend.Infrastructure.Context;
using GenericRepository;

namespace BerberApp_Backend.Infrastructure.Repositories;
internal sealed class EmployeeRepository : Repository<Employee, ApplicationDbContext>, IEmployeeRepository
{
    public EmployeeRepository(ApplicationDbContext context) : base(context)
    {
    }
}
