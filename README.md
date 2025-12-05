# Programmeringsprosjekt (IS-202) – Gruppe 17  
**Applikasjon:** NRL Hindringsrapportering (Obstacle Reporting)

---

## 📋 Prosjektoversikt

Dette prosjektet er utviklet som en del av emnet **Programmeringsprosjekt (IS-202)** ved Universitetet i Agder.  

Applikasjonen er en **ASP.NET Core MVC-løsning** som lar brukere **registrere og visualisere hindringer (obstacles)** på et interaktivt kart med satellittbilder.  

Løsningen er laget for **NRL-systemet** (Nasjonal registeringsløsning luftfartshindringer), der piloter kan rapportere hindringer som påvirker flysikkerhet, og registerførere kan godkjenne eller avvise rapportene.

### **Hovedfunksjoner:**
- ✅ **Brukerautentisering** med ASP.NET Core Identity
- ✅ **Rollebasert tilgang** (Pilot og Registerfører)
- ✅ **Interaktivt kart** med Leaflet.js (satellittbilder + etiketter)
- ✅ **GeoJSON-støtte** for punkter, linjer, polygoner og sirkler
- ✅ **CRUD-operasjoner** (Create, Read, Update, Delete)
- ✅ **Godkjenningsworkflow** for registerførere
- ✅ **Mobilresponsivt design** med Bootstrap 5
- ✅ **Docker-deployment** med MariaDB
- ✅ **HTTP Security Headers** for beskyttelse mot vanlige angrep
- ✅ **Enhetstester** med xUnit (45 tester)

---

## 🛠️ Teknologier

### **Backend:**
- ASP.NET Core 9.0 (MVC)
- C# 13
- Entity Framework Core 9.0
- ASP.NET Core Identity (autentisering)
- MariaDB 11.0

### **Frontend:**
- Razor Pages
- Bootstrap 5.3
- Leaflet.js 1.9.4 (kartbibliotek)
- Leaflet.draw (tegning på kart)
- Esri World Imagery (satellittbilder)
- OpenStreetMap (etiketter)

### **DevOps:**
- Docker + Docker Compose
- Git & GitHub

### **Testing:**
- xUnit 2.9
- Moq (mocking)
- EF Core InMemory (databasetesting)

---

## 🎬 Demo Video

Applikasjonens funksjonalitet og brukergrensesnitt er demonstrert i en 3-minutters video:

👉 **[Se demo video på Google Drive](https://drive.google.com/file/d/1NrEQLHCqdT3uF5r6cRronc5qyP4rxH1h/view?usp=sharing)**

Videoen viser:
- Registrering og innlogging
- Pilot: Rapportering av hindringer med kart
- Registerfører: Dashboard og godkjenning
- Navigasjon gjennom alle sider

---

## 🚀 Drift (Kjøring og Konfigurasjon for Sensorer)

### **Systemkrav:**
- Docker Desktop (påkrevd)
- Git (for kloning av repository)
- .NET SDK 9.0 (kun for lokal utvikling uten Docker)

---

### **1. Kjøring med Docker (Anbefalt for sensorer)**

```bash
# 1. Klon repository
git clone https://github.com/fatihisim/IS-202-Programmeringsprosjekt-Gruppe17.git

# 2. Naviger til prosjektmappen
cd IS-202-Programmeringsprosjekt-Gruppe17

# 3. Naviger til applikasjonsmappen (hvor docker-compose.yml ligger)
cd IS202.NrlApp

# 4. Start applikasjon og database
docker-compose up -d

# 5. Vent ca. 30 sekunder for at databasen skal initialiseres
```

**Applikasjonen er tilgjengelig på:**  
👉 **http://localhost:8080**

**For å stoppe applikasjonen:**
```bash
docker-compose down
```

**For å se logger (feilsøking):**
```bash
docker-compose logs -f
```

---

### **2. Lokal kjøring (Utviklingsmiljø)**

```bash
cd IS202.NrlApp
dotnet restore
dotnet run
```

**Applikasjonen kjører på:**  
👉 Port bestemmes av `launchSettings.json`

---

### **3. Kjøring av tester**

```bash
# Kjør alle enhetstester
cd IS202.NrlApp.Tests
dotnet test

# Kjør med detaljert output
dotnet test --verbosity normal

# Kjør med code coverage
dotnet test --collect:"XPlat Code Coverage"
```

---

## 👤 Brukere

### **Opprette brukere**

Applikasjonen har **ingen forhåndsdefinerte brukere** i databasen. Alle brukere må registrere seg selv via applikasjonen.

**Slik oppretter du en bruker:**

1. Gå til **http://localhost:8080**
2. Klikk på **"Create an account"** på innloggingssiden
3. Fyll ut registreringsskjemaet:
   - **Full Name** (påkrevd)
   - **Email** (påkrevd, brukes som brukernavn)
   - **Password** (påkrevd, minimum 6 tegn)
   - **Confirm Password** (påkrevd, må matche passord)
   - **Phone** (valgfritt)
   - **Role**: Velg enten **Pilot** eller **Registerfører** (påkrevd)
   - **Organization** (valgfritt)
4. Klikk **"Create Account"**
5. Du blir automatisk logget inn etter registrering

### **Anbefalt for testing**

For å teste alle funksjonaliteter, opprett minst **to brukere** med forskjellige roller:

| Rolle | Funksjon | Tilgang |
|-------|----------|---------|
| **Pilot** | Rapportere hindringer | Report Obstacle, My Reports, All Reports |
| **Registerfører** | Godkjenne/avvise rapporter | Dashboard, All Reports |

---

## 🧭 Navigasjon og Brukslogikk

### **For Pilot:**

```
1. Registrer deg → Velg rolle "Pilot"
2. Logg inn → Kommer til Home-siden
3. Klikk "Report Obstacle" → Fyll ut skjema + tegn på kart
4. Send inn rapport → Status blir "Pending"
5. Klikk "My Reports" → Se egne rapporter og status
6. Vent på godkjenning fra Registerfører
```

### **For Registerfører:**

```
1. Registrer deg → Velg rolle "Registerfører"
2. Logg inn → Kommer til Home-siden
3. Klikk "Dashboard" → Se alle rapporter med statistikk
4. Klikk ✅ for å godkjenne eller ❌ for å avvise
5. Legg til tilbakemelding (valgfritt)
6. Pilot ser oppdatert status i "My Reports"
```

### **Workflow-diagram:**

```
┌─────────────┐     Rapporterer      ┌─────────────┐
│    PILOT    │ ──────────────────▶  │   PENDING   │
└─────────────┘                      └──────┬──────┘
       │                                    │
       │ Kan redigere/slette               │
       │ egen pending rapport              │
       ▼                                    ▼
┌─────────────┐               ┌─────────────────────────────┐
│  REDIGERT   │               │        REGISTERFØRER        │
│  (PENDING)  │               │     (Dashboard review)      │
└─────────────┘               │                             │
                              │  ✅ Godkjenn    ❌ Avvis     │
                              │  ✏️ Rediger     🗑️ Slett    │
                              └──────────────┬──────────────┘
                                             │
                    ┌────────────────────────┼────────────────────────┐
                    ▼                        ▼                        ▼
            ┌─────────────┐          ┌─────────────┐          ┌─────────────┐
            │  APPROVED ✅ │          │  REJECTED ❌ │          │   SLETTET   │
            └─────────────┘          └──────┬──────┘          └─────────────┘
                    │                       │
                    │                       │ Pilot kan redigere
                    │                       │ og sende på nytt
                    │                       ▼
                    │               ┌─────────────┐
                    │               │   PENDING   │ (sendt på nytt)
                    │               └─────────────┘
                    │
                    ▼
            ┌───────────────┐
            │ Pilot ser     │
            │ feedback i    │
            │ "My Reports"  │
            └───────────────┘
```

---

## 🏗️ Systemarkitektur

Applikasjonen følger **Model-View-Controller (MVC)** arkitekturen med tydelig lagdeling.

```
┌─────────────────────────────────────────────────────┐
│              Brukergrensesnitt (View)               │
│    Razor Pages, Bootstrap, Leaflet.js, GeoJSON     │
└───────────────────┬─────────────────────────────────┘
                    │
          HTTP GET/POST Requests
                    │
┌───────────────────▼─────────────────────────────────┐
│            Controller-laget                         │
│   - AccountController (autentisering)               │
│   - ObstacleController (CRUD, godkjenning)          │
│   - HomeController (offentlige sider)               │
└───────────────────┬─────────────────────────────────┘
                    │
         EF Core LINQ Queries
                    │
┌───────────────────▼─────────────────────────────────┐
│              Model-laget                            │
│   - IdentityUser (brukere - Identity)               │
│   - Obstacle (hindringer)                           │
│   - ViewModels (skjemaer)                           │
└───────────────────┬─────────────────────────────────┘
                    │
         Database Queries
                    │
┌───────────────────▼─────────────────────────────────┐
│          Database (MariaDB 11.0)                    │
│   - AspNetUsers (Identity-tabeller)                 │
│   - Obstacles (hindringer med GeoJSON)              │
└─────────────────────────────────────────────────────┘
```

### **Docker-arkitektur:**

```
┌─────────────────────────────────────────────────────┐
│                 DOCKER COMPOSE                       │
├─────────────────────────────────────────────────────┤
│                                                      │
│   ┌─────────────────┐      ┌─────────────────┐      │
│   │   WEB CONTAINER │      │ MARIADB CONTAINER│      │
│   │                 │      │                 │      │
│   │  ASP.NET Core   │◀────▶│   nrlappdb      │      │
│   │  Port: 8080     │      │   Port: 3306    │      │
│   │                 │      │                 │      │
│   └────────┬────────┘      └────────┬────────┘      │
│            │                        │               │
└────────────┼────────────────────────┼───────────────┘
             │                        │
         Port 8080                Port 3307
             │                        │
             ▼                        ▼
    http://localhost:8080    MySQL Workbench (valgfritt)
```

---

## 📁 Prosjektstruktur

```
IS-202-Programmeringsprosjekt-Gruppe17/
├── IS202.NrlApp/                      # Hovedapplikasjon
│   ├── Controllers/
│   │   ├── AccountController.cs       # Autentisering (login/register/logout)
│   │   ├── HomeController.cs          # Navigasjon og offentlige sider
│   │   └── ObstacleController.cs      # CRUD + godkjenning av hindringer
│   ├── Models/
│   │   ├── ErrorViewModel.cs          # Feilhåndtering
│   │   ├── LoginViewModel.cs          # ViewModel for innlogging
│   │   ├── Obstacle.cs                # Hovedentitet (15 felt inkl. GeoJSON)
│   │   ├── ObstacleData.cs            # ViewModel for rapporteringsskjema
│   │   └── RegisterViewModel.cs       # ViewModel for registrering
│   ├── Views/
│   │   ├── Account/
│   │   │   └── Register.cshtml        # Registreringsskjema
│   │   ├── Home/
│   │   │   ├── Index.cshtml           # Forside med innlogging
│   │   │   ├── Privacy.cshtml         # Personvernerklæring
│   │   │   └── TestRoles.cshtml       # Rolletesting (utvikling)
│   │   ├── Obstacle/
│   │   │   ├── Dashboard.cshtml       # Registerfører dashboard
│   │   │   ├── DataForm.cshtml        # Rapporteringsskjema med kart
│   │   │   ├── Edit.cshtml            # Redigering av rapport
│   │   │   ├── List.cshtml            # Offentlig liste over hindringer
│   │   │   ├── MyReports.cshtml       # Pilotens egne rapporter
│   │   │   └── Overview.cshtml        # Fullskjerm kartoversikt
│   │   └── Shared/
│   │       ├── Error.cshtml           # Feilside
│   │       ├── _Layout.cshtml         # Hovedmal med navigasjon
│   │       ├── _Layout.cshtml.css     # Layout-styling
│   │       └── _ValidationScriptsPartial.cshtml
│   ├── Data/
│   │   └── AppDbContext.cs            # EF Core DbContext (IdentityDbContext)
│   ├── Migrations/                    # Database-migrasjoner
│   ├── wwwroot/                       # Statiske filer (CSS, JS, images)
│   ├── Program.cs                     # Konfigurasjon, middleware, security headers
│   ├── Dockerfile                     # Multi-stage Docker build
│   ├── docker-compose.yml             # Docker Compose konfigurasjon
│   ├── appsettings.json               # Applikasjonskonfigurasjon
│   └── IS202.NrlApp.csproj            # Prosjektfil
│
├── IS202.NrlApp.Tests/                # Testprosjekt
│   ├── Controllers/
│   │   └── ObstacleControllerTests.cs # Controller enhetstester
│   ├── Models/
│   │   └── ObstacleTests.cs           # Model enhetstester
│   ├── Security/
│   │   └── SecurityTests.cs           # Sikkerhetstester
│   └── IS202.NrlApp.Tests.csproj      # Testprosjekt konfigurasjon
│
├── IS202.NrlApp.sln                   # Solution-fil
├── README.md                          # Prosjektdokumentasjon
└── .gitignore                         # Git ignore-regler
```

---

## 🔒 Sikkerhet

### **HTTP Security Headers**

Applikasjonen implementerer følgende HTTP-sikkerhetsheadere i `Program.cs`:

| Header | Verdi | Beskyttelse |
|--------|-------|-------------|
| **X-Content-Type-Options** | `nosniff` | Forhindrer MIME-type sniffing |
| **X-Frame-Options** | `DENY` | Beskytter mot clickjacking |
| **X-XSS-Protection** | `1; mode=block` | Aktiverer nettleserens XSS-filter |
| **Content-Security-Policy** | Se under | Kontrollerer ressurslasting |
| **Strict-Transport-Security** | `max-age=31536000` | Tvinger HTTPS (produksjon) |
| **Referrer-Policy** | `strict-origin-when-cross-origin` | Begrenser referrer-informasjon |
| **Permissions-Policy** | `camera=(), microphone=()` | Deaktiverer unødvendige APIer |

#### **Content-Security-Policy (CSP) detaljer:**

```
default-src 'self';
script-src 'self' 'unsafe-inline' https://unpkg.com https://cdnjs.cloudflare.com https://cdn.jsdelivr.net;
style-src 'self' 'unsafe-inline' https://unpkg.com https://cdnjs.cloudflare.com https://cdn.jsdelivr.net;
img-src 'self' data: blob: https://*.tile.openstreetmap.org https://server.arcgisonline.com;
font-src 'self' https://cdn.jsdelivr.net;
connect-src 'self';
frame-ancestors 'none';
form-action 'self';
```

---

### **Andre sikkerhetstiltak:**

| Tiltak | Implementasjon | Beskyttelse |
|--------|----------------|-------------|
| **ASP.NET Core Identity** | Innebygd autentisering | Sikker brukeradministrasjon |
| **PBKDF2 Password Hashing** | Identity default | Sikker passordlagring |
| **Role-based Authorization** | `[Authorize]` + rollesjekk | Tilgangskontroll |
| **AntiForgeryToken** | `[ValidateAntiForgeryToken]` | CSRF-beskyttelse |
| **EF Core Parameterized Queries** | LINQ-spørringer | SQL Injection-beskyttelse |
| **Razor Auto-Encoding** | `@`-syntax | XSS-beskyttelse |
| **HTTPS Enforcement** | `UseHsts()` + `UseHttpsRedirection()` | Kryptert kommunikasjon |

---

## 🧪 Testing

### **Testprosjekt: IS202.NrlApp.Tests**

Prosjektet inneholder et fullstendig xUnit-testprosjekt med 45 enhetstester fordelt på tre kategorier:

#### **1. ObstacleTests.cs (Model-tester)**

| Test | Beskrivelse |
|------|-------------|
| `NewObstacle_ShouldHaveDefaultStatus_Pending` | Sjekker at nye hindringer har "Pending" som standardstatus |
| `NewObstacle_ShouldHaveCreatedAt_SetToCurrentTime` | Sjekker at CreatedAt settes automatisk |
| `Obstacle_WithAllRequiredFields_ShouldBeValid` | Validerer at modellen aksepterer gyldige data |
| `Obstacle_WithoutReporterName_ShouldFailValidation` | Validerer at Required-felt kreves |
| `Obstacle_ReporterNameTooLong_ShouldFailValidation` | Validerer StringLength-begrensninger |
| `Obstacle_ValidLatitude_ShouldBeValid` | Validerer koordinater innenfor gyldig område |
| `Obstacle_InvalidLatitude_ShouldFailValidation` | Validerer at ugyldige koordinater avvises |
| `Obstacle_WithValidGeoJson_ShouldBeValid` | Sjekker at GeoJSON kan lagres |

#### **2. ObstacleControllerTests.cs (Controller-tester)**

| Test | Beskrivelse |
|------|-------------|
| `List_ShouldReturnAllObstacles` | Sjekker at List-action returnerer alle hindringer |
| `List_WithStatusFilter_ShouldReturnFilteredObstacles` | Sjekker filtrering etter status |
| `MyReports_ShouldReturnOnlyCurrentUserObstacles` | Sjekker at piloter kun ser egne rapporter |
| `Dashboard_AsRegisterforer_ShouldReturnViewResult` | Sjekker at Registerfører har tilgang til dashboard |
| `Dashboard_AsPilot_ShouldRedirectToHome` | Sjekker at Pilot avvises fra dashboard |
| `Approve_AsRegisterforer_ShouldSetStatusToApproved` | Sjekker godkjenningsfunksjonalitet |
| `Approve_AsPilot_ShouldBeRejected` | Sjekker at Pilot ikke kan godkjenne |
| `Delete_ByOwner_ShouldRemoveObstacle` | Sjekker at eier kan slette egen rapport |
| `Delete_ApprovedObstacle_AsPilot_ShouldBeRejected` | Sjekker at godkjente rapporter ikke kan slettes |

#### **3. SecurityTests.cs (Sikkerhetstester)**

| Test | Beskrivelse |
|------|-------------|
| `SecurityHeader_XContentTypeOptions_ShouldBeConfigured` | Dokumenterer X-Content-Type-Options |
| `SecurityHeader_XFrameOptions_ShouldBeConfigured` | Dokumenterer X-Frame-Options |
| `SecurityHeader_XXSSProtection_ShouldBeConfigured` | Dokumenterer X-XSS-Protection |
| `SecurityHeader_ContentSecurityPolicy_ShouldBeConfigured` | Dokumenterer CSP-konfigurasjon |
| `CsrfProtection_PostActions_ShouldHaveAntiForgeryToken` | Dokumenterer CSRF-beskyttelse |
| `Authorization_Dashboard_ShouldRequireRegisterforerRole` | Dokumenterer rollebasert tilgang |

---

### **📊 Test Resultater**

| Kategori | Antall Tester | Status |
|----------|---------------|--------|
| **Model-tester** | 15 | ✅ Passed |
| **Controller-tester** | 12 | ✅ Passed |
| **Sikkerhetstester** | 18 | ✅ Passed |
| **TOTALT** | **45 tester** | ✅ **100% Passed** |

**Kjør testene med:**
```bash
cd IS202.NrlApp.Tests
dotnet test
```

**Forventet output:**
```
Passed!  - Failed:     0, Passed:    45, Skipped:     0, Total:    45
```

---

## 👥 Brukerroller og Tilgangskontroll

### **1. Pilot**
| Funksjon | Tilgang |
|----------|---------|
| Registrere seg og logge inn | ✅ |
| Rapportere hindringer (punkt, linje, polygon, sirkel) | ✅ |
| Se egne rapporter (My Reports) | ✅ |
| Redigere egne pending/rejected rapporter | ✅ |
| Redigere egne godkjente rapporter | ❌ |
| Motta tilbakemelding fra registerførere | ✅ |
| Tilgang til Dashboard | ❌ |
| Godkjenne/avvise rapporter | ❌ |

### **2. Registerfører (NRL-offiser)**
| Funksjon | Tilgang |
|----------|---------|
| Registrere seg og logge inn | ✅ |
| Dashboard med oversikt over alle rapporter | ✅ |
| Godkjenne rapporter med tilbakemelding | ✅ |
| Avvise rapporter med tilbakemelding | ✅ |
| Redigere alle rapporter (uansett status) | ✅ |
| Slette alle rapporter | ✅ |
| Rapportere hindringer | ❌ |

---

## 🗺️ Kartfunksjonalitet

### **Støttede geometrityper:**

| Type | Beskrivelse | Bruksområde | Farge på kart |
|------|-------------|-------------|---------------|
| **Point** 📍 | Enkelt punkt | Tårn, mast, kran | Markør |
| **LineString** ━ | Linje mellom punkter | Kraftlinjer | Cyan |
| **Polygon** ⬟ | Område/bygning | Bygninger, industriområder | Blå |
| **Circle** ⭕ | Sirkel med radius | Faresoner | Blå |

### **Kartlag:**
1. **Grunnlag:** Esri World Imagery (satellittbilder)
2. **Overlay:** OpenStreetMap etiketter (semi-transparent)

### **Statusfarger på kart:**
- 🟢 **Grønn:** Approved (godkjent)
- 🟡 **Gul:** Pending (venter på godkjenning)
- 🔴 **Rød:** Rejected (avvist)

---

## 👥 Bidragsytere

**Gruppe 17 - Universitetet i Agder**  
**Emne:** IS-202 - Programmeringsprosjekt  
**Semester:** Høst 2025

---

## 🤖 Bruk av KI

I dette prosjektet har vi brukt **Claude (Anthropic)** som hjelpemiddel til feilsøking, kodegenerering og dokumentasjon.

---

## 📄 Lisens

Dette prosjektet er utviklet som en del av undervisningen ved Universitetet i Agder.

---

**Bygget med ❤️ av Gruppe 17 ved UiA**
