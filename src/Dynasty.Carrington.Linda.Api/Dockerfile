# syntax=docker/dockerfile:1
# Build context must be the repository root, e.g.:
#   docker build -f src/Dynasty.Carrington.Linda.Api/Dockerfile .

FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src

# Copy csproj files first so `dotnet restore` is cached until dependencies change.
COPY ["Directory.Build.props", "Directory.Packages.props", "./"]
COPY ["src/Dynasty.Carrington.Linda.Api/Dynasty.Carrington.Linda.Api.csproj", "src/Dynasty.Carrington.Linda.Api/"]
COPY ["src/Dynasty.Carrington.Linda.Infrastructure/Dynasty.Carrington.Linda.Infrastructure.csproj", "src/Dynasty.Carrington.Linda.Infrastructure/"]
COPY ["src/Dynasty.Carrington.Linda.Application/Dynasty.Carrington.Linda.Application.csproj", "src/Dynasty.Carrington.Linda.Application/"]
COPY ["src/Dynasty.Carrington.Linda.Domain/Dynasty.Carrington.Linda.Domain.csproj", "src/Dynasty.Carrington.Linda.Domain/"]
COPY ["src/Dynasty.Carrington.Core/Dynasty.Carrington.Core.csproj", "src/Dynasty.Carrington.Core/"]
COPY ["src/Dynasty.Carrington.ServiceDefaults/Dynasty.Carrington.ServiceDefaults.csproj", "src/Dynasty.Carrington.ServiceDefaults/"]
RUN dotnet restore "src/Dynasty.Carrington.Linda.Api/Dynasty.Carrington.Linda.Api.csproj"

# Copy the rest of the source and publish.
COPY src/Dynasty.Carrington.Linda.Api/ src/Dynasty.Carrington.Linda.Api/
COPY src/Dynasty.Carrington.Linda.Infrastructure/ src/Dynasty.Carrington.Linda.Infrastructure/
COPY src/Dynasty.Carrington.Linda.Application/ src/Dynasty.Carrington.Linda.Application/
COPY src/Dynasty.Carrington.Linda.Domain/ src/Dynasty.Carrington.Linda.Domain/
COPY src/Dynasty.Carrington.Core/ src/Dynasty.Carrington.Core/
COPY src/Dynasty.Carrington.ServiceDefaults/ src/Dynasty.Carrington.ServiceDefaults/

RUN dotnet publish "src/Dynasty.Carrington.Linda.Api/Dynasty.Carrington.Linda.Api.csproj" \
    -c $BUILD_CONFIGURATION \
    -o /app/publish \
    /p:UseAppHost=false

FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS final
WORKDIR /app
EXPOSE 8080
COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "Dynasty.Carrington.Linda.Api.dll"]
