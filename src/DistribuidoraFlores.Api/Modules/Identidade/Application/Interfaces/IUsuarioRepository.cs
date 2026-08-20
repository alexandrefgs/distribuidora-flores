using DistribuidoraFlores.Api.Modules.Identidade.Domain;

namespace DistribuidoraFlores.Api.Modules.Identidade.Application.Interfaces;

public interface IUsuarioRepository
{
    Task<Usuario?> ObterPorEmailAsync(string email);
    Task<Usuario?> ObterPorIdAsync(Guid id);
    Task<bool> ExisteAdminAsync();
    Task AdicionarAsync(Usuario usuario);
    Task SalvarAlteracoesAsync();
}