using DistribuidoraFlores.Api.Modules.Pedidos.Domain;

namespace DistribuidoraFlores.Api.Modules.Pedidos.Application.Interfaces;

public interface IPedidoRepository
{
    Task<Pedido?> ObterPorIdAsync(Guid id);
    Task<List<Pedido>> ListarPorClienteAsync(Guid clienteId);
    Task<List<Pedido>> ListarTodosAsync();
    Task AdicionarAsync(Pedido pedido);
    Task SalvarAlteracoesAsync();
}