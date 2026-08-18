using DistribuidoraFlores.Api.Modules.Pedidos.Application.Interfaces;
using DistribuidoraFlores.Api.Modules.Pedidos.Application.UseCases;
using DistribuidoraFlores.Api.Modules.Pedidos.Infrastructure;

namespace DistribuidoraFlores.Api.Modules.Pedidos;

public static class PedidosModuleExtensions
{
    public static IServiceCollection AddPedidosModule(this IServiceCollection services)
    {
        services.AddScoped<IPedidoRepository, PedidoRepository>();
        services.AddScoped<CriarPedidoUseCase>();
        services.AddScoped<AprovarPedidoUseCase>();
        services.AddScoped<MarcarPedidoSeparadoUseCase>();
        services.AddScoped<MarcarPedidoEmRotaUseCase>();
        services.AddScoped<MarcarPedidoEntregueUseCase>();
        services.AddScoped<CancelarPedidoUseCase>();

        return services;
    }
}