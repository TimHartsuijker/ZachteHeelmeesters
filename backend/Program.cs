var builder = WebApplication.CreateBuilder(args);

// Add services to the container
builder.Services.AddControllers();

// CORS configuratie
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowFrontend", policy =>
    {
        policy.WithOrigins("http://localhost:5174", "https://localhost:5174")
              .AllowAnyHeader()
              .AllowAnyMethod()
              .AllowCredentials(); // Voor cookies/JWT tokens (later bij authenticatie)
    });
});

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

// Middleware in correcte volgorde - BELANGRIJK!
app.UseCors("AllowFrontend");  // 1. CORS eerst

app.UseHttpsRedirection();      // 2. HTTPS redirect

app.UseAuthorization();         // 3. Authorization

app.MapControllers();           // 4. Controllers mappen

app.Run();
