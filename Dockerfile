FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

COPY AgroConecta.sln ./
COPY src/ ./src/

RUN dotnet restore src/AgroConecta.Web/AgroConecta.Web.csproj
RUN dotnet publish src/AgroConecta.Web/AgroConecta.Web.csproj \
    -c Release \
    -o /app/publish \
    /p:UseAppHost=false

FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS final
WORKDIR /app

ENV ASPNETCORE_URLS=http://+:8080
ENV ASPNETCORE_ENVIRONMENT=Production
EXPOSE 8080

COPY --from=build /app/publish .

ENTRYPOINT ["dotnet", "AgroConecta.Web.dll"]
