# Programmeringsprosjekt (IS-202) – Gruppe 17  
**Applikasjon:** NRL Hindringsrapportering (Obstacle Reporting)

## Prosjektoversikt
Dette prosjektet er utviklet som en del av emnet **Programmeringsprosjekt (IS-202)** ved Universitetet i Agder.  
Applikasjonen er en **ASP.NET Core MVC-løsning** som lar brukere **registrere og visualisere hindringer (obstacles)** på et kart.  
Løsningen er laget for **NRL-systemet**, der piloter og teknisk personell kan rapportere hindringer som påvirker flysikkerhet.

Systemet består av:
- **Brukergrensesnitt (View):** responsive nettsider laget med Bootstrap og Leaflet-kart.
- **Forretningslogikk (Controller):** håndterer GET- og POST-forespørsler, datavalidering og kommunikasjon med databasen.
- **Datamodell (Model):** representerer hindringsinformasjon lagret i en SQLite-database via Entity Framework Core.
- **Docker-miljø:** applikasjonen kjører i en container basert på ASP.NET 9.0 med SQLite-støtte.

Brukeren kan:
1. Åpne et skjema for å registrere en ny hindring.  
2. Klikke på kartet for å hente **koordinater (lat/lng)** automatisk.  
3. Sende inn skjemaet via **POST** og få bekreftelse i en **egen oversiktsside**.  
4. Se alle registrerte hindringer i en tabell og på et dynamisk kart.

Applikasjonen følger **MVC-arkitekturen** og benytter både **server- og klientsidevalidering**.  
All kode og dokumentasjon er tilgjengelig på **GitHub**, og løsningen kan kjøres enten **lokalt** eller i en **Docker-container**.

## Drift (Kjøring og Konfigurasjon)

Denne applikasjonen kan kjøres både **lokalt** og i **Docker-container** uten ekstra oppsett.  
Ved første oppstart opprettes databasen automatisk, og applikasjonen er klar til bruk.

### Krav
- **.NET SDK 9.0**
- *(Valgfritt)* **Docker** installert

### Lokal kjøring
For å starte applikasjonen lokalt:

```bash
cd IS202.NrlApp
dotnet restore
dotnet run
```


Applikasjonen kjører deretter på:  
👉 **http://localhost:5048**

Databasen `app.db` opprettes automatisk første gang applikasjonen kjøres.

### **Docker-kjøring**

For å kjøre applikasjonen i container:

```bash
cd IS202.NrlApp
docker build -t nrl-app .
docker run -p 8080:8080 nrl-app
```

Applikasjonen er da tilgjengelig på:  
👉 **http://localhost:8080**

### **Konfigurasjon**

Applikasjonen bruker SQLite som standard:

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Data Source=app.db"
  }
}

```

For produksjon kan miljøvariabelen  
`ConnectionStrings__DefaultConnection` brukes til å definere ekstern database.

## Systemarkitektur

Applikasjonen er bygget med **ASP.NET Core MVC** og følger prinsippene for **Model–View–Controller**-arkitekturen.  
Løsningen består av tydelig adskilte lag for datahåndtering, forretningslogikk og presentasjon, og kjøres i et container-miljø via Docker.

### Oversikt

┌───────────────────────────────────────────────┐
│ Brukergrensesnitt │
│ (HTML, CSS, Bootstrap, Leaflet, Razor Views) │
└───────────────────────────────────────────────┘
│ ▲
GET/POST │ │ Dynamisk innhold
▼ │
┌───────────────────────────────────────────────┐
│ Controller-laget │
│ (Håndterer forespørsler, validering, logikk) │
└───────────────────────────────────────────────┘
│
▼
┌───────────────────────────────────────────────┐
│ Modell-laget │
│ (Entity Framework Core, SQLite database) │
│ Henter, lagrer og oppdaterer data │
└───────────────────────────────────────────────┘
│
▼
┌───────────────────────────────────────────────┐
│ Docker-miljø │
│ (ASP.NET 9.0 image, EXPOSE 8080, app.db) │
└───────────────────────────────────────────────┘
---

### Forklaring
- **View:** presenterer data og kartinformasjon ved bruk av **Razor Pages** og **Leaflet**.  
- **Controller:** mottar brukerforespørsler, utfører validering, og sender data mellom View og Model.  
- **Model:** definerer dataobjekter (hindringer) og håndterer databaseoperasjoner via **Entity Framework Core**.  
- **Database:** bruker **SQLite** som lagringsløsning.  
- **Docker:** sørger for portabilitet og enkel distribusjon.

Denne strukturen gir en **tydelig separasjon av ansvar**, enkel vedlikehold, og gjør systemet lett å utvide i fremtidige versjoner.

## Testing (Testscenarier og Resultater)

Det ble gjennomført manuell testing for å sikre at applikasjonen fungerer som forventet, både lokalt og i Docker-container.  
Fokuset var på **funksjonalitet, validering, og dataintegritet**.

### Testoppsett
- **Miljø:** Windows 11, .NET 9.0, Docker Desktop  
- **Database:** SQLite (`app.db`)  
- **Verktøy:** Nettleser (Chrome/Edge), Postman for HTTP-forespørsler  

### Testscenarier

| Nr. | Scenario                    | Handling                                                          | Forventet resultat                                           | Status |
|-----|-----------------------------|-------------------------------------------------------------------|--------------------------------------------------------------|--------|
| 1   | Hente startsiden            | Åpne `http://localhost:5048`                                      | Nettsiden lastes uten feil                                   | ✅     |
| 2   | Opprette ny hindring        | Fylle ut skjemaet og sende **POST**                               | Hindringen lagres og vises i oversikten                      | ✅     |
| 3   | Manglende obligatorisk felt | Sende skjema uten tittel                                          | Valideringsfeil vises på skjermen                            | ✅     |
| 4   | Hente data fra kartet       | Klikke på Leaflet-kartet                                          | Koordinater fylles automatisk i skjemaet                     | ✅     |
| 5   | Vise oversiktsside          | Navigere til “Hindringsliste”                                     | Alle registrerte hindringer vises                            | ✅     |
| 6   | Kjøre applikasjonen i Docker| `docker build` og `docker run -p 8080:8080 nrl-app`               | Applikasjonen tilgjengelig på `http://localhost:8080`        | ✅     |

### Resultat
Alle testene ble **godkjent** uten feil.  
Databasen opprettes automatisk, POST-forespørsler lagres korrekt, og valideringen fungerer både på klient- og serversiden.  
Systemet oppfører seg likt i både **lokalt miljø og Docker-miljø**.

