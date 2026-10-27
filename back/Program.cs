using System.Text;
using DotNetEnv;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Runpd.Api.Features.Autenticacion;
using Runpd.Api.Features.Perfil;
using Runpd.Api.Features.Roles;
using Runpd.Api.Features.Usuarios;
using Runpd.Api.Infrastructure.Persistence;

var environmentName = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT");
if (string.Equals(environmentName, "Development", StringComparison.OrdinalIgnoreCase))
{
    var environmentFilePath = Path.Combine(Directory.GetCurrentDirectory(), ".env");
    if (File.Exists(environmentFilePath))
    {
        Env.NoClobber().Load(environmentFilePath);
    }
}

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddOpenApi();

var jwtSecret = builder.Configuration["JWT_SECRET"];
if (string.IsNullOrWhiteSpace(jwtSecret) || Encoding.UTF8.GetByteCount(jwtSecret) < 32)
{
    throw new InvalidOperationException(
        "Configura JWT_SECRET con al menos 32 bytes en el archivo .env local o en el entorno de ejecución.");
}

const string jwtIssuer = "Runpd.Api";
const string jwtAudience = "Runpd.Api";
const int jwtExpirationMinutes = 60;
var jwtKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSecret));

builder.Services.AddSingleton(new JwtSettings(
    jwtSecret,
    jwtIssuer,
    jwtAudience,
    jwtExpirationMinutes));
builder.Services
    .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.MapInboundClaims = false;
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = jwtKey,
            ValidateIssuer = true,
            ValidIssuer = jwtIssuer,
            ValidateAudience = true,
            ValidAudience = jwtAudience,
            ValidateLifetime = true,
            ClockSkew = TimeSpan.FromSeconds(30),
            NameClaimType = "sub",
            RoleClaimType = "role"
        };
    });
builder.Services.AddAuthorization();

var databaseHost = builder.Configuration["DB_HOST"];
var databasePortValue = builder.Configuration["DB_PORT"];
var databaseName = builder.Configuration["DB_NAME"];
var databaseUser = builder.Configuration["DB_USER"];
var databasePassword = builder.Configuration["DB_PASSWORD"];

if (string.IsNullOrWhiteSpace(databaseHost)
    || string.IsNullOrWhiteSpace(databaseName)
    || string.IsNullOrWhiteSpace(databaseUser)
    || string.IsNullOrWhiteSpace(databasePassword))
{
    throw new InvalidOperationException(
        "Configura DB_HOST, DB_NAME, DB_USER y DB_PASSWORD en el archivo .env local o en el entorno de ejecución.");
}

if (!int.TryParse(databasePortValue, out var databasePort) || databasePort is < 1 or > 65535)
{
    throw new InvalidOperationException("DB_PORT debe ser un puerto válido entre 1 y 65535.");
}

var connectionString = new SqlConnectionStringBuilder
{
    DataSource = $"{databaseHost},{databasePort}",
    InitialCatalog = databaseName,
    UserID = databaseUser,
    Password = databasePassword,
    Encrypt = true,
    TrustServerCertificate = builder.Configuration.GetValue<bool>("DB_TRUST_SERVER_CERTIFICATE")
}.ConnectionString;

builder.Services.AddDbContext<RunpdDbContext>(options => options.UseSqlServer(connectionString));
builder.Services.AddScoped<IPasswordHasher<Usuario>, PasswordHasher<Usuario>>();
builder.Services.AddScoped<AutenticacionService>();
builder.Services.AddSingleton<JwtTokenService>();
builder.Services.AddScoped<RolesService>();
builder.Services.AddScoped<UsuariosService>();
builder.Services.AddScoped<PerfilService>();

var app = builder.Build();

await using (var scope = app.Services.CreateAsyncScope())
{
    var dbContext = scope.ServiceProvider.GetRequiredService<RunpdDbContext>();

    if (app.Environment.IsDevelopment())
    {
        await dbContext.Database.MigrateAsync();
    }

    await DatabaseSeeder.SeedAsync(
        dbContext,
        scope.ServiceProvider.GetRequiredService<IPasswordHasher<Usuario>>(),
        app.Configuration,
        app.Logger);
}

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
