# ZachteHeelmeesters
## Vereisten

- [Node.js](https://nodejs.org/) (v16 of hoger)
- [.NET 8 SDK](https://dotnet.microsoft.com/download)
- [Docker Desktop](https://www.docker.com/products/docker-desktop/)
- VS Code met extensie **SQL Server (mssql)**

## Installatie
### 1. Database Setup

**Stap 1: Docker starten**
- Start Docker Desktop op

**Stap 2: SQL Server container starten**
Open een terminal en voer uit:
```powershell
docker run -e "ACCEPT_EULA=Y" -e "SA_PASSWORD=gmQ0veEXZ1uOJqG" -p 1433:1433 -d mcr.microsoft.com/mssql/server:2022-latest
```

**Stap 3: Database aanmaken**
- Installeer de VS Code extensie **SQL Server (mssql)** als je dit nog niet hebt
- Open het mapje `frontend/database/`
- Open de file `DatabaseCreationScript.sql`
- Run de query met het groene pijltje rechtsboven (of druk op `Ctrl+Shift+E`)
- Open vervolgens `database.sql` in dezelfde map
- Run deze query ook met het groene pijltje

**Stap 4: Verificatie**
- Ga in VS Code naar het **SQL Server** paneel (druk op `Ctrl+Alt+D`)
- Refresh de Docker container door op het ronde pijltje te klikken
- Open de container en de **Databases** map
- De database `zachteheelmeester` zou nu zichtbaar moeten zijn!

### 2. Backend Setup (.NET)

Open een terminal en navigeer naar de backend folder:

```powershell
cd backend
dotnet restore
dotnet run
```

De backend draait nu op: **http://localhost:5016**

Je kunt Swagger gebruiken om de API te testen: **http://localhost:5016/swagger**

### 3. Frontend Setup (Vue.js)

Open een **nieuwe** terminal en navigeer naar de frontend folder:

```powershell
cd frontend
npm install
npm run dev
```

De frontend is nu beschikbaar op: **http://localhost:5173** (of een andere poort die getoond wordt in de terminal)

## Project Draaien

Elke keer als je aan het project wilt werken:

1. **Start Docker Desktop**
2. **Start de backend:**
   ```powershell
   cd backend
   dotnet run
   ```
3. **Start de frontend** (in een aparte terminal):
   ```powershell
   cd frontend
   npm run dev
   ```

## Troubleshooting

### Database verbinding mislukt
- Controleer of Docker Desktop draait
- Controleer of de SQL Server container actief is: `docker ps`
- Als de container niet draait, start hem opnieuw met het docker run commando hierboven

### Backend kan niet starten
- Zorg dat poort 5016 niet al in gebruik is
- Controleer of .NET 8 SDK geïnstalleerd is: `dotnet --version`

### Frontend kan niet connecten met backend
- Zorg dat de backend draait op http://localhost:5016
- Check de browser console voor foutmeldingen (F12)
