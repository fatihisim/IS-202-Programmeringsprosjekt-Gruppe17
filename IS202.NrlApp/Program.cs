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

// --------------------------------------------------------
// HTTP Sikkerhets-headere (Security Headers)
// Beskytter mot vanlige webangrepsvektorer
// --------------------------------------------------------
app.Use(async (context, next) =>
{
    // X-Content-Type-Options: Forhindrer MIME-type sniffing
    // Nettleseren vil ikke gjette innholdstypen, noe som kan forhindre XSS-angrep
    context.Response.Headers.Append("X-Content-Type-Options", "nosniff");
    
    // X-Frame-Options: Beskytter mot clickjacking-angrep
    // DENY = siden kan ikke vises i en iframe i det hele tatt
    // SAMEORIGIN = siden kan kun vises i iframe fra samme domene
    context.Response.Headers.Append("X-Frame-Options", "DENY");
    
    // X-XSS-Protection: Aktiverer nettleserens innebygde XSS-filter
    // 1; mode=block = blokkerer siden helt hvis et XSS-angrep oppdages
    context.Response.Headers.Append("X-XSS-Protection", "1; mode=block");
    
    // Referrer-Policy: Kontrollerer hvor mye referrer-informasjon som sendes
    // strict-origin-when-cross-origin = full URL til samme origin, kun origin til andre
    context.Response.Headers.Append("Referrer-Policy", "strict-origin-when-cross-origin");
    
    // Permissions-Policy: Begrenser tilgang til nettleser-APIer
    // Deaktiverer kamera, mikrofon, geolokasjon osv. med mindre eksplisitt tillatt
    context.Response.Headers.Append("Permissions-Policy", "camera=(), microphone=(), geolocation=(self)");
    
    // Content-Security-Policy: Definerer hvilke ressurser som kan lastes
    // Dette er den viktigste headeren for å forhindre XSS-angrep
    context.Response.Headers.Append("Content-Security-Policy", 
        "default-src 'self'; " +                                    // Standard: kun fra eget domene
        "script-src 'self' 'unsafe-inline' 'unsafe-eval' " +        // JavaScript kilder
            "https://unpkg.com " +                                  // Leaflet.js
            "https://cdnjs.cloudflare.com " +                       // Leaflet plugins
            "https://cdn.jsdelivr.net; " +                          // Bootstrap/andre CDN
        "style-src 'self' 'unsafe-inline' " +                       // CSS kilder
            "https://unpkg.com " +                                  // Leaflet CSS
            "https://cdnjs.cloudflare.com " +                       // Leaflet plugins CSS
            "https://cdn.jsdelivr.net; " +                          // Bootstrap CSS
        "img-src 'self' data: blob: " +                             // Bilder
            "https://*.tile.openstreetmap.org " +                   // OpenStreetMap tiles
            "https://server.arcgisonline.com " +                    // Esri satellite tiles
            "https://*.arcgisonline.com; " +                        // Esri andre tiles
        "font-src 'self' " +                                        // Skrifttyper
            "https://cdn.jsdelivr.net " +                           // Bootstrap Icons
            "https://cdnjs.cloudflare.com; " +                      // Andre fonter
        "connect-src 'self'; " +                                    // AJAX/WebSocket
        "frame-ancestors 'none'; " +                                // Ingen iframe embedding
        "form-action 'self'; " +                                    // Skjemaer kun til eget domene
        "base-uri 'self'");                                         // Base URL begrensning
    
    // Strict-Transport-Security: Tvinger HTTPS for fremtidige forespørsler
    // max-age=31536000 = 1 år, includeSubDomains = gjelder også subdomener
    // Kun aktiver i produksjon (når HTTPS er konfigurert)
    if (!context.RequestServices.GetRequiredService<IWebHostEnvironment>().IsDevelopment())
    {
        context.Response.Headers.Append("Strict-Transport-Security", "max-age=31536000; includeSubDomains");
    }
    
    await next();
});

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