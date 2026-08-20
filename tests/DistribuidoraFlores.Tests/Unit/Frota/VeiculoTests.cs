using DistribuidoraFlores.Api.Modules.Frota.Domain;
using FluentAssertions;
using Xunit;

namespace DistribuidoraFlores.Tests.Unit.Frota;

public class VeiculoTests
{
    [Fact]
    public void Deve_normalizar_placa_para_maiusculo()
    {
        var veiculo = new Veiculo("abc1234", "Fiorino", 500);

        veiculo.Placa.Should().Be("ABC1234");
    }

    [Fact]
    public void Nao_deve_permitir_placa_vazia()
    {
        var acao = () => new Veiculo("", "Fiorino", 500);

        acao.Should().Throw<ArgumentException>()
            .WithMessage("Placa é obrigatória.");
    }

    [Fact]
    public void Nao_deve_permitir_capacidade_zero_ou_negativa()
    {
        var acao = () => new Veiculo("ABC1234", "Fiorino", 0);

        acao.Should().Throw<ArgumentException>()
            .WithMessage("Capacidade deve ser maior que zero.");
    }
}