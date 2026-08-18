using DistribuidoraFlores.Api.Modules.Clientes.Domain;

namespace DistribuidoraFlores.Api.Modules.Clientes.Api.DTOs;

public record ClienteResponse(
    Guid Id,
    string NomeFantasia,
    string Documento,
    string TipoDocumento,
    string Telefone,
    string Email,
    string Endereco,
    bool Ativo,
    DateTime DataCadastro
)
{
    public static ClienteResponse FromDomain(Cliente cliente)
    {
        return new ClienteResponse(
            cliente.Id,
            cliente.NomeFantasia,
            cliente.Documento.Numero,
            cliente.Documento.Tipo.ToString(),
            cliente.Telefone,
            cliente.Email,
            cliente.Endereco,
            cliente.Ativo,
            cliente.DataCadastro
        );
    }
}