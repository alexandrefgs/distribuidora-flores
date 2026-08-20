using DistribuidoraFlores.Api.Modules.Identidade.Domain;

namespace DistribuidoraFlores.Api.Modules.Identidade.Application.Interfaces;

public interface ITokenService
{
    string GerarToken(Usuario usuario);
}