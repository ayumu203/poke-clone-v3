FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

COPY PokeClone.sln .
COPY src/Server.Domain/Server.Domain.csproj src/Server.Domain/
COPY src/Server.Application/Server.Application.csproj src/Server.Application/
COPY src/Server.Infrastructure/Server.Infrastructure.csproj src/Server.Infrastructure/
COPY src/Server.WebAPI/Server.WebAPI.csproj src/Server.WebAPI/
COPY tests/Server.Tests/Server.Tests.csproj tests/Server.Tests/

RUN dotnet restore

COPY . .

RUN dotnet publish src/Server.WebAPI/Server.WebAPI.csproj -c Release -o /app/publish /p:UseAppHost=false

FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS runtime
WORKDIR /app

ENV ASPNETCORE_URLS=http://+:5000
EXPOSE 5000

COPY --from=build /app/publish .

ENTRYPOINT ["dotnet", "Server.WebAPI.dll"]
