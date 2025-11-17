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
// Kjører databasemigrasjoner automatisk ved oppstart
// Dette sikrer at databasen alltid er oppdatert med siste schema
// --------------------------------------------------------
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    var logger = services.GetRequiredService<ILogger<Program>>();
    
    try
    {
        var context = services.GetRequiredService<AppDbContext>();
        
        logger.LogInformation("Starter databasemigrering...");
        
        // Venter på at databasen er klar (viktig i Docker-miljø)
        var retryCount = 0;
        var maxRetries = 10;
        
        while (retryCount < maxRetries)
        {
            try
            {
                // Kjører alle pendende migrasjoner
                context.Database.Migrate();
                logger.LogInformation("Databasemigrering fullført.");
                break;
            }
            catch (Exception ex)
            {
                retryCount++;
                if (retryCount >= maxRetries)
                {
                    logger.LogError(ex, "Kunne ikke gjennomføre databasemigrering etter {RetryCount} forsøk.", maxRetries);
                    throw;
                }
                
                logger.LogWarning("Database er ikke klar ennå. Venter 2 sekunder... (Forsøk {RetryCount}/{MaxRetries})", retryCount, maxRetries);
                System.Threading.Thread.Sleep(2000); // Venter 2 sekunder
            }
        }
    }
    catch (Exception ex)
    {
        logger.LogError(ex, "En kritisk feil oppstod under databasemigrering.");
        // I produksjon bør applikasjonen stoppe hvis migrering feiler
        throw;
    }
}

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