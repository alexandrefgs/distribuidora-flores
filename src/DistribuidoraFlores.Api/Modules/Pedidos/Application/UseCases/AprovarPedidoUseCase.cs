using DistribuidoraFlores.Api.Modules.Pedidos.Application.Interfaces;

namespace DistribuidoraFlores.Api.Modules.Pedidos.Application.UseCases;

public class AprovarPedidoUseCase
{
    private readonly IPedidoRepository _pedidoRepository;

    public AprovarPedidoUseCase(IPedidoRepository pedidoRepository)
    {
        _pedidoRepository = pedidoRepository;
    }

    public async Task ExecutarAsync(Guid pedidoId)
    {
        var pedido = await _pedidoRepository.ObterPorIdAsync(pedidoId);

        if (pedido is null)
            throw new InvalidOperationException("Pedido não encontrado.");

        pedido.Aprovar();

        await _pedidoRepository.SalvarAlteracoesAsync();
    }
}