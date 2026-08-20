using DistribuidoraFlores.Api.Modules.Frota.Domain;

namespace DistribuidoraFlores.Api.Modules.Frota.Application.Interfaces;

public interface IVeiculoRepository
{
    Task<Veiculo?> ObterPorIdAsync(Guid id);
    Task<List<Veiculo>> ListarAtivosAsync();
    Task AdicionarAsync(Veiculo veiculo);
    Task SalvarAlteracoesAsync();
}