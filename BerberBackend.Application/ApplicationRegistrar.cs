using BerberApp_Backend.Application.Behaviors;
using Microsoft.Extensions.DependencyInjection;
using FluentValidation;

namespace BerberApp_Backend.Application;
public static class ApplicationRegistrar
{
    public static IServiceCollection AddApplicationRegistrar(this IServiceCollection services)
    {
        services.AddMediatR(conf =>
        {
            conf.RegisterServicesFromAssembly(typeof(ApplicationRegistrar).Assembly);
            conf.AddOpenBehavior(typeof(ValidationBehavior<,>));
        });

        services.AddValidatorsFromAssembly(typeof(ApplicationRegistrar).Assembly);

        return services;
    }
}
