using System.Text.Json.Serialization;
using ExchangeClick;
using ExchangeClick.Services;
using ExchangeClick.Services.Implementations;
using Microsoft.EntityFrameworkCore;
using System.Text;
using Microsoft.OpenApi.Models;
using Microsoft.IdentityModel.Tokens;
using ExchangeClick.Services.Interfaces;

var builder = WebApplication.CreateBuilder(args);

// Agrega servicios a contenedor de inyección de dependencia.
builder.Services.AddControllers().AddJsonOptions(options =>
{
    options.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles;
    options.JsonSerializerOptions.WriteIndented = true;
});
// Configura Entity Framework Core y registra el contexto de la base de datos
builder.Services.AddDbContext<ExchangeClickContext>(options =>
{
    options.UseSqlite("Data Source=exchangeclick.db");
});

// Configura Swagger/OpenAPI (si es necesario)
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddSwaggerGen(setupAction =>
{
    setupAction.AddSecurityDefinition("ExchangeClickBearerAuth", new OpenApiSecurityScheme() //Esto va a permitir usar swagger con el token.
    {
        Type = SecuritySchemeType.Http,
        Scheme = "Bearer",
        Description = "Acá pegar el token generado al loguearse."
    });

    setupAction.AddSecurityRequirement(new OpenApiSecurityRequirement
  {
    {
      new OpenApiSecurityScheme
      {
        Reference = new OpenApiReference
        {
          Type = ReferenceType.SecurityScheme,
          Id = "ExchangeClickBearerAuth" } //Tiene que coincidir con el id seteado arriba en la definición
        }, new List<string>() }
  });
});
builder.Configuration.AddJsonFile("appsettings.json");

builder.Services.AddAuthentication("Bearer") //"Bearer" es el tipo de auntenticación que tenemos que elegir después en PostMan para pasarle el token
.AddJwtBearer(options => //Acá definimos la configuración de la autenticación. le decimos qué cosas queremos comprobar. La fecha de expiración se valida por defecto.
{
    options.TokenValidationParameters = new()
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = builder.Configuration["Authentication:Issuer"],
        ValidAudience = builder.Configuration["Authentication:Audience"],
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(s: builder.Configuration["Authentication:SecretForKey"]))//pide la clave del appsettings.json
    };
}
);

#region DependencyInjections
builder.Services.AddScoped<CurrencyServices>();
builder.Services.AddScoped<SubscriptionServices>();
builder.Services.AddScoped<UserServices>();
#endregion

builder.Services.AddScoped<SubscriptionServices>();
var app = builder.Build();

app.UseCors(options => options
  .SetIsOriginAllowed(origin => origin == "http://localhost:4200")
  .AllowAnyHeader()
  .AllowAnyMethod());


// Configura el pipeline de solicitud HTTP.

// Habilita Swagger (solo en entorno de desarrollo)
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}


// Redirección HTTPS
app.UseHttpsRedirection();


// Configuración de autorización
app.UseAuthorization();

// Mapea los controladores
app.MapControllers();

// Inicia la aplicación
app.Run();
