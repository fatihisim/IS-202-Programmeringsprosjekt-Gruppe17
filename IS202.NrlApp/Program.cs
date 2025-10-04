// Program.cs
// Inneholder hovedinngangen til applikasjonen.
// Konfigurerer webserver, tjenester og routing (MVC).
// Oppretter database ved behov og starter applikasjonen.

using IS202.NrlApp.Data;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Legger til støtte for MVC (Model-View-Controller)
builder.Services.AddControllersWithViews();

// Konfigurerer Entity Framework Core med SQLite som database
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlite(builder.Configuration.GetConnectionString("DefaultConnection")));

var app = builder.Build();

// Sikrer at databasen blir opprettet hvis den ikke finnes
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    db.Database.EnsureCreated();
}

// Konfigurerer middleware-pipeline
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

// Aktiverer HTTPS og statiske filer
app.UseHttpsRedirection();
app.UseStaticFiles();

// Konfigurerer routing og autorisasjon
app.UseRouting();
app.UseAuthorization();

// Definerer standardrute for applikasjonen
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

// Starter webapplikasjonen
app.Run();
