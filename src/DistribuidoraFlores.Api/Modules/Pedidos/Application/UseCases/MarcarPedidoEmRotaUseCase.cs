using DistribuidoraFlores.Api.Modules.Pedidos.Application.Interfaces;

namespace DistribuidoraFlores.Api.Modules.Pedidos.Application.UseCases;

public class MarcarPedidoEmRotaUseCase
{
    private readonly IPedidoRepository _pedidoRepository;

    public MarcarPedidoEmRotaUseCase(IPedidoRepository pedidoRepository)
    {
        _pedidoRepository = pedidoRepository;
    }

    public async Task ExecutarAsync(Guid pedidoId)
    {
        var pedido = await _pedidoRepository.ObterPorIdAsync(pedidoId);

        if (pedido is null)
            throw new InvalidOperationException("Pedido não encontrado.");

        pedido.MarcarEmRota();

        await _pedidoRepository.SalvarAlteracoesAsync();
    }
}