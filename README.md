# De Zachte Heelmeesters - Ctrl+C

> Dit project is een afsprakensysteem voor ziekenhuis "De Zachte Heelmeesters" met betrekking tot hun vraag en wensen
Om dit project te kunnen gebruiken moeten de volgende stappen ondernomen worden



## Inhoudsopgave

* [Setup Technical Environment](#setup-technical-environment)
    * [_Database_ Setup](#database-setup)
    * [_Backend_ Setup](#backend-setup)
    * [_Frontend_ Setup](#frontend-setup)



## Setup Technical Environment

### Database setup:

* 1. Start Visual Studio Code (VS Code) op
* 2. Installeer extentie "SQL Server (mssql)" _(bij "Extensions" Ctrl+Shift+X)_
* 3. Start Docker Desktop op
* 4. In VS Code, open het mapje `frontend\database` en open de file `DatabaseCreationScript.sql`
* 5. Run met het **groene** pijltje **rechts bovenin** de query
    * Als er om een container gevraagd wordt, kies dan `sql_server_container`
* 6. Nadat deze query geslaagd is, run nogmaals de query in dezelfde map
* 7. Klik links in VS Code naar het nieuwe blok "SQL Server" _(Ctrl+alt+D)_
* 8. Refresh de docker image door op het ronde pijltje te drukken die bij de image staat
* 9. Open de image en de `databases` map daarin
* 10. De database zou nu in docker moeten staan!

**Note:** De backend past automatisch migrations toe en seed testdata bij het opstarten (in Development mode).

### Backend setup:

* 1. Open een terminal in de `backend` map
* 2. Run `dotnet run` (gebruikt automatisch het https profile)
* 3. Backend draait nu op:
    * `https://localhost:7240` (primary)
    * `http://localhost:5016` (fallback)

**Belangrijke opmerking:** Gebruik altijd `dotnet run` zonder extra argumenten - het https profile is nu de default en werkt direct met de frontend.

### Frontend setup:

* 1. Open een terminal in de `frontend` map
* 2. Run `npm install` (eerste keer)
* 3. Run `npm run dev`
* 4. Frontend draait nu op `http://localhost:5173`

### Tests draaien:

```powershell
# Alle tests
dotnet test SeleniumTests/SeleniumTests.csproj

# Specifieke test suite (bijv. US 2.12)
dotnet test SeleniumTests/SeleniumTests.csproj --filter "FullyQualifiedName~US2_12"
```

**Belangrijk:** Backend en frontend moeten beiden draaien voordat je Selenium tests uitvoert.

## Test Accounts

Na het seeden zijn deze accounts beschikbaar:

**Patient:**
- Email: `gebruiker@example.com`
- Password: `Wachtwoord123`

**Doctor:**
- Email: `dokter@example.com`
- Password: `Wachtwoord123`

## Troubleshooting

### CORS Errors / Network Errors
**Probleem:** "CORS-aanvraag is niet gelukt" of "Network Error"

**Oplossing:** Zorg dat de backend draait. Run `dotnet run` in de backend map (https is nu default).

### Lege pagina in Selenium Tests
**Probleem:** Vue app laadt niet, lege pagina, of "504 Outdated Optimize Dep"

**Oplossing:** Herstart de frontend dev server:
```powershell
cd frontend
# Stop huidige server (Ctrl+C)
npm run dev
```

### Geen dossier entries in tests
**Probleem:** Test faalt met "No dossier entries found"

**Oplossing:** Medical data wordt automatisch geseed in Development mode. Check backend logs voor seeding bevestiging. 
# Fullstack-project-met-Docker

## Over dit project

Deze repository bevat een startpunt voor een fullstack-project met Docker:

- `Client/` — hier komt je Vue.js-project (frontend).
- `Server/` — hier komt je ASP.NET Core WebAPI (backend).
- Een MSSQL database draait ook als container.
- Een reverse proxy (nginx) routeert verkeer: de client is beschikbaar op poort 80 en de API onder het pad `/api` (ook op poort 80).


## Wat je moet weten / contract

- Inputs: vervang de placeholders tussen `{{}}` in de repository met jouw waarden.
- Output: na het bouwen via Docker Compose is de frontend bereikbaar op http://localhost en de API op http://localhost/api.
- Fouten/Edge-cases: controleer dat Docker Desktop draait, dat poort 80 vrij is en dat je sterk wachtwoord voor sa gebruikt.


## Alle placeholders die je moet vervangen

De volgende placeholders komen in de repository voor. Vervang ze allemaal met je eigen waarden.

- `{{Jouw WebAppFileNaam}}`
	- Bestand: `Server/Dockerfile`
	- Beschrijving: de naam van je ASP.NET projectbestand (zonder extensie). Bijvoorbeeld `MyWebApp` als je `MyWebApp.csproj` hebt.

- `{{Jouw groepsletter}}`
	- Bestanden: `.env`, `acceptatie.env`, `docker-compose.yml`
	- Beschrijving: de letter van jouw groep (bijv. `A`, `B`, etc.). Dit wordt gebruikt voor de database, domeinnamen en de `Team_name` variabele.

- `{{JouwConnectieNaam}}`
	- Bestanden: `Server/Program.cs`, `docker-compose.dev.yml`
	- Beschrijving: de naam van de connection string zoals gebruikt in je appsettings (bv. `DefaultConnection` of `AppDb`).

- `{{Jouw context hier}}`
	- Bestand: `Server/Program.cs`
	- Beschrijving: de naam van je DbContext klasse (bijv. `MyDbContext`).

- `{{Jouw wachtwoord}}`
	- Bestanden: `.env`, `acceptatie.env`, `docker-compose.dev.yml`
	- Beschrijving: sterk SA-wachtwoord voor de MSSQL container. Moet voldoen aan SQL Server password policy (minimaal 8 tekens, hoofdletter, kleine letter, cijfer en speciaal teken).

- `{{Jouwdatabasenaam}}`
	- Bestand: `docker-compose.dev.yml`
	- Beschrijving: de naam van de database die je wilt gebruiken/aanmaken (voor lokaal ontwikkelen).


Zorg dat je elk van bovenstaande placeholders vervangt voordat je de containers opstart.


## Stappen om lokaal te draaien (Windows PowerShell)

1. Installeer en start Docker Desktop (met WSL2 backend of Windows containers indien je dat nodig hebt).

2. Open een PowerShell venster en ga naar de projectmap (de map waar `docker-compose.yml` staat):

```powershell
cd "c:\Users\Martijn\Desktop\Fullstack-project-met-Docker"
```

3. Vervang alle `{{...}}` placeholders in de repository met jouw waarden. Belangrijke bestanden om aan te passen:

- `Server/Dockerfile` — zet hier de juiste projectnaam.
- `Server/Program.cs` — vul de juiste connection string naam en DbContext in.
- `.env` — vul `{{Jouw groepsletter}}` en `{{Jouw wachtwoord}}` in.
- `acceptatie.env` — vul `{{Jouw groepsletter}}` en `{{Jouw wachtwoord}}` in.
- `docker-compose.yml` — vul `{{Jouw groepsletter}}` in.
- `docker-compose.dev.yml` — vul `{{Jouw wachtwoord}}`, `{{Jouwdatabasenaam}}` en `{{JouwConnectieNaam}}` in.

Tip: gebruik een teksteditor of VS Code zoek-en-vervang om alle `{{` te vinden.

4. Bouw en start alles met Docker Compose:

```powershell
docker compose up -d --build
```

Dit bouwt de Docker images en start de containers in de achtergrond. Verwacht:

- Een `db` container (MSSQL)
- Een `server` container (ASP.NET Core)
- Een `client` container (Vue.js)
- Een `nginx` container (reverse proxy)

5. Controleer de logs (optioneel):

```powershell
docker compose logs -f
```

6. Open de applicatie in je browser:

- Frontend (Vue.js): http://localhost/
- Backend API: http://localhost/api


## Veel voorkomende problemen en oplossingen

- Poort 80 is al in gebruik: stop de service die poort 80 gebruikt (IIS, Apache, andere) of pas nginx-config aan naar een andere poort.
- MSSQL start niet door zwak wachtwoord: gebruik een sterk wachtwoord voor `{{Jouw wachtwoord}}`.
- Entity Framework migrations: als je migraties gebruikt, zorg dat de server container ze uitvoert bij start (of voer `dotnet ef database update` lokaal uit tegen de db container).


## Waar in de repository moet je kijken

- Frontend: `Client/` (hier komt je Vue project). Bouw je Vue-app zoals normaal (npm / yarn) of laat Docker dit doen volgens de aanwezige `Client/Dockerfile`.
- Backend: `Server/` (hier komt je ASP.NET Core WebAPI). Let op `Program.cs` waar de connection string wordt ingelezen via environment variables.
- Reverse proxy: `docker/nginx/default.conf` (routeert `/` naar de client en `/api` naar de server).


## Stoppen en opruimen

```powershell
docker compose down --volumes --remove-orphans
```

Dit stopt en verwijdert de containers, netwerken en volumes die door Compose zijn aangemaakt.