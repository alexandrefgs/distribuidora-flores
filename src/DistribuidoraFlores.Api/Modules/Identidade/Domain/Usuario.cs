namespace DistribuidoraFlores.Api.Modules.Identidade.Domain;

public class Usuario
{
    public Guid Id { get; private set; }
    public string Email { get; private set; }
    public string SenhaHash { get; private set; }
    public Role Role { get; private set; }
    public Guid? ClienteId { get; private set; } // vínculo com o Cliente, quando Role = Comerciante
    public bool Ativo { get; private set; }
    public DateTime DataCriacao { get; private set; }

    protected Usuario()
    {
        Email = null!;
        SenhaHash = null!;
    }

    private Usuario(string email, string senhaHash, Role role, Guid? clienteId)
    {
        if (string.IsNullOrWhiteSpace(email) || !email.Contains('@'))
            throw new ArgumentException("Email inválido.");

        Id = Guid.NewGuid();
        Email = email.ToLowerInvariant();
        SenhaHash = senhaHash;
        Role = role;
        ClienteId = clienteId;
        Ativo = true;
        DataCriacao = DateTime.UtcNow;
    }

    public static Usuario CriarComerciante(string email, string senhaHash, Guid clienteId)
    {
        return new Usuario(email, senhaHash, Role.Comerciante, clienteId);
    }

    public static Usuario CriarAdmin(string email, string senhaHash)
    {
        return new Usuario(email, senhaHash, Role.Admin, clienteId: null);
    }

    public void Desativar() => Ativo = false;
}