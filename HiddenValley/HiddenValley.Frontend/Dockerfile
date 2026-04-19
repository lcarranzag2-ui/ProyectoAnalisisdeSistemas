FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src

COPY ["HiddenValley.Frontend.csproj", "./"]
RUN dotnet restore "HiddenValley.Frontend.csproj"

COPY . .
RUN dotnet build "HiddenValley.Frontend.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "HiddenValley.Frontend.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM nginx:alpine AS final
WORKDIR /usr/share/nginx/html

COPY --from=publish /app/publish/wwwroot .

EXPOSE 80

ENTRYPOINT ["nginx", "-g", "daemon off;"]