# syntax=docker/dockerfile:1
# Build context must be the repository root, e.g.:
#   docker build -f src/Dynasty.Carrington.Blake.Web/Dockerfile .

FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src

# Copy csproj files first so `dotnet restore` is cached until dependencies change.
COPY ["Directory.Build.props", "Directory.Packages.props", "./"]
COPY ["src/Dynasty.Carrington.Blake.Web/Dynasty.Carrington.Blake.Web.csproj", "src/Dynasty.Carrington.Blake.Web/"]
COPY ["src/Dynasty.Carrington.Blake.Infrastructure.Database/Dynasty.Carrington.Blake.Infrastructure.Database.csproj", "src/Dynasty.Carrington.Blake.Infrastructure.Database/"]
COPY ["src/Dynasty.Carrington.Blake.Application/Dynasty.Carrington.Blake.Application.csproj", "src/Dynasty.Carrington.Blake.Application/"]
COPY ["src/Dynasty.Carrington.Blake.Domain/Dynasty.Carrington.Blake.Domain.csproj", "src/Dynasty.Carrington.Blake.Domain/"]
COPY ["src/Dynasty.Carrington.Core/Dynasty.Carrington.Core.csproj", "src/Dynasty.Carrington.Core/"]
COPY ["src/Dynasty.Carrington.ServiceDefaults/Dynasty.Carrington.ServiceDefaults.csproj", "src/Dynasty.Carrington.ServiceDefaults/"]
RUN dotnet restore "src/Dynasty.Carrington.Blake.Web/Dynasty.Carrington.Blake.Web.csproj"

# Copy the rest of the source and publish.
COPY src/Dynasty.Carrington.Blake.Web/ src/Dynasty.Carrington.Blake.Web/
COPY src/Dynasty.Carrington.Blake.Infrastructure.Database/ src/Dynasty.Carrington.Blake.Infrastructure.Database/
COPY src/Dynasty.Carrington.Blake.Application/ src/Dynasty.Carrington.Blake.Application/
COPY src/Dynasty.Carrington.Blake.Domain/ src/Dynasty.Carrington.Blake.Domain/
COPY src/Dynasty.Carrington.Core/ src/Dynasty.Carrington.Core/
COPY src/Dynasty.Carrington.ServiceDefaults/ src/Dynasty.Carrington.ServiceDefaults/

RUN dotnet publish "src/Dynasty.Carrington.Blake.Web/Dynasty.Carrington.Blake.Web.csproj" \
    -c $BUILD_CONFIGURATION \
    -o /app/publish \
    /p:UseAppHost=false

FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS final
WORKDIR /app
EXPOSE 8080
COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "Dynasty.Carrington.Blake.Web.dll"]
