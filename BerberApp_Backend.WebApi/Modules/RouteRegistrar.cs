namespace BerberApp_Backend.WebApi.Modules;

public static class RouteRegistrar
{
    public static void RegisterRoutes(this IEndpointRouteBuilder app)
    {
        app.RegisterEmployeeRoutes();
        app.RegisterAuthRoutes();
    }
}
