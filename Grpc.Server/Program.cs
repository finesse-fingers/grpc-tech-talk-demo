﻿using Grpc.Server.Services;
using Microsoft.OpenApi.Models;

var builder = WebApplication.CreateBuilder(args);

builder.WebHost.ConfigureKestrel(options =>
{
    options.ListenAnyIP(5186, listenOptions =>
    {
        listenOptions.Protocols = Microsoft.AspNetCore.Server.Kestrel.Core.HttpProtocols.Http2;
    });
    options.ListenAnyIP(5187, listenOptions =>
    {
        listenOptions.Protocols = Microsoft.AspNetCore.Server.Kestrel.Core.HttpProtocols.Http1;
    });
});

// Additional configuration is required to successfully run gRPC on macOS.
// For instructions on how to configure Kestrel and gRPC clients on macOS, visit https://go.microsoft.com/fwlink/?linkid=2099682

// Add services to the container.
builder.Services.AddGrpc().AddJsonTranscoding();
builder.Services.AddGrpcReflection();
//Register Swagger generator, defining one or more documents to be generated by Swagger generator
builder.Services.AddGrpcSwagger();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1",
        new OpenApiInfo { Title = "gRPC transcoding", Version = "v1" });
});

var app = builder.Build();

// Configure the HTTP request pipeline.
app.MapGrpcService<GreeterService>();

// Enable grpc server reflection
app.MapGrpcReflectionService();

app.MapGet("/", () => "Communication with gRPC endpoints must be made through a gRPC client. To learn how to create a client, visit: https://go.microsoft.com/fwlink/?linkid=2086909");

// Enable middleware to serve generated Swagger as a JSON endpoint.
app.UseSwagger();
//Enable middleware to serve swagger-ui (HTML, JS, CSS, etc.)
app.UseSwaggerUI(c =>
{
    c.SwaggerEndpoint("/swagger/v1/swagger.json", "My API V1");
});


app.Run();

