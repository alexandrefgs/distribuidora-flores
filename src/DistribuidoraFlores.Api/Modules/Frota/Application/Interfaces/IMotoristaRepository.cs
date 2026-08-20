using DistribuidoraFlores.Api.Modules.Frota.Domain;

namespace DistribuidoraFlores.Api.Modules.Frota.Application.Interfaces;

public interface IMotoristaRepository
{
    Task<Motorista?> ObterPorIdAsync(Guid id);
    Task<List<Motorista>> ListarAtivosAsync();
    Task AdicionarAsync(Motorista motorista);
    Task SalvarAlteracoesAsync();
}