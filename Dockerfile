FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS base
USER $APP_UID
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src

RUN --mount=type=secret,id=cert,target=/tmp/cert.pem \
    && cp /tmp/cert.pem /usr/local/share/ca-certificates/local.devspot.tech.pem \
    && update-ca-certificates

COPY ["SaxsSpot.NanoSystemService.Host/SaxsSpot.NanoSystemService.Host.csproj", "SaxsSpot.NanoSystemService.Host/"]
COPY ["SaxsSpot.NanoSystemService.Application/SaxsSpot.NanoSystemService.Application.csproj", "SaxsSpot.NanoSystemService.Application/"]
COPY ["SaxsSpot.NanoSystemService.Contracts/SaxsSpot.NanoSystemService.Contracts.csproj", "SaxsSpot.NanoSystemService.Contracts/"]
COPY ["SaxsSpot.NanoSystemService.Domain/SaxsSpot.NanoSystemService.Domain.csproj", "SaxsSpot.NanoSystemService.Domain/"]
COPY ["SaxsSpot.NanoSystemService.Storage/SaxsSpot.NanoSystemService.Storage.csproj", "SaxsSpot.NanoSystemService.Storage/"]
RUN dotnet nuget add source https://local.devspot.tech/git/api/packages/SaxsSpot/nuget/index.json --name saxscalc

RUN dotnet restore "SaxsSpot.NanoSystemService.Host/SaxsSpot.NanoSystemService.Host.csproj"
COPY . .
WORKDIR "/src/SaxsSpot.NanoSystemService.Host"
RUN dotnet build "SaxsSpot.NanoSystemService.Host.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "SaxsSpot.NanoSystemService.Host.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "SaxsSpot.NanoSystemService.Host.dll"]
# docker build --build-arg CRT_PATH="local.devspot.tech.pem" . -t "jordenndev/saxsspot-nanosystem-service"