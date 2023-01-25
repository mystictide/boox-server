FROM mcr.microsoft.com/dotnet/sdk:6.0 as build-env
WORKDIR /src
EXPOSE 8585
COPY boox.api/*.csproj .
RUN dotnet restore
COPY boox.api/ .
RUN dotnet publish -c Release -o /publish

FROM mcr.microsoft.com/dotnet/aspnet:6.0 as runtime
WORKDIR /publish
EXPOSE 8585
COPY --from=build-env /publish .
ENTRYPOINT ["dotnet", "boox.api.dll"]