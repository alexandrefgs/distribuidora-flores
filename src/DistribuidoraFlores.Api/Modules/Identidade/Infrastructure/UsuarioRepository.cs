using Microsoft.EntityFrameworkCore;
using DistribuidoraFlores.Api.Infrastructure.Persistence;
using DistribuidoraFlores.Api.Modules.Identidade.Application.Interfaces;
using DistribuidoraFlores.Api.Modules.Identidade.Domain;

namespace DistribuidoraFlores.Api.Modules.Identidade.Infrastructure;

public class UsuarioRepository : IUsuarioRepository
{
    private readonly AppDbContext _context;

    public UsuarioRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<Usuario?> ObterPorEmailAsync(string email)
    {
        return await _context.Usuarios.FirstOrDefaultAsync(u => u.Email == email.ToLower());
    }

    public async Task<Usuario?> ObterPorIdAsync(Guid id)
    {
        return await _context.Usuarios.FirstOrDefaultAsync(u => u.Id == id);
    }

    public async Task<bool> ExisteAdminAsync()
    {
        return await _context.Usuarios.AnyAsync(u => u.Role == Role.Admin);
    }

    public async Task AdicionarAsync(Usuario usuario)
    {
        await _context.Usuarios.AddAsync(usuario);
    }

    public async Task SalvarAlteracoesAsync()
    {
        await _context.SaveChangesAsync();
    }
}