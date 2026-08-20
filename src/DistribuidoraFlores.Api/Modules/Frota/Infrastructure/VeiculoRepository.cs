using Microsoft.EntityFrameworkCore;
using DistribuidoraFlores.Api.Infrastructure.Persistence;
using DistribuidoraFlores.Api.Modules.Frota.Application.Interfaces;
using DistribuidoraFlores.Api.Modules.Frota.Domain;

namespace DistribuidoraFlores.Api.Modules.Frota.Infrastructure;

public class VeiculoRepository : IVeiculoRepository
{
    private readonly AppDbContext _context;

    public VeiculoRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<Veiculo?> ObterPorIdAsync(Guid id)
    {
        return await _context.Veiculos.FirstOrDefaultAsync(v => v.Id == id);
    }

    public async Task<List<Veiculo>> ListarAtivosAsync()
    {
        return await _context.Veiculos.Where(v => v.Ativo).ToListAsync();
    }

    public async Task AdicionarAsync(Veiculo veiculo)
    {
        await _context.Veiculos.AddAsync(veiculo);
    }

    public async Task SalvarAlteracoesAsync()
    {
        await _context.SaveChangesAsync();
    }
}