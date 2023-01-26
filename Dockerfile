FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS base
WORKDIR /app

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src
COPY ["boox.api/boox.api.csproj", "boox.api/", "boox.api", "boox.api/media/"]
RUN dotnet restore "boox.api/boox.api.csproj"
COPY . .
WORKDIR "/src/boox.api"
RUN dotnet build "boox.api.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "boox.api.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENV ASPNETCORE_URLS=http://+:477
ENV ASPNETCORE_HTTP_PORT=http://+:477
EXPOSE 477
ENTRYPOINT ["dotnet", "boox.api.dll", "--urls", "http://+:477"]