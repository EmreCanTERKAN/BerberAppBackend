var builder = DistributedApplication.CreateBuilder(args);

builder.AddProject<Projects.BerberApp_Backend_WebApi>("berberapp-backend-webapi");

builder.Build().Run();
