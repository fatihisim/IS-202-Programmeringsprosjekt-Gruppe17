# Programmeringsprosjekt (IS-202) – Gruppe 17  
**Applikasjon:** NRL Hindringsrapportering (Obstacle Reporting)

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
- ✅ **Enhetstester** med xUnit

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

## 🚀 Drift (Kjøring og Konfigurasjon)

### **Krav:**
- Docker Desktop
- .NET SDK 9.0 (for lokal utvikling)

### **1. Kjøring med Docker (Anbefalt)**

```bash
# Klon repository
git clone https://github.com/fatihisim/IS-202-Programmeringsprosjekt-Gruppe17.git
cd IS-202-Programmeringsprosjekt-Gruppe17

# Start applikasjon og database
docker-compose up -d
```

**Applikasjonen er tilgjengelig på:**  
👉 **http://localhost:8080**

**Database:**  
- MariaDB kjører automatisk i container
- Database opprettes automatisk ved første oppstart

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
                    │
┌───────────────────▼─────────────────────────────────┐
│            Docker-miljø                             │
│   - is202nrlapp-web-1 (ASP.NET Core)               │
│   - is202nrlapp-mariadb-1 (MariaDB)                │
│   - Docker Compose network                          │
└─────────────────────────────────────────────────────┘
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
│   │   ├── Shared/
│   │   │   ├── Error.cshtml           # Feilside
│   │   │   ├── _Layout.cshtml         # Hovedmal med navigasjon
│   │   │   ├── _Layout.cshtml.css     # Layout-styling
│   │   │   └── _ValidationScriptsPartial.cshtml
│   │   ├── _ViewImports.cshtml
│   │   └── _ViewStart.cshtml
│   ├── Data/
│   │   └── AppDbContext.cs            # EF Core DbContext (IdentityDbContext)
│   ├── Migrations/                    # Database-migrasjoner
│   ├── Properties/                    # Launch settings
│   ├── wwwroot/                       # Statiske filer (CSS, JS, images)
│   ├── Program.cs                     # Konfigurasjon, middleware, security headers
│   ├── Dockerfile                     # Multi-stage Docker build
│   ├── docker-compose.yml             # Docker Compose konfigurasjon
│   ├── appsettings.json               # Applikasjonskonfigurasjon
│   └── IS202.NrlApp.csproj            # Prosjektfil
│
├── IS202.NrlApp.Tests/                # Testprosjekt
│   ├── Controllers/
│   │   └── ObstacleControllerTests.cs # Controller enhetstester (12 tester)
│   ├── Models/
│   │   └── ObstacleTests.cs           # Model enhetstester (12 tester)
│   ├── Security/
│   │   └── SecurityTests.cs           # Sikkerhetstester (9 tester)
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

Prosjektet inneholder et fullstendig xUnit-testprosjekt med følgende testklasser:

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

### **📊 Test Oppsummering**

| Kategori | Antall Tester | Status |
|----------|---------------|--------|
| **Model-tester** | 15 | ✅ |
| **Controller-tester** | 12 | ✅ |
| **Sikkerhetstester** | 18 | ✅ |
| **TOTALT** | **45 tester** | ✅ |

**Kjør testene med:**
```bash
cd IS202.NrlApp.Tests
dotnet test
```

---

## 👥 Brukerroller

### **1. Pilot**
- ✅ Registrere seg og logge inn
- ✅ Rapportere hindringer (punkt, linje, polygon, sirkel)
- ✅ Se egne rapporter (MyReports)
- ✅ Redigere pending/rejected rapporter
- ✅ Motta tilbakemelding fra registerførere

### **2. Registerfører (NRL-offiser)**
- ✅ Dashboard med oversikt over alle rapporter
- ✅ Godkjenne rapporter med tilbakemelding
- ✅ Avvise rapporter med tilbakemelding
- ✅ Redigere alle rapporter

---

## 🗺️ Kartfunksjonalitet

### **Støttede geometrityper:**

| Type | Beskrivelse | Bruksområde |
|------|-------------|-------------|
| **Point** 📍 | Enkelt punkt | Tårn, mast, kran |
| **LineString** ━ | Linje mellom punkter | Kraftlinjer (cyan farge) |
| **Polygon** ⬟ | Område/bygning | Bygninger, industriområder |
| **Circle** ⭕ | Sirkel med radius | Faresoner |

### **Kartlag:**
1. **Grunnlag:** Esri World Imagery (satellittbilder)
2. **Overlay:** OpenStreetMap etiketter (semi-transparent)

---

## 👥 Bidragsytere

**Gruppe 17 - Universitetet i Agder**  
**Emne:** IS-202 - Programmeringsprosjekt

---

## 📄 Lisens

Dette prosjektet er utviklet som en del av undervisningen ved Universitetet i Agder.

---

**Bygget med ❤️ av Gruppe 17 ved UiA**