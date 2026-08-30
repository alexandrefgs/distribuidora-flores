namespace DistribuidoraFlores.Api.Modules.Catalogo.Api.DTOs;

public record AtualizarProdutoRequest(
    string Nome,
    string Categoria,
    string UnidadeMedida,
    decimal PrecoUnitario
);