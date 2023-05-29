FROM mcr.microsoft.com/dotnet/core/runtime:3.1 AS base
WORKDIR /app

FROM mcr.microsoft.com/dotnet/core/sdk:3.1 AS build
WORKDIR /src
COPY ["MultiplayerDedicatedServer.csproj", "./"]
RUN dotnet restore "MultiplayerDedicatedServer.csproj"
COPY . .
WORKDIR "/src/"
RUN dotnet build "MultiplayerDedicatedServer.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "MultiplayerDedicatedServer.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "MultiplayerDedicatedServer.dll"]
