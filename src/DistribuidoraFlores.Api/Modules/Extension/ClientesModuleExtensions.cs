using DistribuidoraFlores.Api.Modules.Clientes.Application.Interfaces;
using DistribuidoraFlores.Api.Modules.Clientes.Application.UseCases;
using DistribuidoraFlores.Api.Modules.Clientes.Infrastructure;

namespace DistribuidoraFlores.Api.Modules.Clientes;

public static class ClientesModuleExtensions
{
    public static IServiceCollection AddClientesModule(this IServiceCollection services)
    {
        services.AddScoped<IClienteRepository, ClienteRepository>();
        services.AddScoped<CadastrarClienteUseCase>();
        services.AddScoped<AtivarClienteUseCase>();
        services.AddScoped<DesativarClienteUseCase>();

        return services;
    }
}