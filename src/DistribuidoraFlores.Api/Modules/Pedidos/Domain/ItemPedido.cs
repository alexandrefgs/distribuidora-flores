namespace DistribuidoraFlores.Api.Modules.Pedidos.Domain;

public class ItemPedido
{
    public Guid Id { get; private set; }
    public Guid PedidoId { get; private set; }
    public Guid ProdutoId { get; private set; }
    public string NomeProduto { get; private set; } // snapshot, caso o produto mude de nome depois
    public int Quantidade { get; private set; }
    public decimal PrecoUnitario { get; private set; } // congelado no momento do pedido

    protected ItemPedido()
    {
        NomeProduto = null!;
    }

    public ItemPedido(Guid produtoId, string nomeProduto, int quantidade, decimal precoUnitario)
    {
        if (quantidade <= 0)
            throw new ArgumentException("Quantidade deve ser maior que zero.");

        if (precoUnitario <= 0)
            throw new ArgumentException("Preço deve ser maior que zero.");

        Id = Guid.NewGuid();
        ProdutoId = produtoId;
        NomeProduto = nomeProduto;
        Quantidade = quantidade;
        PrecoUnitario = precoUnitario;
    }

    public decimal Subtotal => Quantidade * PrecoUnitario;
}