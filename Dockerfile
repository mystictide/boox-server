FROM mcr.microsoft.com/dotnet/sdk:6.0 as build-env
WORKDIR /src
EXPOSE 87
EXPOSE 887
COPY boox.api/*.csproj .
RUN dotnet restore
COPY boox.api/ .
RUN dotnet publish -c Release -o /publish

FROM mcr.microsoft.com/dotnet/aspnet:6.0 as runtime
WORKDIR /publish
EXPOSE 87
EXPOSE 887
COPY --from=build-env /publish .
ENTRYPOINT ["dotnet", "boox.api.dll"]