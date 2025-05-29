# Use the official .NET 9 SDK image for build
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src

# Copy the entire project directory structure
COPY . .

# Restore dependencies
RUN dotnet restore "Adoption site/AnimalAdoption/AnimalAdoption.sln"

# Publish the app
WORKDIR "/src/Adoption site/AnimalAdoption"
RUN dotnet publish -c Release -o /app/publish

# Use the official .NET 9 ASP.NET runtime image for runtime
FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS runtime
WORKDIR /app

# Copy published app from build stage
COPY --from=build /app/publish ./

# Expose port 5000
EXPOSE 5000

# Set entrypoint to run the app
ENTRYPOINT ["dotnet", "AnimalAdoption.dll"]
