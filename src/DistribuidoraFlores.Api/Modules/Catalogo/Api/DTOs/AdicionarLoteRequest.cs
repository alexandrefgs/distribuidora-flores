namespace DistribuidoraFlores.Api.Modules.Catalogo.Api.DTOs;

public record AdicionarLoteRequest(
    int Quantidade,
    DateTime DataValidade
);