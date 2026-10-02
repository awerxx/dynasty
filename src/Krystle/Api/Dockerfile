# syntax=docker/dockerfile:1
# Build context must be the repository root, e.g.:
#   docker build -f src/Krystle/Api/Dockerfile .

FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src

# Copy csproj files first so `dotnet restore` is cached until dependencies change.
COPY ["Directory.Build.props", "Directory.Packages.props", "./"]
COPY ["src/Krystle/Api/Api.csproj", "src/Krystle/Api/"]
COPY ["src/Krystle/Infrastructure/Infrastructure.csproj", "src/Krystle/Infrastructure/"]
COPY ["src/Krystle/Application/Application.csproj", "src/Krystle/Application/"]
COPY ["src/Krystle/Domain/Domain.csproj", "src/Krystle/Domain/"]
COPY ["src/Dynasty.Carrington.Core/Dynasty.Carrington.Core.csproj", "src/Dynasty.Carrington.Core/"]
COPY ["src/Dynasty.Carrington.ServiceDefaults/Dynasty.Carrington.ServiceDefaults.csproj", "src/Dynasty.Carrington.ServiceDefaults/"]
RUN dotnet restore "src/Krystle/Api/Api.csproj"

# Copy the rest of the source and publish.
COPY src/Krystle/Api/ src/Krystle/Api/
COPY src/Krystle/Infrastructure/ src/Krystle/Infrastructure/
COPY src/Krystle/Application/ src/Krystle/Application/
COPY src/Krystle/Domain/ src/Krystle/Domain/
COPY src/Dynasty.Carrington.Core/ src/Dynasty.Carrington.Core/
COPY src/Dynasty.Carrington.ServiceDefaults/ src/Dynasty.Carrington.ServiceDefaults/

RUN dotnet publish "src/Krystle/Api/Api.csproj" \
    -c $BUILD_CONFIGURATION \
    -o /app/publish \
    /p:UseAppHost=false

FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS final
WORKDIR /app
EXPOSE 8080
COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "Dynasty.Carrington.Krystle.Api.dll"]
