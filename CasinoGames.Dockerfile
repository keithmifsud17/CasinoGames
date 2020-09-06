FROM mcr.microsoft.com/dotnet/core/sdk:3.1-buster AS base
WORKDIR /app
EXPOSE 5001

FROM mcr.microsoft.com/dotnet/core/sdk:3.1-buster AS build
WORKDIR /src
COPY ["CasinoGames/CasinoGames.Website.csproj", "CasinoGames/"]
COPY ["CasinoGames.Shared.Models/CasinoGames.Shared.Models.csproj", "CasinoGames.Shared.Models/"]
RUN dotnet restore "CasinoGames/CasinoGames.Website.csproj"
COPY . .
WORKDIR /src/CasinoGames
RUN dotnet build "CasinoGames.Website.csproj" -o /app/build

FROM build AS publish
RUN dotnet publish "CasinoGames.Website.csproj" -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENV ASPNETCORE_URLS http://+:5001

ENTRYPOINT ["dotnet", "CasinoGames.Website.dll"]