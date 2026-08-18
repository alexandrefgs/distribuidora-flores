using Microsoft.EntityFrameworkCore;
using DistribuidoraFlores.Api.Infrastructure.Persistence;
using DistribuidoraFlores.Api.Modules.Catalogo.Application.Interfaces;
using DistribuidoraFlores.Api.Modules.Catalogo.Application.UseCases;
using DistribuidoraFlores.Api.Modules.Catalogo.Infrastructure;
using DistribuidoraFlores.Api.Modules.Clientes.Application.Interfaces;
using DistribuidoraFlores.Api.Modules.Clientes.Application.UseCases;
using DistribuidoraFlores.Api.Modules.Clientes.Infrastructure;

var builder = WebApplication.CreateBuilder(args);

// Controllers
builder.Services.AddControllers();

// Swagger
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Banco de dados
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// Módulo Catalogo — Repositórios
builder.Services.AddScoped<IProdutoRepository, ProdutoRepository>();

// Módulo Catalogo — Casos de uso
builder.Services.AddScoped<CriarProdutoUseCase>();
builder.Services.AddScoped<AdicionarLoteUseCase>();
builder.Services.AddScoped<IClienteRepository, ClienteRepository>();
builder.Services.AddScoped<CadastrarClienteUseCase>();
builder.Services.AddScoped<AtivarClienteUseCase>();
builder.Services.AddScoped<DesativarClienteUseCase>();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
    app.MapGet("/", () => Results.Redirect("/swagger"));
}

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();

app.Run();