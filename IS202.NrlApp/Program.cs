using Microsoft.EntityFrameworkCore;
using IS202.NrlApp.Data;
using Pomelo.EntityFrameworkCore.MySql.Infrastructure; // Driver for MariaDB

var builder = WebApplication.CreateBuilder(args);

// Knytter applikasjonen til MariaDB-databasen
// Tilkoblingsstrengen hentes fra appsettings.json
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseMySql(
        builder.Configuration.GetConnectionString("DefaultConnection"),
        new MySqlServerVersion(new Version(10, 6, 0)) // MariaDB-versjon
    ));

// Legger til MVC (Controller + View) støtte
builder.Services.AddControllersWithViews();

var app = builder.Build();

// Konfigurerer middleware og ruting for applikasjonen
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts(); // Tvinger HTTPS
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization(); // Aktiverer tilgangskontroll (Identity senere)

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
