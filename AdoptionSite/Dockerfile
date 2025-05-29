# Stage 1: Build
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src

# Copy everything
COPY . .

# Restore dependencies using your solution file path
RUN dotnet restore "AdoptionSite/AnimalAdoption/AnimalAdoption.sln"

# Publish the app in Release mode to /app/publish
WORKDIR "/src/AdoptionSite/AnimalAdoption"
RUN dotnet publish -c Release -o /app/publish --no-restore

# Stage 2: Runtime
FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS runtime
WORKDIR /app

# Copy published output from build stage
COPY --from=build /app/publish ./

# Expose the port your app will listen on (adjust if needed)
EXPOSE 5000

# Set entrypoint to run your app
ENTRYPOINT ["dotnet", "AnimalAdoption.dll"]
