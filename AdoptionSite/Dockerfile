# Stage 1: Build (Force .NET 9)
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src

# Copy solution and project files (relative to repo root)
COPY AdoptionSite/AnimalAdoption/AnimalAdoption.csproj .
RUN dotnet restore "AnimalAdoption.csproj"

# Copy everything else from project folder
COPY AdoptionSite/AnimalAdoption/ .

# Build and publish
RUN dotnet publish "AnimalAdoption.csproj" -c Release -o /app

# Stage 2: Runtime (Force .NET 9)
FROM mcr.microsoft.com/dotnet/aspnet:9.0
WORKDIR /app
COPY --from=build /app .

# Configure environment
ENV ASPNETCORE_ENVIRONMENT=Production
ENV ASPNETCORE_URLS=http://+:3000
ENV PORT=3000

# Create uploads directory
RUN mkdir -p /app/AnimalUploads

ENTRYPOINT ["dotnet", "AnimalAdoption.dll"]