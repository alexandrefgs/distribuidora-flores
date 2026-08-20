namespace DistribuidoraFlores.Api.Modules.Identidade.Domain;

public class RefreshToken
{
    public Guid Id { get; private set; }
    public Guid UsuarioId { get; private set; }
    public string TokenHash { get; private set; }
    public DateTime DataExpiracao { get; private set; }
    public DateTime DataCriacao { get; private set; }
    public bool Revogado { get; private set; }

    protected RefreshToken()
    {
        TokenHash = null!;
    }

    public RefreshToken(Guid usuarioId, string tokenHash, DateTime dataExpiracao)
    {
        Id = Guid.NewGuid();
        UsuarioId = usuarioId;
        TokenHash = tokenHash;
        DataExpiracao = dataExpiracao;
        DataCriacao = DateTime.UtcNow;
        Revogado = false;
    }

    public bool EstaValido()
    {
        return !Revogado && DataExpiracao > DateTime.UtcNow;
    }

    public void Revogar()
    {
        Revogado = true;
    }
}