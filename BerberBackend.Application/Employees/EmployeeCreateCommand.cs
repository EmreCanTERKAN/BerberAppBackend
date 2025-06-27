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
    DateOnly? BirthOfDate,
    decimal Salary,
    PersonelInformation PersonelInformation,
    Address? Address,
    bool IsActive) : IRequest<Result<string>>;

public sealed class EmployeeCreateCommandValidator : AbstractValidator<EmployeeCreateCommand>
{
    public EmployeeCreateCommandValidator()
    {
        RuleFor(x => x.FirstName)
            .NotEmpty().WithMessage("Ad boş olamaz.")
            .MinimumLength(3).WithMessage("Ad en az 3 karakter olmalıdır.");

        RuleFor(x => x.LastName)
            .NotEmpty().WithMessage("Soyad boş olamaz.")
            .MinimumLength(3).WithMessage("Soyad en az 3 karakter olmalıdır.");

        RuleFor(x => x.BirthOfDate)
            .NotNull().WithMessage("Doğum tarihi boş olamaz.")
            .Must(BeAValidAge).WithMessage("Geçersiz doğum tarihi.");

        RuleFor(x => x.Salary)
            .GreaterThan(0).WithMessage("Maaş 0'dan büyük olmalıdır.");

        // PersonelInformation null kontrolü
        RuleFor(x => x.PersonelInformation)
            .NotNull().WithMessage("Personel bilgileri boş olamaz.");

        // PersonelInformation'ın içindeki TCNo kontrolü
        When(x => x.PersonelInformation != null, () =>
        {
            RuleFor(x => x.PersonelInformation.TCNo)
                .NotEmpty().WithMessage("TC Kimlik Numarası boş olamaz.")
                .Length(11).WithMessage("TC Kimlik Numarası 11 karakter olmalıdır.")
                .Matches(@"^\d{11}$").WithMessage("TC Kimlik Numarası sadece rakam içermelidir.");
        });
    }

    private bool BeAValidAge(DateOnly? birthDate)
    {
        if (!birthDate.HasValue) return false;

        var age = DateTime.Today.Year - birthDate.Value.Year;
        return age >= 18 && age <= 65;
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