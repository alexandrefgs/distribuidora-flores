using DistribuidoraFlores.Api.Modules.Pedidos.Application.Interfaces;

namespace DistribuidoraFlores.Api.Modules.Pedidos.Application.UseCases;

public class MarcarPedidoEntregueUseCase
{
    private readonly IPedidoRepository _pedidoRepository;

    public MarcarPedidoEntregueUseCase(IPedidoRepository pedidoRepository)
    {
        _pedidoRepository = pedidoRepository;
    }

    public async Task ExecutarAsync(Guid pedidoId)
    {
        var pedido = await _pedidoRepository.ObterPorIdAsync(pedidoId);

        if (pedido is null)
            throw new InvalidOperationException("Pedido não encontrado.");

        pedido.MarcarComoEntregue();

        await _pedidoRepository.SalvarAlteracoesAsync();
    }
}