using DistribuidoraFlores.Api.Modules.Catalogo.Domain;

namespace DistribuidoraFlores.Api.Modules.Catalogo.Api.DTOs;

public record ProdutoResponse(
    Guid Id,
    string Nome,
    string Categoria,
    string UnidadeMedida,
    decimal PrecoUnitario,
    int QuantidadeDisponivel,
    string? ImagemUrl
)
{
    public static ProdutoResponse FromDomain(Produto produto)
    {
        return new ProdutoResponse(
            produto.Id,
            produto.Nome,
            produto.Categoria,
            produto.UnidadeMedida,
            produto.PrecoUnitario,
            produto.QuantidadeDisponivel(),
            produto.ImagemUrl
        );
    }
}