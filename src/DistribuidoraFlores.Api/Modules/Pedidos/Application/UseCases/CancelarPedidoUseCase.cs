using DistribuidoraFlores.Api.Modules.Pedidos.Application.Interfaces;

namespace DistribuidoraFlores.Api.Modules.Pedidos.Application.UseCases;

public class CancelarPedidoUseCase
{
    private readonly IPedidoRepository _pedidoRepository;

    public CancelarPedidoUseCase(IPedidoRepository pedidoRepository)
    {
        _pedidoRepository = pedidoRepository;
    }

    public async Task ExecutarAsync(Guid pedidoId)
    {
        var pedido = await _pedidoRepository.ObterPorIdAsync(pedidoId);

        if (pedido is null)
            throw new InvalidOperationException("Pedido não encontrado.");

        pedido.Cancelar();

        await _pedidoRepository.SalvarAlteracoesAsync();
    }
}