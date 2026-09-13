# syntax=docker/dockerfile:1
# Build context must be the repository root, e.g.:
#   docker build -f src/Linda/Api/Dockerfile .

FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src

# Copy csproj files first so `dotnet restore` is cached until dependencies change.
COPY ["Directory.Build.props", "Directory.Packages.props", "./"]
COPY ["src/Linda/Api/Api.csproj", "src/Linda/Api/"]
COPY ["src/Linda/Infrastructure/Infrastructure.csproj", "src/Linda/Infrastructure/"]
COPY ["src/Linda/Application/Application.csproj", "src/Linda/Application/"]
COPY ["src/Linda/Domain/Domain.csproj", "src/Linda/Domain/"]
COPY ["src/Dynasty.Carrington.Core/Dynasty.Carrington.Core.csproj", "src/Dynasty.Carrington.Core/"]
COPY ["src/Dynasty.Carrington.ServiceDefaults/Dynasty.Carrington.ServiceDefaults.csproj", "src/Dynasty.Carrington.ServiceDefaults/"]
RUN dotnet restore "src/Linda/Api/Api.csproj"

# Copy the rest of the source and publish.
COPY src/Linda/Api/ src/Linda/Api/
COPY src/Linda/Infrastructure/ src/Linda/Infrastructure/
COPY src/Linda/Application/ src/Linda/Application/
COPY src/Linda/Domain/ src/Linda/Domain/
COPY src/Dynasty.Carrington.Core/ src/Dynasty.Carrington.Core/
COPY src/Dynasty.Carrington.ServiceDefaults/ src/Dynasty.Carrington.ServiceDefaults/

RUN dotnet publish "src/Linda/Api/Api.csproj" \
    -c $BUILD_CONFIGURATION \
    -o /app/publish \
    /p:UseAppHost=false

FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS final
WORKDIR /app
EXPOSE 8080
COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "Dynasty.Carrington.Linda.Api.dll"]
