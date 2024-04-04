FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build-env
WORKDIR /app

COPY PlatformService.csproj ./
RUN dotnet restore

COPY . ./
RUN dotnet publish "PlatformService.csproj" -c Release -o /app/out

FROM mcr.microsoft.com/dotnet/aspnet:6.0
WORKDIR /app
COPY --from=build-env /app/out .
ENTRYPOINT [ "dotnet", "PlatformService.dll" ]