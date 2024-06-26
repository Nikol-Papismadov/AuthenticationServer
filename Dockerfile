FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /source

COPY *.sln .
COPY *.Api/*.csproj ./AuthenticationServer.Api/
COPY *.Data/*.csproj ./AuthenticationServer.Data/
COPY *.Models/*.csproj ./AuthenticationServer.Models/
COPY *.Services/*.csproj ./AuthenticationServer.Services/
COPY *.UnitTests/*.csproj ./AuthenticationServer.UnitTests/
RUN dotnet restore

COPY . .

RUN dotnet publish -c release --property:PublishDir=/app --no-restore
FROM mcr.microsoft.com/dotnet/aspnet:8.0
WORKDIR /app
COPY --from=build /app ./

EXPOSE 5000

ENTRYPOINT ["dotnet", "AuthenticationServer.Api.dll"]