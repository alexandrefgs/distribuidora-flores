using DistribuidoraFlores.Api.Modules.Frota.Domain;

namespace DistribuidoraFlores.Api.Modules.Frota.Application.Interfaces;

public interface IEntregaRepository
{
    Task<Entrega?> ObterPorIdAsync(Guid id);
    Task<List<Entrega>> ListarTodasAsync();
    Task AdicionarAsync(Entrega entrega);
    Task SalvarAlteracoesAsync();
}