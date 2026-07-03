# build stage
FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

COPY ["src/MasarHub.API/MasarHub.API.csproj", "MasarHub.API/"]
COPY ["src/Core/MasarHub.Application/MasarHub.Application.csproj", "Core/MasarHub.Application/"]
COPY ["src/Core/MasarHub.Domain/MasarHub.Domain.csproj", "Core/MasarHub.Domain/"]
COPY ["src/Infrastructure/MasarHub.Infrastructure/MasarHub.Infrastructure.csproj", "Infrastructure/MasarHub.Infrastructure/"]
COPY ["src/Infrastructure/MasarHub.Infrastructure.Persistence/MasarHub.Infrastructure.Persistence.csproj", "Infrastructure/MasarHub.Infrastructure.Persistence/"]

RUN dotnet restore "MasarHub.API/MasarHub.API.csproj"
COPY src/ .
RUN dotnet publish "MasarHub.API/MasarHub.API.csproj" -c Release -o /Publish --no-restore

# runtime stage
FROM mcr.microsoft.com/dotnet/aspnet:10.0
WORKDIR /app

COPY --from=build /Publish .

EXPOSE 80

ENTRYPOINT ["dotnet", "MasarHub.API.dll"]
