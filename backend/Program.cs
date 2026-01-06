using backend.Data;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(
        builder.Configuration.GetConnectionString("DefaultConnection")));

// Add services to the container.
builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// App bouwen
var app = builder.Build();

// MIGRATIONS + SEEDING (NA Build, VOOR Run)
using (var scope = app.Services.CreateScope())
{
    var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();

    context.Database.Migrate(); // past migrations toe
    
    DbSeederStatic.Seed(context);     // seed data

    if (app.Environment.IsDevelopment())
    {
        DbSeederTest.Seed(context);    // alleen dev
    }
}

// Middleware pipeline
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

// CORS inschakelen
app.UseCors();

// Session middleware toevoegen
app.UseSession();

app.UseCors("frontend");
app.UseAuthorization();

app.MapControllers();

// 5️⃣ App starten
app.Run();
