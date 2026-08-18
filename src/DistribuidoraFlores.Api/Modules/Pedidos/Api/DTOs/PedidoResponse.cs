using DistribuidoraFlores.Api.Modules.Pedidos.Domain;

namespace DistribuidoraFlores.Api.Modules.Pedidos.Api.DTOs;

public record ItemPedidoResponse(
    Guid ProdutoId,
    string NomeProduto,
    int Quantidade,
    decimal PrecoUnitario,
    decimal Subtotal
);

public record PedidoResponse(
    Guid Id,
    Guid ClienteId,
    string Status,
    DateTime DataCriacao,
    bool EstoqueInsuficienteNaCriacao,
    decimal Total,
    List<ItemPedidoResponse> Itens
)
{
    public static PedidoResponse FromDomain(Pedido pedido)
    {
        return new PedidoResponse(
            pedido.Id,
            pedido.ClienteId,
            pedido.Status.ToString(),
            pedido.DataCriacao,
            pedido.EstoqueInsuficienteNaCriacao,
            pedido.Total,
            pedido.Itens.Select(i => new ItemPedidoResponse(
                i.ProdutoId,
                i.NomeProduto,
                i.Quantidade,
                i.PrecoUnitario,
                i.Subtotal
            )).ToList()
        );
    }
}