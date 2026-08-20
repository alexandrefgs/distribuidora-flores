using System.Net;
using System.Net.Http.Json;
using FluentAssertions;
using Xunit;
using DistribuidoraFlores.Api.Modules.Identidade.Domain;

namespace DistribuidoraFlores.Tests.Integration;

public class CatalogoIntegrationTests : IClassFixture<CustomWebApplicationFactory>
{
    private readonly HttpClient _client;

    public CatalogoIntegrationTests(CustomWebApplicationFactory factory)
    {
        _client = factory.CreateAuthenticatedClient(Role.Admin);
    }

    [Fact]
    public async Task Deve_criar_produto_e_consultar_por_id()
    {
        var request = new
        {
            nome = "Rosa Vermelha",
            categoria = "Flor",
            unidadeMedida = "unidade",
            precoUnitario = 5.50m
        };

        var respostaCriacao = await _client.PostAsJsonAsync("/api/produtos", request);
        respostaCriacao.StatusCode.Should().Be(HttpStatusCode.Created);

        var criado = await respostaCriacao.Content.ReadFromJsonAsync<CriadoResponse>();
        criado.Should().NotBeNull();

        var respostaConsulta = await _client.GetAsync($"/api/produtos/{criado!.Id}");
        respostaConsulta.StatusCode.Should().Be(HttpStatusCode.OK);

        var produto = await respostaConsulta.Content.ReadFromJsonAsync<ProdutoResponse>();
        produto!.Nome.Should().Be("Rosa Vermelha");
        produto.QuantidadeDisponivel.Should().Be(0); // sem lote ainda
    }

    [Fact]
    public async Task Deve_adicionar_lote_e_refletir_na_quantidade_disponivel()
    {
        var produtoCriado = await CriarProdutoAsync();

        var loteRequest = new
        {
            quantidade = 50,
            dataValidade = DateTime.UtcNow.AddDays(10)
        };

        var respostaLote = await _client.PostAsJsonAsync($"/api/produtos/{produtoCriado.Id}/lotes", loteRequest);
        respostaLote.StatusCode.Should().Be(HttpStatusCode.NoContent);

        var respostaConsulta = await _client.GetAsync($"/api/produtos/{produtoCriado.Id}");
        var produto = await respostaConsulta.Content.ReadFromJsonAsync<ProdutoResponse>();

        produto!.QuantidadeDisponivel.Should().Be(50);
    }

    [Fact]
    public async Task Nao_deve_criar_produto_com_nome_vazio()
    {
        var request = new
        {
            nome = "",
            categoria = "Flor",
            unidadeMedida = "unidade",
            precoUnitario = 5.50m
        };

        var resposta = await _client.PostAsJsonAsync("/api/produtos", request);

        resposta.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    private async Task<CriadoResponse> CriarProdutoAsync()
    {
        var request = new
        {
            nome = "Rosa Vermelha",
            categoria = "Flor",
            unidadeMedida = "unidade",
            precoUnitario = 5.50m
        };

        var resposta = await _client.PostAsJsonAsync("/api/produtos", request);
        return (await resposta.Content.ReadFromJsonAsync<CriadoResponse>())!;
    }

    private record CriadoResponse(Guid Id);
    private record ProdutoResponse(Guid Id, string Nome, string Categoria, string UnidadeMedida, decimal PrecoUnitario, int QuantidadeDisponivel);
}