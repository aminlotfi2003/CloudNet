using CloudNet.Api.Extensions.DependencyInjection;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddServices(builder.Configuration);

var app = builder.Build();

app.UseApplicationPipeline();

app.Run();
