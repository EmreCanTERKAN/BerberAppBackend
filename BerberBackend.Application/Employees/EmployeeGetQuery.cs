using BerberApp_Backend.Domain.Employees;
using MediatR;
using TS.Result;

namespace BerberApp_Backend.Application.Employees;
public sealed record EmployeeGetQuery(
    Guid Id) : IRequest<Result<Employee>>;

internal sealed class EmployeeGetQueryHandler(
    IEmployeeRepository employeeRepository) : IRequestHandler<EmployeeGetQuery, Result<Employee>>
{
    public async Task<Result<Employee>> Handle(EmployeeGetQuery request, CancellationToken cancellationToken)
    {
        Employee? employee = await employeeRepository.FirstOrDefaultAsync(e => e.Id == request.Id, cancellationToken);

        if (employee is null)
        {
            return Result<Employee>.Failure($"Employee with ID {request.Id} not found.");
        }

        return employee;
    }
}
