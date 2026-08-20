using DistribuidoraFlores.Api.Modules.Frota.Application.Interfaces;
using DistribuidoraFlores.Api.Modules.Frota.Domain;

namespace DistribuidoraFlores.Api.Modules.Frota.Application.UseCases;

public class CadastrarVeiculoUseCase
{
    private readonly IVeiculoRepository _veiculoRepository;

    public CadastrarVeiculoUseCase(IVeiculoRepository veiculoRepository)
    {
        _veiculoRepository = veiculoRepository;
    }

    public async Task<Guid> ExecutarAsync(string placa, string modelo, int capacidadeKg)
    {
        var veiculo = new Veiculo(placa, modelo, capacidadeKg);

        await _veiculoRepository.AdicionarAsync(veiculo);
        await _veiculoRepository.SalvarAlteracoesAsync();

        return veiculo.Id;
    }
}