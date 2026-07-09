FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

COPY VillaAgency.Entity/VillaAgency.Entity.csproj VillaAgency.Entity/
COPY VillaAgency.Dto/VillaAgency.Dto.csproj VillaAgency.Dto/
COPY VillaAgency.DataAccess/VillaAgency.DataAccess.csproj VillaAgency.DataAccess/
COPY VillaAgency.Business/VillaAgency.Business.csproj VillaAgency.Business/
COPY VillaAgency.UI/VillaAgency.WebUI.csproj VillaAgency.UI/

RUN dotnet restore VillaAgency.UI/VillaAgency.WebUI.csproj

COPY . .
RUN dotnet publish VillaAgency.UI/VillaAgency.WebUI.csproj -c Release -o /app/publish

FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS runtime
WORKDIR /app
COPY --from=build /app/publish .

ENV ASPNETCORE_URLS=http://+:8080
EXPOSE 8080

ENTRYPOINT ["dotnet", "VillaAgency.WebUI.dll"]
