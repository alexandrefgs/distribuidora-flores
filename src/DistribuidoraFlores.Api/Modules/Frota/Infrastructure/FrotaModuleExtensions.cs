using DistribuidoraFlores.Api.Modules.Frota.Application.Interfaces;
using DistribuidoraFlores.Api.Modules.Frota.Application.UseCases;
using DistribuidoraFlores.Api.Modules.Frota.Infrastructure;

namespace DistribuidoraFlores.Api.Modules.Frota;

public static class FrotaModuleExtensions
{
    public static IServiceCollection AddFrotaModule(this IServiceCollection services)
    {
        services.AddScoped<IVeiculoRepository, VeiculoRepository>();
        services.AddScoped<IMotoristaRepository, MotoristaRepository>();
        services.AddScoped<IEntregaRepository, EntregaRepository>();

        services.AddScoped<CadastrarVeiculoUseCase>();
        services.AddScoped<CadastrarMotoristaUseCase>();
        services.AddScoped<CriarEntregaUseCase>();
        services.AddScoped<IniciarRotaUseCase>();
        services.AddScoped<ConcluirEntregaUseCase>();

        return services;
    }
}