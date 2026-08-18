using DistribuidoraFlores.Api.Modules.Catalogo.Application.Interfaces;
using DistribuidoraFlores.Api.Modules.Catalogo.Domain;

namespace DistribuidoraFlores.Api.Modules.Catalogo.Application.UseCases;

public class CriarProdutoUseCase
{
    private readonly IProdutoRepository _produtoRepository;

    public CriarProdutoUseCase(IProdutoRepository produtoRepository)
    {
        _produtoRepository = produtoRepository;
    }

    public async Task<Guid> ExecutarAsync(string nome, string categoria, string unidadeMedida, decimal precoUnitario)
    {
        var produto = new Produto(nome, categoria, unidadeMedida, precoUnitario);

        await _produtoRepository.AdicionarAsync(produto);
        await _produtoRepository.SalvarAlteracoesAsync();

        return produto.Id;
    }
}