using AppOdontologia.Services;
using MailKit;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// ====================== CONFIGURACIÓN DE CORREO ======================
builder.Services.Configure<MailSettings>(
    builder.Configuration.GetSection("MailSettings"));
builder.Services.AddTransient<MailService>();

// ====================== CORS ======================
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAngular", policy =>
        policy.WithOrigins(
            "http://localhost:4200",
            "https://clinicasalegria.vercel.app"
        )
        .AllowAnyHeader()
        .AllowAnyMethod()
    );
});

// ====================== CONTROLADORES & SWAGGER ======================
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// ====================== JWT ======================
builder.Services.AddScoped<JwtService>();
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = builder.Configuration["Jwt:Issuer"],
            ValidAudience = builder.Configuration["Jwt:Audience"],
            IssuerSigningKey = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"]
                    ?? throw new InvalidOperationException("JWT Key is not configured."))
            )
        };
    });

// ====================== REPOSITORIOS ======================
builder.Services.AddScoped<IAseguradorasRepo, AseguradorasRepo>();
builder.Services.AddScoped<CalidadRepo>();
builder.Services.AddScoped<CanalConfirmacionRepo>();
builder.Services.AddScoped<CitaRepo>();
builder.Services.AddScoped<ConsultorioRepo>();
builder.Services.AddScoped<EspecialidadRepo>();
builder.Services.AddScoped<EstadoCitaRepo>();
builder.Services.AddScoped<EstadoCivilRepo>();
builder.Services.AddScoped<GeneroRepo>();
builder.Services.AddScoped<MaterialRepo>();
builder.Services.AddScoped<MedicamentoCatalogoRepo>();
builder.Services.AddScoped<MetodoPagoRepo>();
builder.Services.AddScoped<MotivoCitaRepo>();
builder.Services.AddScoped<OrigenPacienteRepo>();
builder.Services.AddScoped<PadecimientoCatalogoRepo>();
builder.Services.AddScoped<PiezaDentalRepo>();
builder.Services.AddScoped<PreferenciaCatalogoRepo>();
builder.Services.AddScoped<ProcedimientoRepo>();
builder.Services.AddScoped<ResultadoConfirmacionRepo>();
builder.Services.AddScoped<RolRepo>();
builder.Services.AddScoped<SedeRepo>();
builder.Services.AddScoped<TipoSangreRepo>();
builder.Services.AddScoped<OdontologoRepo>();
builder.Services.AddScoped<MaterialEsperaRepo>();
builder.Services.AddScoped<PacienteRepo>();
builder.Services.AddScoped<UsuarioRepo>();
builder.Services.AddScoped<PacienteAlergiaMedRepo>();
builder.Services.AddScoped<PacienteMedicamentoActualRepo>();
builder.Services.AddScoped<PacientePadecimientoRepo>();
builder.Services.AddScoped<PacientePreferenciaRepo>();
builder.Services.AddScoped<PacienteSeguroRepo>();
builder.Services.AddScoped<TratamientoRepo>();
builder.Services.AddScoped<PlanTratamientoRepo>();
builder.Services.AddScoped<RolesPermisosRepo>();
builder.Services.AddScoped<PermisosRepo>();

var app = builder.Build();

// ====================== MIDDLEWARE ======================
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseCors("AllowAngular");
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();
app.Run();