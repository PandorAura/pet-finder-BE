# Use .NET 9.0 Preview base image for runtime
FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS base
WORKDIR /app
EXPOSE 5000

# Use .NET 9.0 Preview SDK image to build
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src

# Copy project file and restore
COPY AnimalAdoption/AnimalAdoption.csproj ./AnimalAdoption/
RUN dotnet restore ./AnimalAdoption/AnimalAdoption.csproj

# Copy the rest of the code
COPY AnimalAdoption/. ./AnimalAdoption/
WORKDIR /src/AnimalAdoption

# Publish the app
RUN dotnet publish -c Release -o /app/publish

# Final runtime image
FROM base AS final
WORKDIR /app
COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "AnimalAdoption.dll"]
