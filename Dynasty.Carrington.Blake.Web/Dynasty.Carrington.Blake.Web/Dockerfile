FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS base
USER $APP_UID
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["Dynasty.Carrington.Blake.Web/Dynasty.Carrington.Blake.Web/Dynasty.Carrington.Blake.Web.csproj", "Dynasty.Carrington.Blake.Web/Dynasty.Carrington.Blake.Web/"]
COPY ["Dynasty.Carrington.Blake.Web/Dynasty.Carrington.Blake.Web.Client/Dynasty.Carrington.Blake.Web.Client.csproj", "Dynasty.Carrington.Blake.Web/Dynasty.Carrington.Blake.Web.Client/"]
RUN dotnet restore "Dynasty.Carrington.Blake.Web/Dynasty.Carrington.Blake.Web/Dynasty.Carrington.Blake.Web.csproj"
COPY . .
WORKDIR "/src/Dynasty.Carrington.Blake.Web/Dynasty.Carrington.Blake.Web"
RUN dotnet build "./Dynasty.Carrington.Blake.Web.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./Dynasty.Carrington.Blake.Web.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Dynasty.Carrington.Blake.Web.dll"]
