using DistribuidoraFlores.Api.Modules.Frota.Application.Interfaces;
using DistribuidoraFlores.Api.Modules.Frota.Domain;
using DistribuidoraFlores.Api.Modules.Pedidos.Application.Interfaces;
using DistribuidoraFlores.Api.Modules.Pedidos.Domain;

namespace DistribuidoraFlores.Api.Modules.Frota.Application.UseCases;

public class CriarEntregaUseCase
{
    private readonly IEntregaRepository _entregaRepository;
    private readonly IVeiculoRepository _veiculoRepository;
    private readonly IMotoristaRepository _motoristaRepository;
    private readonly IPedidoRepository _pedidoRepository;

    public CriarEntregaUseCase(
        IEntregaRepository entregaRepository,
        IVeiculoRepository veiculoRepository,
        IMotoristaRepository motoristaRepository,
        IPedidoRepository pedidoRepository)
    {
        _entregaRepository = entregaRepository;
        _veiculoRepository = veiculoRepository;
        _motoristaRepository = motoristaRepository;
        _pedidoRepository = pedidoRepository;
    }

    public async Task<Guid> ExecutarAsync(Guid pedidoId, Guid veiculoId, Guid motoristaId)
    {
        var pedido = await _pedidoRepository.ObterPorIdAsync(pedidoId);
        if (pedido is null)
            throw new InvalidOperationException("Pedido não encontrado.");

        if (pedido.Status != StatusPedido.Separado)
            throw new InvalidOperationException("Só é possível criar entrega para um pedido separado.");

        var veiculo = await _veiculoRepository.ObterPorIdAsync(veiculoId);
        if (veiculo is null || !veiculo.Ativo)
            throw new InvalidOperationException("Veículo não encontrado ou inativo.");

        var motorista = await _motoristaRepository.ObterPorIdAsync(motoristaId);
        if (motorista is null || !motorista.Ativo)
            throw new InvalidOperationException("Motorista não encontrado ou inativo.");

        var entrega = new Entrega(pedidoId, veiculoId, motoristaId);

        await _entregaRepository.AdicionarAsync(entrega);
        await _entregaRepository.SalvarAlteracoesAsync();

        return entrega.Id;
    }
}