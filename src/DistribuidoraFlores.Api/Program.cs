using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using DistribuidoraFlores.Api.Infrastructure.Persistence;
using DistribuidoraFlores.Api.Modules.Catalogo;
using DistribuidoraFlores.Api.Modules.Clientes;
using DistribuidoraFlores.Api.Modules.Pedidos;
using DistribuidoraFlores.Api.Modules.Frota;
using DistribuidoraFlores.Api.Modules.Identidade;
using DistribuidoraFlores.Api.Modules.Identidade.Application.Interfaces;
using DistribuidoraFlores.Api.Modules.Identidade.Domain;

var builder = WebApplication.CreateBuilder(args);

// Controllers
builder.Services.AddControllers();

// Swagger
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// CORS — permite o frontend Angular (rodando em outra porta) chamar esta API
builder.Services.AddCors(options =>
{
    options.AddPolicy("PermitirAngular", policy =>
    {
        policy.WithOrigins("http://localhost:4200")
              .AllowAnyHeader()
              .AllowAnyMethod();
    });
});

// Banco de dados
if (!builder.Environment.IsEnvironment("Testing"))
{
    builder.Services.AddDbContext<AppDbContext>(options =>
        options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));
}

// Autenticação JWT
var chaveJwt = builder.Configuration["Jwt:ChaveSecreta"]
    ?? throw new InvalidOperationException("Chave JWT não configurada.");

builder.Services
    .AddAuthentication(options =>
    {
        options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
        options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
    })
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
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(chaveJwt))
        };
    });

builder.Services.AddAuthorization();

// Módulos
builder.Services
    .AddCatalogoModule()
    .AddClientesModule()
    .AddPedidosModule()
    .AddFrotaModule()
    .AddIdentidadeModule();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
    app.MapGet("/", () => Results.Redirect("/swagger"));
}

app.UseHttpsRedirection();
app.UseCors("PermitirAngular");
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();

// Seed do usuário Admin (só cria se ainda não existir nenhum admin)
if (!app.Environment.IsEnvironment("Testing"))
{
    using var scope = app.Services.CreateScope();
    var usuarioRepository = scope.ServiceProvider.GetRequiredService<IUsuarioRepository>();
    var passwordHasher = scope.ServiceProvider.GetRequiredService<IPasswordHasher>();

    if (!await usuarioRepository.ExisteAdminAsync())
    {
        var senhaHash = passwordHasher.GerarHash("Admin@123");
        var admin = Usuario.CriarAdmin("admin@distribuidoraflores.com", senhaHash);

        await usuarioRepository.AdicionarAsync(admin);
        await usuarioRepository.SalvarAlteracoesAsync();
    }
}

app.Run();

public partial class Program { }