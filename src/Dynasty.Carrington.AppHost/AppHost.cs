using Projects;

IDistributedApplicationBuilder builder = DistributedApplication.CreateBuilder(args);

builder.AddProject<Dynasty_Carrington_Linda_Api>("linda-api");

builder.AddProject<Dynasty_Carrington_Blake_Web>("blake-web");

builder.AddJavaScriptApp("alexis", "../Dynasty.Carrington.Alexis")
    .WithHttpEndpoint(env: "PORT")
    .WithExternalHttpEndpoints();

builder.Build().Run();