using DistribuidoraFlores.Api.Modules.Identidade.Domain;

namespace DistribuidoraFlores.Api.Modules.Identidade.Application.Interfaces;

public interface ITokenService
{
    string GerarAccessToken(Usuario usuario);
    string GerarRefreshTokenBruto(); // string aleatória, antes do hash
    string HashRefreshToken(string tokenBruto);
}