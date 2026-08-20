using DistribuidoraFlores.Api.Modules.Identidade.Application.Interfaces;

namespace DistribuidoraFlores.Api.Modules.Identidade.Infrastructure;

public class BCryptPasswordHasher : IPasswordHasher
{
    public string GerarHash(string senha)
    {
        return BCrypt.Net.BCrypt.HashPassword(senha);
    }

    public bool Verificar(string senha, string hash)
    {
        return BCrypt.Net.BCrypt.Verify(senha, hash);
    }
}