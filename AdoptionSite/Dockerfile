FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src

COPY . .

RUN ls -l /src
RUN ls -l /src/AdoptionSite
RUN ls -l /src/AdoptionSite/AnimalAdoption

RUN dotnet restore "AdoptionSite/AnimalAdoption/AnimalAdoption.sln"
