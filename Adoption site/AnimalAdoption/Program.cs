using AnimalAdoption.Models;
using AnimalAdoption;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.Extensions.FileProviders;
using AnimalAdoption.Data;
using Microsoft.EntityFrameworkCore;
using AnimalAdoption.Services; // Add this namespace
using AnimalAdoption.Repositories; // Add this namespace if your repositories are there

var builder = WebApplication.CreateBuilder(args);

// Add before building the app
builder.WebHost.ConfigureKestrel(serverOptions =>
{
    serverOptions.Limits.MaxRequestBodySize = 2_000_000_000; // 2GB
});

builder.Services.Configure<FormOptions>(options =>
{
    options.MultipartBodyLengthLimit = 2_000_000_000; // 2GB
});

var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
builder.Services.AddDbContext<AnimalAdoptionContext>(options =>
    options.UseSqlServer(connectionString));

// Add services to the container
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Add custom services
builder.Services.AddSingleton<IAnimalGenerator, AnimalGenerator>();
builder.Services.AddSingleton<IAnimalFileService, AnimalFileService>();
builder.Services.AddSingleton<AnimalContext>();

// Add these missing registrations
builder.Services.AddScoped<IAnimalService, AnimalService>(); // Add this line
builder.Services.AddScoped<IUserService, UserService>(); // If you have this
builder.Services.AddScoped<IAnimalRepository, AnimalRepository>(); // Add this line
builder.Services.AddScoped<IUserRepository, UserRepository>(); // If you have this

// CORS configuration
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", builder =>
    {
        builder.AllowAnyOrigin()
               .AllowAnyMethod()
               .AllowAnyHeader();
    });
});

var app = builder.Build();
app.UseCors("AllowAll");

// Configure the HTTP request pipeline
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

// WebSocket configuration
app.UseWebSockets();
app.UseMiddleware<WebSocketMiddleware>();

app.UseAuthorization();
app.MapControllers();

// Start the animal generator in the background
var generator = app.Services.GetRequiredService<IAnimalGenerator>();
var lifetime = app.Services.GetRequiredService<IHostApplicationLifetime>();
Task.Run(() => generator.StartGeneratingAsync(lifetime.ApplicationStopping));

app.UseStaticFiles(new StaticFileOptions
{
    FileProvider = new PhysicalFileProvider(
        Path.Combine(builder.Environment.ContentRootPath, "AnimalUploads")),
    RequestPath = "/animal-uploads",
    OnPrepareResponse = ctx =>
    {
        // Cache static files for 1 day
        ctx.Context.Response.Headers.Append(
            "Cache-Control", "public,max-age=86400");
    }
});

app.Run();