using DistribuidoraFlores.Api.Modules.Catalogo.Application.Interfaces;
using DistribuidoraFlores.Api.Modules.Catalogo.Application.UseCases;
using DistribuidoraFlores.Api.Modules.Catalogo.Infrastructure;

namespace DistribuidoraFlores.Api.Modules.Catalogo;

public static class CatalogoModuleExtensions
{
    public static IServiceCollection AddCatalogoModule(this IServiceCollection services)
    {
        services.AddScoped<IProdutoRepository, ProdutoRepository>();
        services.AddScoped<CriarProdutoUseCase>();
        services.AddScoped<AdicionarLoteUseCase>();
        services.AddScoped<DefinirImagemProdutoUseCase>();

        return services;
    }
}