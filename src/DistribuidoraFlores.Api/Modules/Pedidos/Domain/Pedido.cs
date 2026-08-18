namespace DistribuidoraFlores.Api.Modules.Pedidos.Domain;

public class Pedido
{
    public Guid Id { get; private set; }
    public Guid ClienteId { get; private set; }
    public StatusPedido Status { get; private set; }
    public DateTime DataCriacao { get; private set; }
    public bool EstoqueInsuficienteNaCriacao { get; private set; }

    private readonly List<ItemPedido> _itens = new();
    public IReadOnlyCollection<ItemPedido> Itens => _itens.AsReadOnly();

    public decimal Total => _itens.Sum(i => i.Subtotal);

    protected Pedido() { }

    public Pedido(Guid clienteId)
    {
        Id = Guid.NewGuid();
        ClienteId = clienteId;
        Status = StatusPedido.Pendente;
        DataCriacao = DateTime.UtcNow;
        EstoqueInsuficienteNaCriacao = false;
    }

    public void AdicionarItem(Guid produtoId, string nomeProduto, int quantidade, decimal precoUnitario)
    {
        if (Status != StatusPedido.Pendente)
            throw new InvalidOperationException("Só é possível adicionar itens a um pedido pendente.");

        var item = new ItemPedido(produtoId, nomeProduto, quantidade, precoUnitario);
        _itens.Add(item);
    }

    public void MarcarEstoqueInsuficiente()
    {
        EstoqueInsuficienteNaCriacao = true;
    }

    public void Aprovar()
    {
        if (Status != StatusPedido.Pendente)
            throw new InvalidOperationException("Só é possível aprovar um pedido pendente.");

        if (_itens.Count == 0)
            throw new InvalidOperationException("Não é possível aprovar um pedido sem itens.");

        Status = StatusPedido.Aprovado;
    }

    public void MarcarComoSeparado()
    {
        if (Status != StatusPedido.Aprovado)
            throw new InvalidOperationException("Só é possível separar um pedido aprovado.");

        Status = StatusPedido.Separado;
    }

    public void MarcarEmRota()
    {
        if (Status != StatusPedido.Separado)
            throw new InvalidOperationException("Só é possível colocar em rota um pedido separado.");

        Status = StatusPedido.EmRota;
    }

    public void MarcarComoEntregue()
    {
        if (Status != StatusPedido.EmRota)
            throw new InvalidOperationException("Só é possível concluir um pedido que está em rota.");

        Status = StatusPedido.Entregue;
    }

    public void Cancelar()
    {
        if (Status is not (StatusPedido.Pendente or StatusPedido.Aprovado))
            throw new InvalidOperationException("Só é possível cancelar um pedido pendente ou aprovado.");

        Status = StatusPedido.Cancelado;
    }
}