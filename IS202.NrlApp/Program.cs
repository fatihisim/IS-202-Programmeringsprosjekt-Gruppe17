using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using IS202.NrlApp.Data;
using Pomelo.EntityFrameworkCore.MySql.Infrastructure;

var builder = WebApplication.CreateBuilder(args);

// --------------------------------------------------------
// Knytter applikasjonen til MariaDB-databasen
// --------------------------------------------------------
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseMySql(
        builder.Configuration.GetConnectionString("DefaultConnection"),
        new MySqlServerVersion(new Version(10, 6, 0)) // MariaDB-versjon
    ));

// --------------------------------------------------------
// Legger til Identity-tjenester for autentisering og autorisasjon
// --------------------------------------------------------
builder.Services.AddIdentity<IdentityUser, IdentityRole>(options =>
{
    // Konfigurasjon for passordpolitikk (for testing er kravene gjort enkle)
    options.Password.RequireDigit = false;
    options.Password.RequiredLength = 6;
    options.Password.RequireUppercase = false;
    options.Password.RequireNonAlphanumeric = false;
})
.AddEntityFrameworkStores<AppDbContext>()   // Bruker vår AppDbContext for lagring
.AddDefaultTokenProviders();                // Aktiverer støtte for tokens (f.eks. e-postbekreftelse)

// --------------------------------------------------------
// Legger til støtte for MVC (Controllers + Views)
// --------------------------------------------------------
builder.Services.AddControllersWithViews();

var app = builder.Build();

// --------------------------------------------------------
// Konfigurerer middleware og ruting for applikasjonen
// --------------------------------------------------------
if (!app.Environment.IsDevelopment())
{
    // Viser en feilmelding-side i produksjon
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts(); // Tvinger HTTPS for sikkerhet
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

// --------------------------------------------------------
// Aktiverer autentisering og autorisasjon i applikasjonen
// --------------------------------------------------------
app.UseAuthentication();  // Lar brukere logge inn
app.UseAuthorization();   // Styrer tilgang basert på roller/rettigheter

// --------------------------------------------------------
// Definerer standardruten for applikasjonen
// --------------------------------------------------------
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
