using DistribuidoraFlores.Api.Modules.Clientes.Domain;
using FluentAssertions;
using Xunit;

namespace DistribuidoraFlores.Tests.Unit.Clientes;

public class DocumentoTests
{
    [Theory]
    [InlineData("529.982.247-25")]
    [InlineData("52998224725")]
    public void Deve_aceitar_cpf_valido(string cpf)
    {
        var documento = Documento.Criar(cpf);

        documento.Tipo.Should().Be(TipoDocumento.CPF);
        documento.Numero.Should().Be("52998224725");
    }

    [Theory]
    [InlineData("11444777000161")] // CNPJ válido conhecido publicamente para testes
    public void Deve_aceitar_cnpj_valido(string cnpj)
    {
        var documento = Documento.Criar(cnpj);

        documento.Tipo.Should().Be(TipoDocumento.CNPJ);
    }

    [Theory]
    [InlineData("111.111.111-11")]
    [InlineData("000.000.000-00")]
    [InlineData("123.456.789-00")]
    public void Nao_deve_aceitar_cpf_invalido(string cpfInvalido)
    {
        var acao = () => Documento.Criar(cpfInvalido);

        acao.Should().Throw<ArgumentException>()
            .WithMessage("CPF inválido.");
    }

    [Fact]
    public void Nao_deve_aceitar_documento_com_tamanho_invalido()
    {
        var acao = () => Documento.Criar("123");

        acao.Should().Throw<ArgumentException>()
            .WithMessage("Documento deve ter 11 dígitos (CPF) ou 14 dígitos (CNPJ).");
    }

    [Fact]
    public void Deve_remover_pontuacao_do_numero()
    {
        var documento = Documento.Criar("529.982.247-25");

        documento.Numero.Should().NotContain(".").And.NotContain("-");
    }
}