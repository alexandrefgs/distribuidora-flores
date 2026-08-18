namespace DistribuidoraFlores.Api.Modules.Catalogo.Domain;

public class Lote
{
    public Guid Id { get; private set; }
    public Guid ProdutoId { get; private set; }
    public int Quantidade { get; private set; }
    public DateTime DataEntrada { get; private set; }
    public DateTime DataValidade { get; private set; }

    protected Lote() { }

    public Lote(Guid produtoId, int quantidade, DateTime dataValidade)
    {
        if (quantidade <= 0)
            throw new ArgumentException("Quantidade deve ser maior que zero.");

        if (dataValidade <= DateTime.UtcNow)
            throw new ArgumentException("Data de validade deve ser futura.");

        Id = Guid.NewGuid();
        ProdutoId = produtoId;
        Quantidade = quantidade;
        DataEntrada = DateTime.UtcNow;
        DataValidade = dataValidade;
    }

    public bool EstaValido()
    {
        return DataValidade > DateTime.UtcNow && Quantidade > 0;
    }

    public bool EstaProximoDoVencimento(int diasAlerta = 2)
    {
        return EstaValido() && (DataValidade - DateTime.UtcNow).TotalDays <= diasAlerta;
    }

    public void ReduzirQuantidade(int quantidade)
    {
        if (quantidade > Quantidade)
            throw new InvalidOperationException("Quantidade insuficiente no lote.");

        Quantidade -= quantidade;
    }
}