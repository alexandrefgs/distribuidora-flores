namespace DistribuidoraFlores.Api.Modules.Identidade.Api.DTOs;

public record RegistrarComercianteRequest(
    string NomeFantasia,
    string Documento,
    string Telefone,
    string Endereco,
    string Email,
    string Senha
);