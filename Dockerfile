﻿FROM mcr.microsoft.com/dotnet/runtime:6.0 AS base
WORKDIR /app

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src
COPY ["NAS-App/NAS-App.csproj", "NAS-App/"]
RUN dotnet restore "NAS-App/NAS-App.csproj"
COPY . .
WORKDIR "/src/NAS-App"
RUN dotnet build "NAS-App.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "NAS-App.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "NAS-App.dll"]
