using DistribuidoraFlores.Api.Modules.Identidade.Domain;

namespace DistribuidoraFlores.Api.Modules.Identidade.Application.Interfaces;

public interface IRefreshTokenRepository
{
    Task<RefreshToken?> ObterPorTokenHashAsync(string tokenHash);
    Task AdicionarAsync(RefreshToken refreshToken);
    Task SalvarAlteracoesAsync();
}