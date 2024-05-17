#See https://aka.ms/customizecontainer to learn how to customize your debug container and how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 8080
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

COPY ["Tor.Api/Tor.Api.csproj", "Tor.Api/"]
COPY ["Tor.Application/Tor.Application.csproj", "Tor.Application/"]
COPY ["Tor.Domain/Tor.Domain.csproj", "Tor.Domain/"]
COPY ["Tor.Contracts/Tor.Contracts.csproj", "Tor.Contracts/"]
COPY ["Tor.Infrastructure/Tor.Infrastructure.csproj", "Tor.Infrastructure/"]

COPY ./NuGet.Config /

RUN dotnet restore --configfile /NuGet.Config "Tor.Api/Tor.Api.csproj"
COPY . .
WORKDIR "/src/Tor.Api"
RUN dotnet build "Tor.Api.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "Tor.Api.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Tor.Api.dll"]