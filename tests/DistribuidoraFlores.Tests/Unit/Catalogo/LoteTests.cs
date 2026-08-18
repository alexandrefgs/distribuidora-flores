using DistribuidoraFlores.Api.Modules.Catalogo.Domain;
using FluentAssertions;
using Xunit;

namespace DistribuidoraFlores.Tests.Unit.Catalogo;

public class LoteTests
{
    [Fact]
    public void Deve_criar_lote_valido_com_dados_corretos()
    {
        var produtoId = Guid.NewGuid();
        var lote = new Lote(produtoId, 50, DateTime.UtcNow.AddDays(5));

        lote.Quantidade.Should().Be(50);
        lote.EstaValido().Should().BeTrue();
    }

    [Fact]
    public void Nao_deve_permitir_quantidade_zero_ou_negativa()
    {
        var acao = () => new Lote(Guid.NewGuid(), 0, DateTime.UtcNow.AddDays(5));

        acao.Should().Throw<ArgumentException>()
            .WithMessage("Quantidade deve ser maior que zero.");
    }

    [Fact]
    public void Nao_deve_permitir_data_de_validade_no_passado()
    {
        var acao = () => new Lote(Guid.NewGuid(), 10, DateTime.UtcNow.AddDays(-1));

        acao.Should().Throw<ArgumentException>()
            .WithMessage("Data de validade deve ser futura.");
    }

    [Fact]
    public void Lote_vencido_nao_deve_estar_valido()
    {
        // Criamos válido e "avançamos o tempo" simulando consulta após vencer
        var lote = new Lote(Guid.NewGuid(), 10, DateTime.UtcNow.AddSeconds(1));

        Thread.Sleep(1100);

        lote.EstaValido().Should().BeFalse();
    }

    [Fact]
    public void Deve_identificar_lote_proximo_do_vencimento()
    {
        var lote = new Lote(Guid.NewGuid(), 10, DateTime.UtcNow.AddDays(1));

        lote.EstaProximoDoVencimento(diasAlerta: 2).Should().BeTrue();
    }

    [Fact]
    public void Nao_deve_reduzir_quantidade_alem_do_disponivel()
    {
        var lote = new Lote(Guid.NewGuid(), 10, DateTime.UtcNow.AddDays(5));

        var acao = () => lote.ReduzirQuantidade(20);

        acao.Should().Throw<InvalidOperationException>()
            .WithMessage("Quantidade insuficiente no lote.");
    }
}