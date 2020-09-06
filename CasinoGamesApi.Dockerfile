FROM mcr.microsoft.com/dotnet/core/aspnet:3.1-buster-slim AS base
WORKDIR /app
EXPOSE 5000

FROM mcr.microsoft.com/dotnet/core/sdk:3.1-buster AS build
WORKDIR /src
COPY ["CasinoGames.Api/CasinoGames.Api.csproj", "CasinoGames.Api/"]
COPY ["CasinoGames.Api.Logic/CasinoGames.Api.Logic.csproj", "CasinoGames.Api.Logic/"]
COPY ["CasinoGames.Api.Data/CasinoGames.Api.Data.csproj", "CasinoGames.Api.Data/"]
COPY ["CasinoGames.Shared.Models/CasinoGames.Shared.Models.csproj", "CasinoGames.Shared.Models/"]
COPY ["RoundRobin/RoundRobin.csproj", "RoundRobin/"]
COPY ["RoundRobin.DependencyFactory/RoundRobin.DependencyFactory.csproj", "RoundRobin.DependencyFactory/"]
RUN dotnet restore "CasinoGames.Api/CasinoGames.Api.csproj"
COPY . .
WORKDIR /src/CasinoGames.Api
RUN dotnet build "CasinoGames.Api.csproj" -o /app/build

FROM build AS publish
RUN dotnet publish "CasinoGames.Api.csproj" -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENV ASPNETCORE_URLS http://+:5000

ENTRYPOINT ["dotnet", "CasinoGames.Api.dll"]