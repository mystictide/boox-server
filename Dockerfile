FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS base
WORKDIR /app
EXPOSE 477
ENV ASPNETCORE_URLS=http://*:477
ENV ASPNETCORE_HTTP_PORT=http://*:477

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src
COPY ["boox.api/boox.api.csproj", "boox.api/"]
RUN dotnet restore "boox.api/boox.api.csproj"
COPY . .
WORKDIR "/src/boox.api"
RUN dotnet build "boox.api.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "boox.api.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "boox.api.dll"]