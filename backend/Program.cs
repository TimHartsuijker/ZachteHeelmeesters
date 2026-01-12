using backend.Data;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(
        builder.Configuration.GetConnectionString("DefaultConnection")));

<<<<<<< HEAD
// Add CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowFrontend", policy =>
    {
        policy.WithOrigins("http://localhost:5173", "http://localhost:3000")
              .AllowAnyHeader()
              .AllowAnyMethod();
    });
});

=======
// Add services to the container.
>>>>>>> feat/inlog
builder.Services.AddControllers();

// CORS toevoegen zodat frontend requests werkt
builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(policy =>
    {
        policy.WithOrigins("http://localhost:5173") // je frontend poort
              .AllowAnyHeader()
              .AllowAnyMethod();
    });
});

// Sessions inschakelen (voor login sessie tracking)
builder.Services.AddDistributedMemoryCache();
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30); // sessie timeout
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});

// Learn more about configuring Swagger/OpenAPI
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
        DbSeederMedicalFiles.Seed(context); // seed medical files for testing
    }
}

// Middleware pipeline
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseCors("AllowFrontend");

app.UseHttpsRedirection();

// CORS inschakelen
app.UseCors();

// Session middleware toevoegen
app.UseSession();

app.UseAuthorization();

app.MapControllers();

// 5️⃣ App starten
app.Run();
