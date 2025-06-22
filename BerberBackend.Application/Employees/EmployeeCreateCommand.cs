using BerberApp_Backend.Domain.Employees;
using FluentValidation;
using GenericRepository;
using Mapster;
using MediatR;
using TS.Result;

namespace BerberApp_Backend.Application.Employees;
public sealed record EmployeeCreateCommand(
    string FirstName,
    string LastName,
    DateOnly BirthOfDate,
    decimal Salary,
    PersonelInformation PersonelInformation,
    Address? Address) : IRequest<Result<string>>;

public sealed class EmployeeCreateCommandValidator : AbstractValidator<EmployeeCreateCommand>
{
    public EmployeeCreateCommandValidator()
    {
        RuleFor(x => x.FirstName)
            .MinimumLength(3).WithMessage("Ad en az 3 karakter olmalıdır.");
        RuleFor(x => x.LastName)
            .MinimumLength(3).WithMessage("Soyad en az 3 karakter olmalıdır.");
        RuleFor(x => x.PersonelInformation.TCNo)
            .MinimumLength(11).WithMessage("Tc Kimlik Numarası en az 11 karakter olmalıdır.")
            .MaximumLength(11).WithMessage("Tc Kimlik Numarası en fazla 11 karakter olmalıdır.");
    }
}

internal sealed class EmployeeCreateCommandHandler(
    IEmployeeRepository employeeRepository,
    IUnitOfWork unitOfWork) : IRequestHandler<EmployeeCreateCommand, Result<string>>
{
    public async Task<Result<string>> Handle(EmployeeCreateCommand request, CancellationToken cancellationToken)
    {
        var isEmployeeExists = await employeeRepository
            .AnyAsync(x => x.PersonelInformation.TCNo == request.PersonelInformation.TCNo, cancellationToken);

        if (isEmployeeExists)
        {
            return Result<string>.Failure("Bu Tc daha önce kayıt edilmiş");
        }

        Employee employee = request.Adapt<Employee>();

        employeeRepository.Add(employee);

        await unitOfWork.SaveChangesAsync(cancellationToken);

        return "Personel kaydı başarıyla tamamlandı";
    }
}