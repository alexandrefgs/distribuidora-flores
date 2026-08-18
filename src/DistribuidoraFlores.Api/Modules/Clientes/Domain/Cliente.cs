namespace DistribuidoraFlores.Api.Modules.Clientes.Domain;

public class Cliente
{
    public Guid Id { get; private set; }
    public string NomeFantasia { get; private set; }
    public Documento Documento { get; private set; }
    public string Telefone { get; private set; }
    public string Email { get; private set; }
    public string Endereco { get; private set; }
    public bool Ativo { get; private set; }
    public DateTime DataCadastro { get; private set; }

    protected Cliente()
    {
        NomeFantasia = null!;
        Documento = null!;
        Telefone = null!;
        Email = null!;
        Endereco = null!;
    }

    public Cliente(string nomeFantasia, string documento, string telefone, string email, string endereco)
    {
        if (string.IsNullOrWhiteSpace(nomeFantasia))
            throw new ArgumentException("Nome é obrigatório.");

        if (string.IsNullOrWhiteSpace(email) || !email.Contains('@'))
            throw new ArgumentException("Email inválido.");

        Id = Guid.NewGuid();
        NomeFantasia = nomeFantasia;
        Documento = Documento.Criar(documento);
        Telefone = telefone;
        Email = email;
        Endereco = endereco;
        Ativo = true;
        DataCadastro = DateTime.UtcNow;
    }

    public void Desativar() => Ativo = false;
    public void Ativar() => Ativo = true;
}