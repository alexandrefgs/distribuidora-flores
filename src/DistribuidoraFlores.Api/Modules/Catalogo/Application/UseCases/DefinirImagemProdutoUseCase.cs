using DistribuidoraFlores.Api.Modules.Catalogo.Application.Interfaces;

namespace DistribuidoraFlores.Api.Modules.Catalogo.Application.UseCases;

public class DefinirImagemProdutoUseCase
{
    private readonly IProdutoRepository _produtoRepository;

    public DefinirImagemProdutoUseCase(IProdutoRepository produtoRepository)
    {
        _produtoRepository = produtoRepository;
    }

    public async Task ExecutarAsync(Guid produtoId, string imagemUrl)
    {
        var produto = await _produtoRepository.ObterPorIdAsync(produtoId);

        if (produto is null)
            throw new InvalidOperationException("Produto não encontrado.");

        produto.DefinirImagem(imagemUrl);

        await _produtoRepository.SalvarAlteracoesAsync();
    }
}