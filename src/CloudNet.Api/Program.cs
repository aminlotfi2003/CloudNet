using CloudNet.Api.Abstractions.Extensions;
using CloudNet.Api.Abstractions.Extensions.DependencyInjection;

var builder = WebApplication.CreateBuilder(args);

builder.AddSerilogLogging();

builder.Services.AddServices(builder.Configuration);

var app = builder.Build();

app.UseApplicationPipeline();

app.Run();
