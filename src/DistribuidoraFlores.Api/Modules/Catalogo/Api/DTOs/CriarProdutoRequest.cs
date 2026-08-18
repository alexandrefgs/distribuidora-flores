namespace DistribuidoraFlores.Api.Modules.Catalogo.Api.DTOs;

public record CriarProdutoRequest(
    string Nome,
    string Categoria,
    string UnidadeMedida,
    decimal PrecoUnitario
);