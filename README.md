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

---

## 🛠️ Teknologier

### **Backend:**
- ASP.NET Core 8.0 (MVC)
- C# 12
- Entity Framework Core 8.0
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

---

## 🚀 Drift (Kjøring og Konfigurasjon)

### **Krav:**
- Docker Desktop
- .NET SDK 8.0 (for lokal utvikling)

### **1. Kjøring med Docker (Anbefalt)**

```bash
# Klon repository (oppgave2 branch)
git clone -b oppgave2 https://github.com/fatihisim/IS-202-Programmeringsprosjekt-Gruppe17.git
cd IS-202-Programmeringsprosjekt-Gruppe17

# Start applikasjon og database
docker-compose up -d
```

**NB:** Bruk `oppgave2` branch - dette er den mest oppdaterte versjonen.

**Applikasjonen er tilgjengelig på:**  
👉 **http://localhost:8080**

**Database:**  
- MariaDB kjører automatisk i container
- Database opprettes automatisk ved første oppstart

---

### **2. Lokal kjøring (Utviklingsmiljø)**

**NB:** Anbefalt metode er Docker. For lokal utvikling:

```bash
cd IS202.NrlApp
dotnet restore
dotnet run
```

**Applikasjonen kjører på:**  
👉 Port bestemmes av `launchSettings.json` (vanligvis 5048 eller 5000)  
👉 Sjekk terminal output for nøyaktig URL

**NB:** Krever lokal MariaDB installasjon eller endre `appsettings.json` til SQLite.

---

### **3. Konfigurasjon**

#### **Database Connection String:**

I `appsettings.json`:

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=nrl-mariadb;Port=3306;Database=nrldb;User=nrluser;Password=YourPassword;"
  }
}
```

For lokal MariaDB:
```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=localhost;Port=3306;Database=nrldb;User=root;Password=YourPassword;"
  }
}
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
│   - nrl-web-app container (ASP.NET Core)            │
│   - nrl-mariadb container (MariaDB)                 │
│   - nrl-network (bridge network)                    │
└─────────────────────────────────────────────────────┘
```

### **Datamodell:**

#### **IdentityUser (ASP.NET Identity)**
- Id, Email, PasswordHash, PhoneNumber
- Role (Pilot / Registerfører)
- Brukes for autentisering og autorisasjon

#### **Obstacle**
- Id, ObstacleType, Comment
- Latitude, Longitude
- **GeometryType** (Point / LineString / Polygon / Circle)
- **GeoJsonData** (full GeoJSON-geometri)
- Status (Pending / Approved / Rejected)
- ReporterId, ProcessedBy, Feedback
- CreatedAt, ProcessedAt

---

## 👥 Brukerroller

### **1. Pilot**
**Funksjonalitet:**
- ✅ Registrere seg som ny bruker
- ✅ Logge inn
- ✅ Rapportere nye hindringer (punkt, linje, polygon, sirkel)
- ✅ Se egne rapporter (MyReports)
- ✅ Redigere pending/rejected rapporter
- ✅ Slette pending/rejected rapporter
- ✅ Motta tilbakemelding fra registerførere

### **2. Registerfører (NRL-offiser)**
**Funksjonalitet:**
- ✅ Dashboard med oversikt over alle rapporter
- ✅ Se pending rapporter
- ✅ Godkjenne rapporter med tilbakemelding
- ✅ Avvise rapporter med tilbakemelding
- ✅ Se alle godkjente hindringer på kart

---

## 🗺️ Kartfunksjonalitet

### **Leaflet.js + Leaflet.draw**

Applikasjonen støtter følgende geometrityper:

| Type | Beskrivelse | Bruksområde |
|------|-------------|-------------|
| **Point** 📍 | Enkelt punkt | Tårn, mast, kran |
| **LineString** ━ | Linje mellom punkter | Kraftlinjer (cyan farge) |
| **Polygon** ⬟ | Område/bygning | Bygninger, industriområder |
| **Circle** ⭕ | Sirkel med radius | Faresoner |

### **Kartlag:**
1. **Grunnlag:** Esri World Imagery (satellittbilder)
2. **Overlay:** OpenStreetMap etiketter (semi-transparent)

### **Interaksjon:**
- Klikk på kart → Plasser marker
- Tegn linje → Velg linjeverktøy, klikk punkter
- Tegn polygon → Velg polygon-verktøy, klikk hjørner
- Tegn sirkel → Velg sirkelverktøy, dra for radius
- **"Use my location"** → Automatisk GPS-posisjon

**GeoJSON lagres i database for presis gjengivelse!**

---

## 🧪 Testing

Applikasjonen er testet gjennom manuelle tester i følgende kategorier:

### **1. Enhetstesting**

Testing av individuelle komponenter og funksjoner:

| Test | Beskrivelse | Forventet resultat | Status |
|------|-------------|-------------------|--------|
| **User Registration** | Registrere ny bruker med gyldig data | Bruker opprettes i database | ✅ |
| **Login Authentication** | Logge inn med korrekt e-post/passord | Redirect til dashboard | ✅ |
| **Create Obstacle (Point)** | Rapportere hindring med punkt | Lagres med status "Pending" | ✅ |
| **Create Obstacle (Line)** | Rapportere kraftlinje med linje | GeoJSON LineString lagres med cyan farge | ✅ |
| **Create Obstacle (Polygon)** | Rapportere bygning med polygon | GeoJSON Polygon lagres | ✅ |
| **Edit Own Report** | Pilot redigerer pending-rapport | Endringer lagres | ✅ |
| **Delete Own Report** | Pilot sletter pending-rapport | Rapport fjernes fra database | ✅ |

**Resultat:** 7/7 tester bestått ✅

---

### **2. Systemstesting**

End-to-end testing av arbeidsflyten:

#### **Scenario 1: Komplett rapporteringsflyt**
**Steg:**
1. Pilot registrerer seg og logger inn
2. Rapporterer en hindring med punkt på kart
3. Navigerer til "My Reports" → Ser "Pending" status
4. Registerfører logger inn og ser rapporten i Dashboard
5. Godkjenner rapporten med tilbakemelding
6. Pilot ser "Approved" status og tilbakemelding

**Resultat:** ✅ PASSED

---

#### **Scenario 2: Kraftlinje med cyan farge**
**Steg:**
1. Pilot logger inn og velger "Power line"
2. Tegner **linje** på kart (2 punkter)
3. Sender inn → GeoJSON LineString lagres
4. Navigerer til Overview → Linjen vises i **cyan farge** (#00ffff)

**Resultat:** ✅ PASSED

---

### **3. Sikkerhetstesting**

Grunnleggende sikkerhetstesting:

| Test | Beskrivelse | Resultat |
|------|-------------|----------|
| **Access Control** | Pilot prøver å åpne admin dashboard (`/Obstacle/Dashboard`) | ✅ Blokkert (redirect til login) |
| **Password Hashing** | Sjekk database - er passord lagret i klartekst? | ✅ Hashet (ikke lesbart) |
| **CSRF Protection** | POST-request uten AntiForgeryToken | ✅ Request blokkert |

**Sikkerhetstiltak implementert:**
- ✅ ASP.NET Core Identity (autentisering)
- ✅ PBKDF2 password hashing
- ✅ Role-based authorization
- ✅ AntiForgeryToken på alle POST-skjemaer
- ✅ EF Core parameteriserte queries (SQL injection-beskyttelse)
- ✅ Razor auto-encoding (XSS-beskyttelse)

**Resultat:** 3/3 sikkerhetstester bestått ✅

---

### **4. Brukervennlighetstesting**

Manuell testing med faktiske brukere:

#### **Scenario: Mobil rapportering**
- **Enheter testet:** iPhone, Android
- **Oppgave:** Rapporter hindring fra mobil enhet
- **Resultat:** ✅ Fungerer godt (responsivt design)
- **Tilbakemelding:** "Intuitiv å bruke, kartet fungerer bra"

---

### **📊 Test Oppsummering**

| Kategori | Antall Tester | Resultat |
|----------|---------------|----------|
| **Enhetstesting** | 7 | ✅ 100% |
| **Systemstesting** | 2 scenarier | ✅ 100% |
| **Sikkerhetstesting** | 3 | ✅ 100% |
| **Brukervennlighetstesting** | 1 | ✅ 100% |
| **TOTALT** | **13 tester** | ✅ **100%** |

---

### **🎯 Test Konklusjon**

**Funksjonalitet:** ✅ Alle hovedfunksjoner fungerer som forventet  
**Sikkerhet:** ✅ Grunnleggende sikkerhetstiltak implementert  
**Brukervennlighet:** ✅ Responsivt design fungerer på mobil og desktop  
**Kompatibilitet:** ✅ Testet i Chrome, Firefox og Safari  

**Status:** ✅ **Applikasjonen fungerer som spesifisert**

---

## 🔒 Sikkerhet

### **Implementerte sikkerhetstiltak:**
- ✅ **ASP.NET Core Identity** for autentisering
- ✅ **PBKDF2** password hashing
- ✅ **Role-based authorization** (Pilot, Registerfører)
- ✅ **AntiForgeryToken** (CSRF-beskyttelse)
- ✅ **EF Core** parameteriserte queries (SQL injection-beskyttelse)
- ✅ **Razor** auto-encoding (XSS-beskyttelse)
- ✅ **HTTPS** enforcement (produksjon)
- ✅ **Input validation** (server + klient)

---

## 👥 Bidragsytere

**Gruppe 17 - Universitetet i Agder**  
**Emne:** IS-202 - Programmeringsprosjekt

---

## 📄 Lisens

Dette prosjektet er utviklet som en del av undervisningen ved Universitetet i Agder.

---

**Bygget med ❤️ av Gruppe 17 ved UiA**