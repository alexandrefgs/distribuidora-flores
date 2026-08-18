namespace DistribuidoraFlores.Api.Modules.Catalogo.Domain;

public class Produto
{
    public Guid Id { get; private set; }
    public string Nome { get; private set; }
    public string Categoria { get; private set; }
    public string UnidadeMedida { get; private set; } // ex: "maço", "unidade", "dúzia"
    public decimal PrecoUnitario { get; private set; }
    public bool Ativo { get; private set; }

    private readonly List<Lote> _lotes = new();
    public IReadOnlyCollection<Lote> Lotes => _lotes.AsReadOnly();

    protected Produto() { } // EF Core precisa de um construtor vazio

    public Produto(string nome, string categoria, string unidadeMedida, decimal precoUnitario)
    {
        if (string.IsNullOrWhiteSpace(nome))
            throw new ArgumentException("Nome do produto é obrigatório.");

        if (precoUnitario <= 0)
            throw new ArgumentException("Preço deve ser maior que zero.");

        Id = Guid.NewGuid();
        Nome = nome;
        Categoria = categoria;
        UnidadeMedida = unidadeMedida;
        PrecoUnitario = precoUnitario;
        Ativo = true;
    }

    public void AdicionarLote(Lote lote)
    {
        _lotes.Add(lote);
    }

    public int QuantidadeDisponivel()
    {
        return _lotes
            .Where(l => l.EstaValido())
            .Sum(l => l.Quantidade);
    }

    public void Desativar()
    {
        Ativo = false;
    }
}