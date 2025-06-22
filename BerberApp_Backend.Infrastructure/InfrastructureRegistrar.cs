using BerberApp_Backend.Infrastructure.Context;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace BerberApp_Backend.Infrastructure;
public static class InfrastructureRegistrar
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddDbContext<ApplicationDbContext>(opt =>
        {
            string connectionString = configuration.GetConnectionString("SqlServer")!;
            opt.UseSqlServer(connectionString);
        });

        services.Scan(opt => opt
        .FromAssemblies(typeof(InfrastructureRegistrar).Assembly)
        .AddClasses(publicOnly: false)
        .UsingRegistrationStrategy(Scrutor.RegistrationStrategy.Skip)
        .AsImplementedInterfaces()
        .WithScopedLifetime());

        return services;
    }
}
