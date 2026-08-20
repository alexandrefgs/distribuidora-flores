using DistribuidoraFlores.Api.Modules.Frota.Application.Interfaces;
using DistribuidoraFlores.Api.Modules.Frota.Domain;

namespace DistribuidoraFlores.Api.Modules.Frota.Application.UseCases;

public class CadastrarMotoristaUseCase
{
    private readonly IMotoristaRepository _motoristaRepository;

    public CadastrarMotoristaUseCase(IMotoristaRepository motoristaRepository)
    {
        _motoristaRepository = motoristaRepository;
    }

    public async Task<Guid> ExecutarAsync(string nome, string telefone, string cnh)
    {
        var motorista = new Motorista(nome, telefone, cnh);

        await _motoristaRepository.AdicionarAsync(motorista);
        await _motoristaRepository.SalvarAlteracoesAsync();

        return motorista.Id;
    }
}