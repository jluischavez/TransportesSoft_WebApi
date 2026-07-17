using Microsoft.EntityFrameworkCore;
using TransportesSoft_WebApi.Data;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// Base de datos.
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(
        builder.Configuration.GetConnectionString("DefaultConnection")
    )
);

builder.Services.AddControllers();

// Swagger
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("TransportesSoftFrontend", policy =>
    {
        var origins = new List<string>
        {
            // Frontend publicado en GitHub Pages
            "https://jluischavez.github.io"
        };

        // Estos orígenes solamente se permiten al ejecutar localmente
        if (builder.Environment.IsDevelopment())
        {
            origins.Add("http://localhost:5500");
            origins.Add("https://localhost:5500");
            origins.Add("http://127.0.0.1:5500");
            origins.Add("https://127.0.0.1:5500");
        }

        policy
            .WithOrigins(origins.ToArray())
            .AllowAnyHeader()
            .AllowAnyMethod();
    });
});

// Autenticación JWT
builder.Services
    .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        var jwtKey = builder.Configuration["Jwt:Key"];

        if (string.IsNullOrWhiteSpace(jwtKey))
        {
            throw new InvalidOperationException(
                "No se encontró la configuración Jwt:Key."
            );
        }

        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,

            ValidIssuer = builder.Configuration["Jwt:Issuer"],
            ValidAudience = builder.Configuration["Jwt:Audience"],

            IssuerSigningKey = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(jwtKey)
            ),

            // Evita que un token vencido siga siendo válido
            // durante los 5 minutos de tolerancia predeterminados.
            ClockSkew = TimeSpan.Zero
        };
    });

var app = builder.Build();

// Swagger solamente estará disponible en Development.
// En Azure, si el ambiente es Production, /swagger devolverá 404.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

// CORS debe ejecutarse antes de autenticación y autorización.
app.UseCors("TransportesSoftFrontend");

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
