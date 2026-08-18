using DistribuidoraFlores.Api.Modules.Catalogo.Domain;

namespace DistribuidoraFlores.Api.Modules.Catalogo.Application.Interfaces;

public interface IProdutoRepository
{
    Task<Produto?> ObterPorIdAsync(Guid id);
    Task<List<Produto>> ListarAtivosAsync();
    Task AdicionarAsync(Produto produto);
    Task SalvarAlteracoesAsync();
}