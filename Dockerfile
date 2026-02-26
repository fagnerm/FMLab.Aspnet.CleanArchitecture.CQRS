FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /build

COPY src/Directory.Packages.props src/

COPY src/FMLab.Aspnet.CleanArchitecture.Domain/FMLab.Aspnet.CleanArchitecture.Domain.csproj \
     src/FMLab.Aspnet.CleanArchitecture.Domain/

COPY src/FMLab.Aspnet.CleanArchitecture.Application/FMLab.Aspnet.CleanArchitecture.Application.csproj \
     src/FMLab.Aspnet.CleanArchitecture.Application/

COPY src/FMLab.Aspnet.CleanArchitecture.Infrastructure/FMLab.Aspnet.CleanArchitecture.Infrastructure.csproj \
     src/FMLab.Aspnet.CleanArchitecture.Infrastructure/

COPY src/FMLab.Aspnet.CleanArchitecture.Api/FMLab.Aspnet.CleanArchitecture.Api.csproj \
     src/FMLab.Aspnet.CleanArchitecture.Api/

RUN dotnet restore src/FMLab.Aspnet.CleanArchitecture.Api/FMLab.Aspnet.CleanArchitecture.Api.csproj

COPY src/ src/

RUN dotnet publish src/FMLab.Aspnet.CleanArchitecture.Api/FMLab.Aspnet.CleanArchitecture.Api.csproj \
    --configuration Release \
    --no-restore \
    --output /app/publish

FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS runtime
WORKDIR /app

EXPOSE 8080

ENV ASPNETCORE_HTTP_PORTS=8080 \
    ASPNETCORE_ENVIRONMENT=Production

COPY --from=build /app/publish .

ENTRYPOINT ["dotnet", "FMLab.Aspnet.CleanArchitecture.Api.dll"]
