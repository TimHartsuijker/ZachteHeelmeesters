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

// 3. CORS configuratie (één duidelijke policy genaamd "frontend")
builder.Services.AddCors(options =>
{
    options.AddPolicy("frontend", policy =>
    {
        policy.WithOrigins("http://localhost:5173", "http://localhost:5174", "http://localhost:3000", "http://localhost:5016")
              .AllowAnyMethod()
              .AllowAnyHeader()
              .AllowCredentials();
    });
});

// 4. Session & Cache services
builder.Services.AddDistributedMemoryCache();
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});

var app = builder.Build();

// 5. MIGRATIONS + SEEDING (NA Build, VOOR Run)
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    try
    {
        var context = services.GetRequiredService<AppDbContext>();

        // Voer migraties uit
        context.Database.Migrate();

        // Seed basis data (Rollen, etc.)
        DbSeederStatic.Seed(context);

        // Seed test data alleen in Development mode
        if (app.Environment.IsDevelopment())
        {
            DbSeederTest.Seed(context);
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

app.UseAuthentication(); // Eerst wie ben je
app.UseAuthorization();  // Dan wat mag je

app.MapControllers();

app.Run();