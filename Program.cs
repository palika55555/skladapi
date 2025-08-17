using Microsoft.EntityFrameworkCore;
using SkladApi.Data;

var builder = WebApplication.CreateBuilder(args);

// ✅ CORS POLICY pre Angular z mobilu
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAngularDev", policy =>
    {
        policy.WithOrigins(
            "https://skladproblock-app.web.app/",
            "http://192.168.240.107:4200",
            "http://172.20.10.4:4200",
            "http://192.168.0.118:4200",
            "http://192.168.240.105:4200")
              .AllowAnyHeader()
              .AllowAnyMethod();
    });
});

builder.Services.AddControllers();

// ✅ Tu pridaj DBContext ak ho máš
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseMySql(builder.Configuration.GetConnectionString("DefaultConnection"),
    new MySqlServerVersion(new Version(8, 0, 29))));

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// ✅ Spustenie servera na všetkých IP adresách
builder.WebHost.UseUrls("http://0.0.0.0:5179");

var app = builder.Build();

// ✅ CORS musí byť zapnutý pred MapControllers
app.UseCors("AllowAngularDev");

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

// TU PRIDÁVAME podávanie statických súborov a fallback pre Angular

app.UseDefaultFiles();  // umožní načítanie index.html ako predvolený súbor
app.UseStaticFiles();   // podáva súbory z wwwroot

app.UseAuthorization();

app.MapControllers();

// fallback na index.html pre Angular routovanie
app.MapFallbackToFile("index.html");

app.Run();
