FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src

# Copiar los csproj primero para aprovechar cache de Docker
COPY ["HiddenValley.Frontend/HiddenValley.Frontend.csproj", "HiddenValley.Frontend/"]
COPY ["HiddenValley.Shared/HiddenValley.Shared.csproj", "HiddenValley.Shared/"]
RUN dotnet restore "HiddenValley.Frontend/HiddenValley.Frontend.csproj"

# Copiar el resto del codigo
COPY HiddenValley.Frontend/ HiddenValley.Frontend/
COPY HiddenValley.Shared/ HiddenValley.Shared/

WORKDIR /src/HiddenValley.Frontend
RUN dotnet build "HiddenValley.Frontend.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "HiddenValley.Frontend.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM nginx:alpine AS final
WORKDIR /usr/share/nginx/html

COPY --from=publish /app/publish/wwwroot .

EXPOSE 80

ENTRYPOINT ["nginx", "-g", "daemon off;"]
