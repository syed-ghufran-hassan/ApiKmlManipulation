﻿using ApiKMLManipulation.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAnyOrigin", policy =>
    {
        policy
            .AllowAnyOrigin()  
            .AllowAnyHeader()  
            .AllowAnyMethod();  
    });
});

builder.Services.AddScoped<KmlService>(provider =>
{
    var configuration = provider.GetRequiredService<IConfiguration>();
    var relativePath = configuration["ApplicationSettings:KmlFilePath"];

    var projectDirectory = Directory.GetParent(Directory.GetCurrentDirectory())?.Parent?.Parent?.FullName;
    if (projectDirectory == null)
    {
        throw new InvalidOperationException("Não foi possível determinar o diretório raiz do projeto.");
    }

    var filePath = Path.Combine(projectDirectory, relativePath);
    return new KmlService(filePath);
});

builder.Services.AddScoped<PlacemarkService>();

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseCors("AllowAnyOrigin");

app.MapControllers();

app.MapGet("/", () => "ApiKMLManipulation - API está funcionando!");

app.Run();
