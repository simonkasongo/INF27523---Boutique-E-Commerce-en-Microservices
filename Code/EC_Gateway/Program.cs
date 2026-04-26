using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using MMLib.SwaggerForOcelot.DependencyInjection;
using Ocelot.DependencyInjection;
using Ocelot.Middleware;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// --- Chargement et fusion automatique des fichiers Ocelot ---
// AddOcelot(folder, env) scanne les fichiers ocelot.*.json du dossier indique
// et fusionne leurs sections "Routes" et "GlobalConfiguration" en un seul ocelot.json.
// On ajoute manuellement le fichier SwaggerEndPoints car il appartient au package
// MMLib.SwaggerForOcelot et n'est pas pris en charge par AddOcelot.
builder.Configuration
    .SetBasePath(builder.Environment.ContentRootPath)
    .AddOcelot("Routes", builder.Environment)
    .AddJsonFile("Routes/ocelot.SwaggerEndPoints.json", optional: false, reloadOnChange: true);

// --- JWT (pour protéger certaines routes via Ocelot si besoin) ---
var cleJwt = builder.Configuration["Jwt:Cle"]!;
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer("Bearer", options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = builder.Configuration["Jwt:Emetteur"],
            ValidAudience = builder.Configuration["Jwt:Audience"],
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(cleJwt))
        };
    });

// --- Ocelot + Swagger agrégé ---
// MMLib.SwaggerForOcelot exige que SwaggerGen soit enregistré au niveau du gateway
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddOcelot(builder.Configuration);
builder.Services.AddSwaggerForOcelot(builder.Configuration);

var app = builder.Build();

// Fichiers statiques (page de connexion) avant Ocelot : / et /index.html
app.UseDefaultFiles();
app.UseStaticFiles();

app.UseSwaggerForOcelotUI(options =>
{
    options.PathToSwaggerGenerator = "/swagger/docs";
});

app.UseAuthentication();
app.UseAuthorization();

await app.UseOcelot();

app.Run();
