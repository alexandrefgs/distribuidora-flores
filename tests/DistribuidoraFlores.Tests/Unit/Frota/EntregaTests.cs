using DistribuidoraFlores.Api.Modules.Frota.Domain;
using FluentAssertions;
using Xunit;

namespace DistribuidoraFlores.Tests.Unit.Frota;

public class EntregaTests
{
    [Fact]
    public void Entrega_deve_nascer_como_aguardando_saida()
    {
        var entrega = new Entrega(Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid());

        entrega.Status.Should().Be(StatusEntrega.AguardandoSaida);
        entrega.DataConclusao.Should().BeNull();
    }

    [Fact]
    public void IniciarRota_deve_mudar_status_para_em_rota()
    {
        var entrega = new Entrega(Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid());

        entrega.IniciarRota();

        entrega.Status.Should().Be(StatusEntrega.EmRota);
    }

    [Fact]
    public void Nao_deve_iniciar_rota_de_entrega_ja_em_rota()
    {
        var entrega = new Entrega(Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid());
        entrega.IniciarRota();

        var acao = () => entrega.IniciarRota();

        acao.Should().Throw<InvalidOperationException>()
            .WithMessage("Só é possível iniciar rota de uma entrega aguardando saída.");
    }

    [Fact]
    public void Concluir_deve_preencher_data_de_conclusao()
    {
        var entrega = new Entrega(Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid());
        entrega.IniciarRota();

        entrega.Concluir();

        entrega.Status.Should().Be(StatusEntrega.Concluida);
        entrega.DataConclusao.Should().NotBeNull();
    }

    [Fact]
    public void Nao_deve_concluir_entrega_que_ainda_nao_esta_em_rota()
    {
        var entrega = new Entrega(Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid());

        var acao = () => entrega.Concluir();

        acao.Should().Throw<InvalidOperationException>()
            .WithMessage("Só é possível concluir uma entrega em rota.");
    }
}