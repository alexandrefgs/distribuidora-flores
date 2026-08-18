namespace DistribuidoraFlores.Api.Modules.Clientes.Api.DTOs;

public record CadastrarClienteRequest(
    string NomeFantasia,
    string Documento,
    string Telefone,
    string Email,
    string Endereco
);