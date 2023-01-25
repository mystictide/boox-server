FROM mcr.microsoft.com/dotnet/sdk:6.0 as build-env
WORKDIR /src
COPY boox.api/*.csproj .
RUN dotnet restore
COPY boox.api/ .
RUN dotnet publish -c Release -o /publish

FROM mcr.microsoft.com/dotnet/aspnet:6.0 as runtime
WORKDIR /publish
COPY --from=build-env /publish .
EXPOSE 8585
ENTRYPOINT ["dotnet", "boox.api.dll"]