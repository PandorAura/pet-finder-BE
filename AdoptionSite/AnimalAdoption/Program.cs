using AnimalAdoption;
using AnimalAdoption.Data;
using AnimalAdoption.Models;
using AnimalAdoption.Repositories;
using AnimalAdoption.Services;
using AnimalAdoption.Utilities.MappingProfiles;
using DotEnv.Core;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.FileProviders;

var builder = WebApplication.CreateBuilder(args);

// Load environment variables (Railway will use ConnectionStrings__DefaultConnection)
builder.Host.ConfigureAppConfiguration((context, config) =>
{
    config.AddEnvironmentVariables();
});

// Add AutoMapper profile
builder.Services.AddAutoMapper(typeof(AnimalAdoptionProfile));

// Handle large file uploads (2GB)
builder.WebHost.ConfigureKestrel(serverOptions =>
{
    serverOptions.Limits.MaxRequestBodySize = 2_000_000_000; // 2GB
});

builder.Services.Configure<FormOptions>(options =>
{
    options.MultipartBodyLengthLimit = 2_000_000_000; // 2GB
});

new EnvLoader().Load();

var connectionString = Environment.GetEnvironmentVariable("ConnectionStrings__DefaultConnection");
if (string.IsNullOrWhiteSpace(connectionString))
{
    throw new InvalidOperationException("Missing database connection string.");
}

builder.Services.AddDbContext<AnimalAdoptionContext>(options =>
    options.UseNpgsql(connectionString));


// Register DbContext
builder.Services.AddDbContext<AnimalAdoptionContext>(options =>
    options.UseNpgsql(connectionString));

// Register services & repositories
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddTransient<DataSeederService>();

builder.Services.AddSingleton<IAnimalGenerator, AnimalGenerator>();
builder.Services.AddSingleton<IAnimalFileService, AnimalFileService>();

builder.Services.AddScoped<AnimalContext>();
builder.Services.AddScoped<IAnimalService, AnimalService>();
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<IAdoptionService, AdoptionService>();

builder.Services.AddScoped<IAnimalRepository, AnimalRepository>();
builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<IAdoptionRepository, AdoptionRepository>();

// CORS policies
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowFrontend", policy =>
    {
        policy.WithOrigins("https://pet-finder-fe-production.up.railway.app")
              .AllowAnyMethod()
              .AllowAnyHeader()
              .AllowCredentials(); // If cookies/auth are used
    });
});

// Build app
var app = builder.Build();

// Seed data if --seed is passed
if (args.Contains("--seed"))
{
    using var scope = app.Services.CreateScope();
    var seeder = scope.ServiceProvider.GetRequiredService<DataSeederService>();
    await seeder.SeedDataAsync();
    return;
}

// Middleware
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseCors("AllowFrontend");

app.UseWebSockets();
app.UseMiddleware<WebSocketMiddleware>();

app.UseAuthorization();
app.MapControllers();

// Static file hosting for uploaded animal images
app.UseStaticFiles(new StaticFileOptions
{
    FileProvider = new PhysicalFileProvider(
        Path.Combine(builder.Environment.ContentRootPath, "AnimalUploads")),
    RequestPath = "/animal-uploads",
    OnPrepareResponse = ctx =>
    {
        ctx.Context.Response.Headers.Append("Cache-Control", "public,max-age=86400");
    }
});

// Start background animal generator
var generator = app.Services.GetRequiredService<IAnimalGenerator>();
var lifetime = app.Services.GetRequiredService<IHostApplicationLifetime>();
Task.Run(() => generator.StartGeneratingAsync(lifetime.ApplicationStopping));


if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
}


// Run app
app.Run();
