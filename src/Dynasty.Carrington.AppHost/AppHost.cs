using Projects;

IDistributedApplicationBuilder builder = DistributedApplication.CreateBuilder(args);

builder.AddProject<Api>("krystle-api");

builder.AddProject<Web>("blake-web");

builder.AddJavaScriptApp("alexis", "../Dynasty.Carrington.Alexis")
    .WithHttpEndpoint(env: "PORT")
    .WithExternalHttpEndpoints();

builder.Build().Run();