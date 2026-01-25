using backend.Data;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// 1. Database Context registreren
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// 2. Controllers en API tools
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// 4. Session & Cache services
builder.Services.AddDistributedMemoryCache();
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});

// Add CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("frontend", builder =>
    {
        builder.WithOrigins(
                "http://localhost",
                "http://localhost:5173",
                "http://127.0.0.1:5173")
               .AllowAnyMethod();
    });
});

// App bouwen
var app = builder.Build();

// 5. MIGRATIONS + SEEDING (NA Build, VOOR Run)
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    try
    {
        var context = services.GetRequiredService<AppDbContext>();

        // Voer migraties uit

        // Seed basis data (Rollen, etc.)
        DbSeederStatic.Seed(context);

        // Seed test data alleen in Development mode
        if (app.Environment.IsDevelopment())
        {
            DbSeederTest.Seed(context);
            DbSeederMedicalFiles.Seed(context);
        }
    }
    catch (Exception ex)
    {
        Console.WriteLine($"FOUT BIJ MIGRATIE/SEEDING: {ex.Message}");
    }
}

// 6. Middleware Pipeline
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

// BELANGRIJK: De volgorde van middleware is essentieel!
app.UseCors("frontend");
app.UseSession();

app.UseAuthorization();

app.UseAuthorization();

app.MapControllers();

app.Run();