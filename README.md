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