using ACTI_AssetManager_Project.Application.Data;
using ACTI_AssetManager_Project.Application.Interfaces;
using ACTI_AssetManager_Project.Application.Services;
using ACTI_AssetManager_Project.Domain.Interfaces;
using ACTI_AssetManager_Project.Infrastructure.Repository;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;

//CUANDO ESCRIBIMOS ESTE CODIGO DE ABAJO
// LO QUE ESTAMOS HACIENDO ES UN OBJETO DE CONFIGURACIÓN DE LA APLICACIÓN
//ESTO CONTIENE, CONFIGURACIÓN, SERVICIOS, ENTORNO Y LOGGING
//ES BASICAMENTE EL PUNTO DE ENTRADA PARA CONFIGURAR NUESTRA APLICACIÓN ANTES DE EJECUTARLA

System.IdentityModel.Tokens.Jwt.JwtSecurityTokenHandler.DefaultInboundClaimTypeMap.Clear();

//WebApplication es una clase
var builder = WebApplication.CreateBuilder(args);

//DEBEMOS DE ENTENDER LO SIGUIENTE
//builder.Services : ES UNA COLECCIÓN DE SERVICIOS QUE SE PUEDEN INYECTAR EN TODA LA APLICACIÓN
//builder.Configuration : ES UNA INTERFAZ PARA ACCEDER A LA CONFIGURACIÓN DE LA APLICACIÓN, COMO ARCHIVOS DE CONFIGURACIÓN, VARIABLES DE ENTORNO, ETC
//builder.Environment : PROPORCIONA INFORMACIÓN SOBRE EL ENTORNO DE EJECUCIÓN ACTUAL, COMO SI ESTAMOS EN DESARROLLO, PRODUCCIÓN, ETC

var jwtKey = builder.Configuration["JwtSettings:Key"];

if(string.IsNullOrEmpty(jwtKey))
{
    throw new Exception("JWT_KEY no esta configurada.");
}

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultSignInScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.TokenValidationParameters = new Microsoft.IdentityModel.Tokens.TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = builder.Configuration["JwtSettings:Issuer"], 
        ValidAudience = builder.Configuration["JwtSettings:Audience"],
        IssuerSigningKey = new Microsoft.IdentityModel.Tokens.SymmetricSecurityKey(System.Text.Encoding.UTF8.GetBytes(jwtKey)),
        ClockSkew = TimeSpan.Zero,
        NameClaimType = System.Security.Claims.ClaimTypes.Name,
        RoleClaimType = System.Security.Claims.ClaimTypes.Name

    };

    options.Events = new JwtBearerEvents
    {
        OnMessageReceived = context =>
        {
            // Lee la cookie que definiste en tu AuthController
            context.Token = context.Request.Cookies["AuthToken"];
            return Task.CompletedTask;
        },OnChallenge = context => 
        {
            if (!context.Request.Path.StartsWithSegments("/Auth")) 
            { 
            context.HandleResponse();
            context.Response.Redirect("/Auth/Index");
            }
            return Task.CompletedTask;


        }
    };


});

builder.Services.AddDbContext<AM_DBContext>(options =>
options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddHttpContextAccessor();

//CONEXIÓN PARA EL SERVICIO DE AUTENTICACIÓN
builder.Services.AddScoped<IAuthService,AuthService>();

//CONEXIÓN PARA EL SERVICIO DE RECURSOS
builder.Services.AddScoped<IRecursoService, RecursoService>();

//CONEXIÓN PARA EL SERVICIO DE LICENCIA
builder.Services.AddScoped<ILicenciaService, LicenciaService>();

builder.Services.AddScoped<ILicenciaRepository, LicenciaRepository>();

//CONEXIÓN PARA EL SERVICIO DE RESPONSABLE RECURSOS
builder.Services.AddScoped<IResponsableRecursoService, ResponsableRecursoService>();

//CONEXIÓN PARA EL REPOSITORY DE RESPONSABLE RECURSOS
builder.Services.AddScoped<IResponsableRecursoRepository, ResponsableRecursoRepository>();

//CONEXIÓN PARA EL REPOSITORY DE RECURSOS
builder.Services.AddScoped<IRecursoRepository, RecursoRepository>();

//CONEXIÓN PARA EL REPOSITORY DE ASIGNACION DE PROYECTOS
builder.Services.AddScoped<IAsignacionRepository, AsignacionRepository>();

//CONEXIÓN PARA EL SERVICIO DE ASIGNACION DE PROYECTOS
builder.Services.AddScoped<IAsignacionService, AsignacionService>();

//CONEXIÓN PARA EL REPOSITORY DE PROYECTOS
builder.Services.AddScoped<IProyectoRepository, ProyectoRepository>();

//CONEXIÓN PARA EL SERVICIO DE PROYECTOS
builder.Services.AddScoped<IProyectoService, ProyectoService>();

//CONEXIÓN PARA EL REPOSITORY DE TIPO RECURSOS
builder.Services.AddScoped<ITipoRecursoRepository, TipoRecursoRepository>();


// Add services to the container.
builder.Services.AddControllersWithViews(options =>
{
    options.ModelBindingMessageProvider.SetValueMustNotBeNullAccessor(
        _ => "Este campo es obligatorio.");
    options.ModelBindingMessageProvider.SetAttemptedValueIsInvalidAccessor(
        (_, _) => "El valor ingresado no es válido.");
    options.ModelBindingMessageProvider.SetValueIsInvalidAccessor(
        _ => "El valor ingresado no es válido.");
    options.ModelBindingMessageProvider.SetMissingBindRequiredValueAccessor(
        _ => "Este campo es obligatorio.");
    options.ModelBindingMessageProvider.SetValueMustBeANumberAccessor(
        _ => "Ingresa un número válido.");
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication();

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Auth}/{action=Index}/{id?}");

app.Run();
