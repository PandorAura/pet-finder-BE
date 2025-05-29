# Use the official .NET 9 SDK image for build
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src

# Copy the solution file and project files from the AnimalAdoption folder
COPY AnimalAdoption/AnimalAdoption.sln ./AnimalAdoption/
COPY AnimalAdoption/AnimalAdoption.csproj ./AnimalAdoption/

# Restore dependencies using the solution file inside AnimalAdoption
RUN dotnet restore AnimalAdoption/AnimalAdoption.sln

# Copy all source code from AnimalAdoption folder
COPY AnimalAdoption/. ./AnimalAdoption/

WORKDIR /src/AnimalAdoption

# Publish the app to /app/publish folder
RUN dotnet publish -c Release -o /app/publish

# Use the official .NET 9 ASP.NET runtime image for runtime
FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS runtime
WORKDIR /app

# Copy published app from build stage
COPY --from=build /app/publish ./

# Expose port 5000 (adjust if needed)
EXPOSE 5000

# Set entrypoint to run the app
ENTRYPOINT ["dotnet", "AnimalAdoption.dll"]


