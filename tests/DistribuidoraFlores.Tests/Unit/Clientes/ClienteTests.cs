using DistribuidoraFlores.Api.Modules.Clientes.Domain;
using FluentAssertions;
using Xunit;

namespace DistribuidoraFlores.Tests.Unit.Clientes;

public class ClienteTests
{
    private static Cliente CriarClienteValido()
    {
        return new Cliente("Floricultura Bela Flor", "529.982.247-25", "47999999999", "contato@belaflor.com", "Rua das Flores, 123");
    }

    [Fact]
    public void Deve_criar_cliente_ativo_por_padrao()
    {
        var cliente = CriarClienteValido();

        cliente.Ativo.Should().BeTrue();
    }

    [Fact]
    public void Nao_deve_permitir_nome_vazio()
    {
        var acao = () => new Cliente("", "529.982.247-25", "47999999999", "contato@belaflor.com", "Rua X");

        acao.Should().Throw<ArgumentException>()
            .WithMessage("Nome é obrigatório.");
    }

    [Theory]
    [InlineData("emailinvalido")]
    [InlineData("")]
    public void Nao_deve_permitir_email_invalido(string emailInvalido)
    {
        var acao = () => new Cliente("Floricultura X", "529.982.247-25", "47999999999", emailInvalido, "Rua X");

        acao.Should().Throw<ArgumentException>()
            .WithMessage("Email inválido.");
    }

    [Fact]
    public void Cliente_deve_propagar_erro_de_documento_invalido()
    {
        // Confirma que o Cliente delega a validação pro Value Object Documento
        var acao = () => new Cliente("Floricultura X", "111.111.111-11", "47999999999", "x@x.com", "Rua X");

        acao.Should().Throw<ArgumentException>()
            .WithMessage("CPF inválido.");
    }

    [Fact]
    public void Desativar_e_Ativar_devem_alternar_o_status()
    {
        var cliente = CriarClienteValido();

        cliente.Desativar();
        cliente.Ativo.Should().BeFalse();

        cliente.Ativar();
        cliente.Ativo.Should().BeTrue();
    }
}