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