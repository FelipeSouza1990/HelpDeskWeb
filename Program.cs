using HelpDesk.Data;
using HelpDesk.Middleware;
using HelpDesk.Models;
using HelpDesk.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

// Porta do Render (ou outros PaaS)
var port = Environment.GetEnvironmentVariable("PORT");
if (!string.IsNullOrEmpty(port))
{
    builder.WebHost.UseUrls($"http://0.0.0.0:{port}");
}

// Serilog
builder.Host.UseSerilog((ctx, lc) => lc.ReadFrom.Configuration(ctx.Configuration));

// DB (caminho absoluto)
var dbFile = builder.Configuration["Database:RelativePath"] ?? "helpdesk.db";
var dbPath = Path.Combine(builder.Environment.ContentRootPath, dbFile);
builder.Services.AddDbContext<AppDbContext>(opt => opt.UseSqlite($"Data Source={dbPath}"));

// MVC + Razor Pages (para Identity UI)
builder.Services.AddControllersWithViews();
builder.Services.AddRazorPages();

// Identity (login/cadastro prontos)
builder
    .Services.AddDefaultIdentity<IdentityUser>(o =>
    {
        o.SignIn.RequireConfirmedAccount = false;
        // ajuste de política de senha, se quiser
        // o.Password.RequiredLength = 6;
    })
    .AddEntityFrameworkStores<AppDbContext>();

// DI
builder.Services.AddScoped<IEstimationService, EstimationService>();
builder.Services.AddScoped<INotificationService, LoggingNotificationService>();

// Swagger
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Dev: erros detalhados
if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
}

// Swagger em Dev OU habilitado por config/env
var swaggerEnabled =
    app.Environment.IsDevelopment()
    || app.Configuration.GetValue<bool>("Swagger:Enable")
    || string.Equals(
        Environment.GetEnvironmentVariable("SWAGGER_ENABLE"),
        "true",
        StringComparison.OrdinalIgnoreCase
    );

app.Logger.LogInformation(
    "Swagger enabled? {Enabled} | Env: {Env}",
    swaggerEnabled,
    app.Environment.EnvironmentName
);

if (swaggerEnabled)
{
    app.UseSwagger();
    app.UseSwaggerUI(o =>
    {
        o.SwaggerEndpoint("/swagger/v1/swagger.json", "HelpDesk API v1");
        // o.RoutePrefix = string.Empty; // opcional: swagger na raiz "/"
    });
}

// MIGRATE antes do seed
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    app.Logger.LogInformation("SQLite file: {Path}", db.Database.GetDbConnection().DataSource);

    db.Database.Migrate();

    if (!db.Tickets.Any())
    {
        db.Tickets.AddRange(
            new Ticket
            {
                Title = "Bug no login",
                CustomerEmail = "cli@example.com",
                Description = "Erro 500 ao logar",
                Severity = TicketSeverity.High,
            },
            new Ticket
            {
                Title = "UI quebra no Safari",
                CustomerEmail = "ana@example.com",
                Description = "Layout desalinhado",
                Severity = TicketSeverity.Medium,
            }
        );
        db.SaveChanges();
    }
}

app.UseMiddleware<ErrorHandlingMiddleware>();

app.UseStaticFiles();
app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllerRoute(name: "default", pattern: "{controller=Tickets}/{action=Index}/{id?}");

app.MapRazorPages();

app.MapGet("/health", () => Results.Ok(new { status = "ok", ts = DateTime.UtcNow }));

// /diag para conferir DB (SEM vírgula sobrando!)
app.MapGet(
    "/diag",
    async (AppDbContext db) =>
    {
        var path = db.Database.GetDbConnection().DataSource;
        var ok = await db.Database.CanConnectAsync();

        var tables = new List<string>();
        await using (var conn = db.Database.GetDbConnection())
        {
            await conn.OpenAsync();
            await using var cmd = conn.CreateCommand();
            cmd.CommandText = "SELECT name FROM sqlite_master WHERE type='table' ORDER BY name";
            await using var rdr = await cmd.ExecuteReaderAsync();
            while (await rdr.ReadAsync())
                tables.Add(rdr.GetString(0));
        }

        return Results.Ok(
            new
            {
                path,
                canConnect = ok,
                tables,
            }
        );
    }
);

app.Run();
