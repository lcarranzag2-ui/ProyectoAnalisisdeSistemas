FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS base
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src

# Copiar los csproj primero (mejora cache de Docker)
COPY ["HiddenValley.API/HiddenValley.API.csproj", "HiddenValley.API/"]
COPY ["HiddenValley.Shared/HiddenValley.Shared.csproj", "HiddenValley.Shared/"]
RUN dotnet restore "HiddenValley.API/HiddenValley.API.csproj"

# Copiar el resto del código
COPY HiddenValley.API/ HiddenValley.API/
COPY HiddenValley.Shared/ HiddenValley.Shared/

WORKDIR /src/HiddenValley.API
RUN dotnet build "HiddenValley.API.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "HiddenValley.API.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "HiddenValley.API.dll"]
