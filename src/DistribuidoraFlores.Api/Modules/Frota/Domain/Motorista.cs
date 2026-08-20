namespace DistribuidoraFlores.Api.Modules.Frota.Domain;

public class Motorista
{
    public Guid Id { get; private set; }
    public string Nome { get; private set; }
    public string Telefone { get; private set; }
    public string Cnh { get; private set; }
    public bool Ativo { get; private set; }

    protected Motorista()
    {
        Nome = null!;
        Telefone = null!;
        Cnh = null!;
    }

    public Motorista(string nome, string telefone, string cnh)
    {
        if (string.IsNullOrWhiteSpace(nome))
            throw new ArgumentException("Nome é obrigatório.");

        if (string.IsNullOrWhiteSpace(cnh))
            throw new ArgumentException("CNH é obrigatória.");

        Id = Guid.NewGuid();
        Nome = nome;
        Telefone = telefone;
        Cnh = cnh;
        Ativo = true;
    }

    public void Desativar() => Ativo = false;
    public void Ativar() => Ativo = true;
}