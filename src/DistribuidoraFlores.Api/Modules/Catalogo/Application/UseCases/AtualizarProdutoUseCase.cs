using DistribuidoraFlores.Api.Modules.Catalogo.Application.Interfaces;

namespace DistribuidoraFlores.Api.Modules.Catalogo.Application.UseCases;

public class AtualizarProdutoUseCase
{
    private readonly IProdutoRepository _produtoRepository;

    public AtualizarProdutoUseCase(IProdutoRepository produtoRepository)
    {
        _produtoRepository = produtoRepository;
    }

    public async Task ExecutarAsync(Guid produtoId, string nome, string categoria, string unidadeMedida, decimal precoUnitario)
    {
        var produto = await _produtoRepository.ObterPorIdAsync(produtoId);

        if (produto is null)
            throw new InvalidOperationException("Produto não encontrado.");

        produto.AtualizarDados(nome, categoria, unidadeMedida, precoUnitario);

        await _produtoRepository.SalvarAlteracoesAsync();
    }
}