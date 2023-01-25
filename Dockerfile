FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS base
WORKDIR /app
EXPOSE 87
EXPOSE 887

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src
COPY boox.api/*.csproj .
COPY boox.api/ .
RUN dotnet restore
COPY . .
WORKDIR /src/boox
RUN dotnet publish -c Release -o /src/build

FROM build AS publish
RUN dotnet publish -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "boox.api.dll"]