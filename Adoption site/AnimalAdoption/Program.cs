using AnimalAdoption.Models;
using AnimalAdoption;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.Extensions.FileProviders;


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

// Add services to the container
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Add custom services
builder.Services.AddSingleton<IAnimalGenerator, AnimalGenerator>();
builder.Services.AddSingleton<IAnimalFileService, AnimalFileService>();
builder.Services.AddSingleton<AnimalContext>();

// In Program.cs or Startup.cs
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


// Seed initial data
using (var scope = app.Services.CreateScope())
{
    var context = scope.ServiceProvider.GetRequiredService<AnimalContext>();

    context.Add(new Animal
    {
        Name = "Buddy",
        Age = 3,
        Photo = "https://example.com/dog1.jpg",
        Description = "Friendly dog",
        Location = "New York",
        IsAdopted = false
    });

    context.Add(new Animal
    {
        Name = "Whiskers",
        Age = 5,
        Photo = "https://example.com/cat1.jpg",
        Description = "Calm cat",
        Location = "Los Angeles",
        IsAdopted = false
    });

}

// Configure the HTTP request pipeline
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseCors("AllowReactApp");

// WebSocket configuration
app.UseWebSockets();
app.UseMiddleware<WebSocketMiddleware>();

app.UseAuthorization();
app.MapControllers();

// Start the animal generator in the background
var generator = app.Services.GetRequiredService<IAnimalGenerator>();
var lifetime = app.Services.GetRequiredService<IHostApplicationLifetime>();
Task.Run(() => generator.StartGeneratingAsync(lifetime.ApplicationStopping));

// Add this before app.Run()
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