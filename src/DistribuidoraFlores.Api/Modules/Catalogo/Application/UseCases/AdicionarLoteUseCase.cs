using DistribuidoraFlores.Api.Modules.Catalogo.Application.Interfaces;
using DistribuidoraFlores.Api.Modules.Catalogo.Domain;

namespace DistribuidoraFlores.Api.Modules.Catalogo.Application.UseCases;

public class AdicionarLoteUseCase
{
    private readonly IProdutoRepository _produtoRepository;

    public AdicionarLoteUseCase(IProdutoRepository produtoRepository)
    {
        _produtoRepository = produtoRepository;
    }

    public async Task ExecutarAsync(Guid produtoId, int quantidade, DateTime dataValidade)
    {
        var produto = await _produtoRepository.ObterPorIdAsync(produtoId);

        if (produto is null)
            throw new InvalidOperationException("Produto não encontrado.");

        var lote = new Lote(produtoId, quantidade, dataValidade);
        produto.AdicionarLote(lote);

        await _produtoRepository.SalvarAlteracoesAsync();
    }
}