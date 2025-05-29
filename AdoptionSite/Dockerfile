# Stage 1: Build
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src

# Copy solution and project files first (better layer caching)
COPY ./AnimalAdoption/*.csproj ./AnimalAdoption/
RUN dotnet restore "AnimalAdoption/AnimalAdoption.csproj"

# Copy everything else
COPY . .

# Build and publish
WORKDIR /src/AnimalAdoption
RUN dotnet publish -c Release -o /app

# Stage 2: Runtime
FROM mcr.microsoft.com/dotnet/aspnet:9.0
WORKDIR /app
COPY --from=build /app .

# Environment variables
ENV ASPNETCORE_ENVIRONMENT=Production
ENV ASPNETCORE_URLS=http://+:3000
ENV PORT=3000

# Create directory for uploads
RUN mkdir -p /app/AnimalUploads

ENTRYPOINT ["dotnet", "AnimalAdoption.dll"]