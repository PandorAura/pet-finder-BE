# Stage 1: Build (explicitly uses .NET 9 SDK)
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src

# Copy ONLY the project file first (for layer caching)
COPY ./AnimalAdoption/AnimalAdoption.csproj ./AnimalAdoption/
RUN dotnet restore "./AnimalAdoption/AnimalAdoption.csproj"

# Copy everything else
COPY . .

# Build and publish
WORKDIR "/src/AnimalAdoption"
RUN dotnet publish -c Release -o /app

# Stage 2: Runtime (uses .NET 9 runtime)
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