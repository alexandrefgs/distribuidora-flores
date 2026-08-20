using DistribuidoraFlores.Api.Modules.Frota.Application.Interfaces;

namespace DistribuidoraFlores.Api.Modules.Frota.Application.UseCases;

public class ConcluirEntregaUseCase
{
    private readonly IEntregaRepository _entregaRepository;

    public ConcluirEntregaUseCase(IEntregaRepository entregaRepository)
    {
        _entregaRepository = entregaRepository;
    }

    public async Task ExecutarAsync(Guid entregaId)
    {
        var entrega = await _entregaRepository.ObterPorIdAsync(entregaId);

        if (entrega is null)
            throw new InvalidOperationException("Entrega não encontrada.");

        entrega.Concluir();

        await _entregaRepository.SalvarAlteracoesAsync();
    }
}