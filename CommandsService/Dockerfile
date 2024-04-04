FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build-env
WORKDIR /app

COPY CommandsService.csproj ./
RUN dotnet restore

COPY . ./
RUN dotnet publish "CommandsService.csproj" -c Release -o /app/out

FROM mcr.microsoft.com/dotnet/aspnet:6.0
WORKDIR /app
COPY --from=build-env /app/out .
ENTRYPOINT [ "dotnet", "CommandsService.dll" ]