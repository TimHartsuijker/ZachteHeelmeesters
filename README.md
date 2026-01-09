om dit project te kunnen gebruiken moeten de volgende stappen gedaan worden:

### Databse setup:



* start vs code op,
* installeer de vs code extentie "SQL Server (mssql)",
* start docker desktop op,
* in vs code, open het mapje "database" en open de file "DatabaseCreationScript.sql",
* run met het groene pijltje rechts bovenin de query,
* in hetzelfde mapje doe je hetzelfde nadat de vorige query geslaagd is in de file "database.sql"
* ga links in vs code naar de nieuwe blok "SQL Server" (ctrl+alt+D),
* refresh de docker image door op het ronde pijltje te drukken die bij de image staat,
* open de image en de "databases" map daarin,
* de database zou nu in docker moeten staan!

### Migration draaien (beschikbaarheidskalender)

* Zorg dat de SQL container draait en luistert op poort 1433.
* Run het migratiescript voor de doctor_availability tabel:
	* `sqlcmd -S localhost,1433 -U sa -P "<SA_PASSWORD>" -d zachteheelmeester -i "frontend/database/4.20-migration.sql"`
* (Optioneel) Seeding testdokter + weekslots staat in `frontend/database/database.sql`; rerun alleen het laatste blok indien nodig.

### Backend starten

* Ga naar `backend/`
* Zorg dat de connection string in `appsettings.Development.json` wijst naar je Docker SQL (nu: `Server=localhost,1433;Database=zachteheelmeester;User Id=sa;Password=gmQ0veEXZ1uOJqG;TrustServerCertificate=True`)
* Restore en start: `dotnet restore` daarna `dotnet run --urls http://localhost:5016`
* API draait op `http://localhost:5016` (zie `launchSettings.json`)

### Frontend starten

* Ga naar `frontend/`
* Installeer dependencies: `npm install`
* Start dev server: `npm run dev`
* Frontend verwacht de API op `http://localhost:5016` (pas `apiBaseUrl` prop/setting aan indien anders).
