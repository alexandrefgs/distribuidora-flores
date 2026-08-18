using DistribuidoraFlores.Api.Modules.Clientes.Domain;

namespace DistribuidoraFlores.Api.Modules.Clientes.Application.Interfaces;

public interface IClienteRepository
{
    Task<Cliente?> ObterPorIdAsync(Guid id);
    Task<Cliente?> ObterPorDocumentoAsync(string documento);
    Task<List<Cliente>> ListarAtivosAsync();
    Task AdicionarAsync(Cliente cliente);
    Task SalvarAlteracoesAsync();
}