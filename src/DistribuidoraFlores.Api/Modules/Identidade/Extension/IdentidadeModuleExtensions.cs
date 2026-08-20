using DistribuidoraFlores.Api.Modules.Identidade.Application.Interfaces;
using DistribuidoraFlores.Api.Modules.Identidade.Application.UseCases;
using DistribuidoraFlores.Api.Modules.Identidade.Infrastructure;

namespace DistribuidoraFlores.Api.Modules.Identidade;

public static class IdentidadeModuleExtensions
{
    public static IServiceCollection AddIdentidadeModule(this IServiceCollection services)
    {
        services.AddScoped<IUsuarioRepository, UsuarioRepository>();
        services.AddScoped<IPasswordHasher, BCryptPasswordHasher>();
        services.AddScoped<ITokenService, JwtTokenService>();

        services.AddScoped<RegistrarComercianteUseCase>();
        services.AddScoped<LoginUseCase>();

        return services;
    }
}