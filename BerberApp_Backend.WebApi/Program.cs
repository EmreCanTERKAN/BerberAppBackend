using BerberApp_Backend.Application;
using BerberApp_Backend.Infrastructure;
using BerberApp_Backend.WebApi.Controllers;
using BerberApp_Backend.WebApi.Modules;
using Microsoft.AspNetCore.OData;
using Scalar.AspNetCore;
using System.Threading.RateLimiting;

var builder = WebApplication.CreateBuilder(args);

builder.AddServiceDefaults();
builder.Services.AddApplicationRegistrar();
builder.Services.AddInfrastructure(builder.Configuration);
builder.Services.AddCors();
builder.Services.AddOpenApi();

builder.Services.AddControllers().AddOData(opt =>
    opt
    .Select()
    .Filter()
    .Count()
    .Expand()
    .OrderBy()
    .SetMaxTop(null)
    .AddRouteComponents("odata", AppODataController.GetEdmModel()));

builder.Services.AddRateLimiter(options =>
{
    options.GlobalLimiter = PartitionedRateLimiter.Create<HttpContext, string>(context =>
    RateLimitPartition.GetFixedWindowLimiter("global", _ => new FixedWindowRateLimiterOptions
    {
        PermitLimit = 100,
        QueueProcessingOrder = QueueProcessingOrder.OldestFirst,
        QueueLimit = 100,
        Window = TimeSpan.FromSeconds(1)
    }));
});


var app = builder.Build();

app.UseCors(x => x
.AllowAnyHeader()
.AllowCredentials()
.AllowAnyMethod()
.SetIsOriginAllowed(t => true)
.SetPreflightMaxAge(TimeSpan.FromMinutes(10)));

app.RegisterRoutes();

app.UseRateLimiter();

app.MapOpenApi();
app.MapScalarApiReference();
app.MapDefaultEndpoints();
app.MapControllers();

app.Run();
