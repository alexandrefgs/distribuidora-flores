using Microsoft.EntityFrameworkCore;
using DistribuidoraFlores.Api.Infrastructure.Persistence;
using DistribuidoraFlores.Api.Modules.Identidade.Application.Interfaces;
using DistribuidoraFlores.Api.Modules.Identidade.Domain;

namespace DistribuidoraFlores.Api.Modules.Identidade.Infrastructure;

public class RefreshTokenRepository : IRefreshTokenRepository
{
    private readonly AppDbContext _context;

    public RefreshTokenRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<RefreshToken?> ObterPorTokenHashAsync(string tokenHash)
    {
        return await _context.RefreshTokens.FirstOrDefaultAsync(rt => rt.TokenHash == tokenHash);
    }

    public async Task AdicionarAsync(RefreshToken refreshToken)
    {
        await _context.RefreshTokens.AddAsync(refreshToken);
    }

    public async Task SalvarAlteracoesAsync()
    {
        await _context.SaveChangesAsync();
    }
}