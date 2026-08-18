using DistribuidoraFlores.Api.Modules.Pedidos.Application.DTOs;
using DistribuidoraFlores.Api.Modules.Pedidos.Application.Interfaces;
using DistribuidoraFlores.Api.Modules.Pedidos.Domain;
using DistribuidoraFlores.Api.Modules.Clientes.Application.Interfaces;
using DistribuidoraFlores.Api.Modules.Catalogo.Application.Interfaces;

namespace DistribuidoraFlores.Api.Modules.Pedidos.Application.UseCases;

public class CriarPedidoUseCase
{
    private readonly IPedidoRepository _pedidoRepository;
    private readonly IClienteRepository _clienteRepository;
    private readonly IProdutoRepository _produtoRepository;

    public CriarPedidoUseCase(
        IPedidoRepository pedidoRepository,
        IClienteRepository clienteRepository,
        IProdutoRepository produtoRepository)
    {
        _pedidoRepository = pedidoRepository;
        _clienteRepository = clienteRepository;
        _produtoRepository = produtoRepository;
    }

    public async Task<Guid> ExecutarAsync(Guid clienteId, List<ItemPedidoInput> itensSolicitados)
    {
        var cliente = await _clienteRepository.ObterPorIdAsync(clienteId);

        if (cliente is null || !cliente.Ativo)
            throw new InvalidOperationException("Cliente não encontrado ou inativo.");

        if (itensSolicitados.Count == 0)
            throw new ArgumentException("O pedido precisa ter ao menos um item.");

        var pedido = new Pedido(clienteId);
        var estoqueInsuficiente = false;

        foreach (var itemSolicitado in itensSolicitados)
        {
            var produto = await _produtoRepository.ObterPorIdAsync(itemSolicitado.ProdutoId);

            if (produto is null || !produto.Ativo)
                throw new InvalidOperationException($"Produto {itemSolicitado.ProdutoId} não encontrado ou inativo.");

            if (produto.QuantidadeDisponivel() < itemSolicitado.Quantidade)
                estoqueInsuficiente = true;

            pedido.AdicionarItem(produto.Id, produto.Nome, itemSolicitado.Quantidade, produto.PrecoUnitario);
        }

        if (estoqueInsuficiente)
            pedido.MarcarEstoqueInsuficiente();

        await _pedidoRepository.AdicionarAsync(pedido);
        await _pedidoRepository.SalvarAlteracoesAsync();

        return pedido.Id;
    }
}