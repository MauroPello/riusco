# Cambiare da Default a Docker il nome della ConnectionString da prendere per il database
# sudo docker-compose build
# sudo docker-compose up

# sudo docker-compose down
# per controllare se ci sia già un server mssql
# sudo lsof -i -P -n | grep 1433
# cancellare tutti i container
# sudo docker ps -aq

# far partire mssql localmente : systemctl enable and start mssql-server
# stoppare mssql localmente : systemctl stop and disable mssql-server

FROM mcr.microsoft.com/dotnet/aspnet:7.0 AS base
WORKDIR /app
EXPOSE 80

FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build
WORKDIR /src
COPY ["riusco-mvc.csproj", "./"]
RUN dotnet restore "./riusco-mvc.csproj"
COPY . .
RUN dotnet build "riusco-mvc.csproj" -c Release -o /app

FROM build AS publish
RUN dotnet publish "riusco-mvc.csproj" -c Release -o /app

FROM base AS final
WORKDIR /app
COPY --from=publish /app .
ENTRYPOINT ["dotnet", "riusco-mvc.dll"]