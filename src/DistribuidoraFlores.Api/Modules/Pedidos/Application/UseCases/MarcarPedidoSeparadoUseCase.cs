using DistribuidoraFlores.Api.Modules.Pedidos.Application.Interfaces;

namespace DistribuidoraFlores.Api.Modules.Pedidos.Application.UseCases;

public class MarcarPedidoSeparadoUseCase
{
    private readonly IPedidoRepository _pedidoRepository;

    public MarcarPedidoSeparadoUseCase(IPedidoRepository pedidoRepository)
    {
        _pedidoRepository = pedidoRepository;
    }

    public async Task ExecutarAsync(Guid pedidoId)
    {
        var pedido = await _pedidoRepository.ObterPorIdAsync(pedidoId);

        if (pedido is null)
            throw new InvalidOperationException("Pedido não encontrado.");

        pedido.MarcarComoSeparado();

        await _pedidoRepository.SalvarAlteracoesAsync();
    }
}