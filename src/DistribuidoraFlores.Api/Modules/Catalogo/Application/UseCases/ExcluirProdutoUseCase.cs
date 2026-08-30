using DistribuidoraFlores.Api.Modules.Catalogo.Application.Interfaces;

namespace DistribuidoraFlores.Api.Modules.Catalogo.Application.UseCases;

public class ExcluirProdutoUseCase
{
    private readonly IProdutoRepository _produtoRepository;

    public ExcluirProdutoUseCase(IProdutoRepository produtoRepository)
    {
        _produtoRepository = produtoRepository;
    }

    public async Task ExecutarAsync(Guid produtoId)
    {
        var produto = await _produtoRepository.ObterPorIdAsync(produtoId);

        if (produto is null)
            throw new InvalidOperationException("Produto não encontrado.");

        produto.Desativar();

        await _produtoRepository.SalvarAlteracoesAsync();
    }
}