FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS base
USER $APP_UID
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["src/Blake/Web.Client/Web.Client.csproj", "src/Blake/Web.Client/"]
COPY ["src/Blake/Web.Shared/Web.Shared.csproj", "src/Blake/Web.Shared/"]
RUN dotnet restore "src/Blake/Web.Client/Web.Client.csproj"
COPY . .
WORKDIR "/src/src/Blake/Web.Client"
RUN dotnet build "./Web.Client.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./Web.Client.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Dynasty.Carrington.Blake.Web.Client.dll"]
