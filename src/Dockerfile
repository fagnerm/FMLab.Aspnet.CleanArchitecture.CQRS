FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /build

COPY Directory.Packages.props /

COPY FMLab.Aspnet.CleanArchitecture.Domain/FMLab.Aspnet.CleanArchitecture.Domain.csproj \
     FMLab.Aspnet.CleanArchitecture.Domain/

COPY FMLab.Aspnet.CleanArchitecture.Application/FMLab.Aspnet.CleanArchitecture.Application.csproj \
     FMLab.Aspnet.CleanArchitecture.Application/

COPY FMLab.Aspnet.CleanArchitecture.Infrastructure/FMLab.Aspnet.CleanArchitecture.Infrastructure.csproj \
     FMLab.Aspnet.CleanArchitecture.Infrastructure/

COPY FMLab.Aspnet.CleanArchitecture.Api/FMLab.Aspnet.CleanArchitecture.Api.csproj \
     FMLab.Aspnet.CleanArchitecture.Api/

RUN dotnet restore FMLab.Aspnet.CleanArchitecture.Api/FMLab.Aspnet.CleanArchitecture.Api.csproj

COPY . .

RUN dotnet publish FMLab.Aspnet.CleanArchitecture.Api/FMLab.Aspnet.CleanArchitecture.Api.csproj \
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
