using DistribuidoraFlores.Api.Modules.Pedidos.Domain;
using FluentAssertions;
using Xunit;

namespace DistribuidoraFlores.Tests.Unit.Pedidos;

public class PedidoTests
{
    private static Pedido CriarPedidoComItem()
    {
        var pedido = new Pedido(Guid.NewGuid());
        pedido.AdicionarItem(Guid.NewGuid(), "Rosa Vermelha", 10, 5.50m);
        return pedido;
    }

    [Fact]
    public void Pedido_deve_nascer_como_pendente()
    {
        var pedido = new Pedido(Guid.NewGuid());

        pedido.Status.Should().Be(StatusPedido.Pendente);
    }

    [Fact]
    public void Total_deve_ser_soma_dos_subtotais_dos_itens()
    {
        var pedido = new Pedido(Guid.NewGuid());
        pedido.AdicionarItem(Guid.NewGuid(), "Rosa Vermelha", 10, 5.50m);
        pedido.AdicionarItem(Guid.NewGuid(), "Girassol", 5, 3.00m);

        pedido.Total.Should().Be(70.00m); // (10*5.50) + (5*3.00)
    }

    [Fact]
    public void Nao_deve_adicionar_item_com_quantidade_zero()
    {
        var pedido = new Pedido(Guid.NewGuid());

        var acao = () => pedido.AdicionarItem(Guid.NewGuid(), "Rosa", 0, 5.50m);

        acao.Should().Throw<ArgumentException>();
    }

    [Fact]
    public void Nao_deve_adicionar_item_apos_pedido_aprovado()
    {
        var pedido = CriarPedidoComItem();
        pedido.Aprovar();

        var acao = () => pedido.AdicionarItem(Guid.NewGuid(), "Girassol", 5, 3.00m);

        acao.Should().Throw<InvalidOperationException>()
            .WithMessage("Só é possível adicionar itens a um pedido pendente.");
    }

    [Fact]
    public void Nao_deve_aprovar_pedido_sem_itens()
    {
        var pedido = new Pedido(Guid.NewGuid());

        var acao = () => pedido.Aprovar();

        acao.Should().Throw<InvalidOperationException>()
            .WithMessage("Não é possível aprovar um pedido sem itens.");
    }

    [Fact]
    public void Fluxo_completo_deve_seguir_a_sequencia_correta()
    {
        var pedido = CriarPedidoComItem();

        pedido.Aprovar();
        pedido.Status.Should().Be(StatusPedido.Aprovado);

        pedido.MarcarComoSeparado();
        pedido.Status.Should().Be(StatusPedido.Separado);

        pedido.MarcarEmRota();
        pedido.Status.Should().Be(StatusPedido.EmRota);

        pedido.MarcarComoEntregue();
        pedido.Status.Should().Be(StatusPedido.Entregue);
    }

    [Theory]
    [InlineData(StatusPedido.Separado)]
    [InlineData(StatusPedido.EmRota)]
    [InlineData(StatusPedido.Entregue)]
    public void Nao_deve_aprovar_pedido_que_nao_esta_pendente(StatusPedido statusAlvo)
    {
        var pedido = AvancarPedidoPara(statusAlvo);

        var acao = () => pedido.Aprovar();

        acao.Should().Throw<InvalidOperationException>();
    }

    [Fact]
    public void Nao_deve_pular_etapa_direto_de_aprovado_para_em_rota()
    {
        var pedido = CriarPedidoComItem();
        pedido.Aprovar();

        var acao = () => pedido.MarcarEmRota();

        acao.Should().Throw<InvalidOperationException>()
            .WithMessage("Só é possível colocar em rota um pedido separado.");
    }

    [Theory]
    [InlineData(StatusPedido.Pendente)]
    [InlineData(StatusPedido.Aprovado)]
    public void Deve_permitir_cancelar_pedido_pendente_ou_aprovado(StatusPedido statusInicial)
    {
        var pedido = AvancarPedidoPara(statusInicial);

        pedido.Cancelar();

        pedido.Status.Should().Be(StatusPedido.Cancelado);
    }

    [Theory]
    [InlineData(StatusPedido.Separado)]
    [InlineData(StatusPedido.EmRota)]
    [InlineData(StatusPedido.Entregue)]
    public void Nao_deve_permitir_cancelar_pedido_apos_separado(StatusPedido statusAlvo)
    {
        var pedido = AvancarPedidoPara(statusAlvo);

        var acao = () => pedido.Cancelar();

        acao.Should().Throw<InvalidOperationException>()
            .WithMessage("Só é possível cancelar um pedido pendente ou aprovado.");
    }

    [Fact]
    public void MarcarEstoqueInsuficiente_nao_deve_bloquear_criacao()
    {
        var pedido = CriarPedidoComItem();

        pedido.MarcarEstoqueInsuficiente();

        pedido.EstoqueInsuficienteNaCriacao.Should().BeTrue();
        pedido.Status.Should().Be(StatusPedido.Pendente);
    }

    // Helper que avança o pedido até o status desejado, reaproveitando as próprias transições
    private static Pedido AvancarPedidoPara(StatusPedido status)
    {
        var pedido = CriarPedidoComItem();

        if (status == StatusPedido.Pendente) return pedido;

        pedido.Aprovar();
        if (status == StatusPedido.Aprovado) return pedido;

        pedido.MarcarComoSeparado();
        if (status == StatusPedido.Separado) return pedido;

        pedido.MarcarEmRota();
        if (status == StatusPedido.EmRota) return pedido;

        pedido.MarcarComoEntregue();
        return pedido;
    }
}