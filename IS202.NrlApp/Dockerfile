# 🇳🇴 Bruk .NET 9 SDK for å bygge applikasjonen (oppdatert fra 8.0)
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src

# 🇳🇴 Kopier prosjektfilen og gjenopprett avhengigheter (nuget-pakker)
COPY IS202.NrlApp.csproj ./
RUN dotnet restore

# 🇳🇴 Kopier resten av prosjektet til containeren
COPY . .

# 🇳🇴 Bygg applikasjonen i "Release"-modus og legg det ferdige resultatet i /app/publish
RUN dotnet publish -c Release -o /app/publish

# 🇳🇴 Bruk .NET 9 runtime (lettere versjon, kun for å kjøre applikasjonen)
FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS runtime
WORKDIR /app

# 🇳🇴 Kopier de ferdig bygde filene fra forrige steg (build) til runtime containeren
COPY --from=build /app/publish .

# 🇳🇴 Eksponer port 8080 slik at applikasjonen kan nås utenfra
EXPOSE 8080

# 🇳🇴 Start applikasjonen når containeren kjører
ENTRYPOINT ["dotnet", "IS202.NrlApp.dll"]
