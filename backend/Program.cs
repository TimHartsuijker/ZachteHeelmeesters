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

// Add session services
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
        builder.WithOrigins("http://localhost:5173", "http://localhost:5174", "http://localhost:3000", "http://localhost:5016")
               .AllowAnyMethod()
               .AllowAnyHeader()
               .AllowCredentials();
    });
});

// App bouwen
var app = builder.Build();

// MIGRATIONS + SEEDING (NA Build, VOOR Run)
try
{
    using (var scope = app.Services.CreateScope())
    {
        var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        context.Database.Migrate();
        DbSeederStatic.Seed(context);
        if (app.Environment.IsDevelopment())
        {
            DbSeederTest.Seed(context);
        }
    }
}
catch (Exception ex)
{
    Console.WriteLine($"FOUT BIJ MIGRATIE: {ex.Message}");
    // De app start nu in ieder geval wel op, ook als de db faalt
}

// Middleware pipeline
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

// Apply CORS policy FIRST before any other middleware
app.UseCors("frontend");

app.UseHttpsRedirection();

// Session middleware toevoegen
app.UseSession();

app.UseAuthorization();

app.MapControllers();

// 5️⃣ App starten
app.Run();
