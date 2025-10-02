# IS-202-Programmeringsprosjekt-Gruppe17
ASP.NET Core MVC web application for reporting obstacles to NRL (Nasjonalt register over luftfartshindre). Group 17 project for IS-202 at UiA.

## Introduksjon
Dette prosjektet er en ASP.NET Core MVC-applikasjon for rapportering av hindringer til NRL (Nasjonalt register over luftfartshindre). 
Applikasjonen lar brukere registrere hindringer gjennom et skjema, lagre data i en SQLite-database, og vise resultatene både i en tabell og på et kart.

## Systemarkitektur
Applikasjonen er bygget som en klassisk **ASP.NET Core MVC-applikasjon** med følgende hovedkomponenter:

- **Model**: Representerer hindringer (Obstacle) med felter som navn på rapportør, organisasjon, type hindring, kommentar, posisjon (latitude/longitude) og tidspunkt.
- **View**: Viser data til brukeren, inkludert en tabellvisning og et kart med markører.
- **Controller**: Håndterer logikken mellom brukerens forespørsler og databasen, samt oppdatering av visninger.

Data lagres i en **SQLite-database**, og vises både i en **tabell** og på et **interaktivt Leaflet-kart**.

## Testing
Vi har gjennomført manuell testing av applikasjonen for å sikre at kravene er oppfylt:

- **GET-forespørsler**: Testet ved å hente listen over registrerte hindringer.  
- **POST-forespørsler**: Testet ved å sende inn skjema med nye hindringer.  
- **Kartvisning**: Testet at innsendte hindringer vises som markører på Leaflet-kartet.  
- **Docker**: Applikasjonen ble bygget og kjørt i en Docker-container for å bekrefte at den fungerer uavhengig av utviklingsmiljøet.  

Resultat: Alle funksjonaliteter fungerte som forventet.


