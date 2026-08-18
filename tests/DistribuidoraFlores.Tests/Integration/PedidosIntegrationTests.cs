using System.Net;
using System.Net.Http.Json;
using FluentAssertions;
using Xunit;

namespace DistribuidoraFlores.Tests.Integration;

public class PedidosIntegrationTests : IClassFixture<CustomWebApplicationFactory>
{
    private readonly HttpClient _client;

    public PedidosIntegrationTests(CustomWebApplicationFactory factory)
    {
        _client = factory.CreateClient();
    }

    [Fact]
    public async Task Deve_criar_pedido_aprovar_separar_e_entregar()
    {
        var clienteId = await CriarClienteAsync();
        var produtoId = await CriarProdutoComEstoqueAsync();

        var pedidoRequest = new
        {
            clienteId,
            itens = new[] { new { produtoId, quantidade = 5 } }
        };

        var respostaCriacao = await _client.PostAsJsonAsync("/api/pedidos", pedidoRequest);
        respostaCriacao.StatusCode.Should().Be(HttpStatusCode.Created);

        var pedidoCriado = await respostaCriacao.Content.ReadFromJsonAsync<CriadoResponse>();

        (await _client.PatchAsync($"/api/pedidos/{pedidoCriado!.Id}/aprovar", null))
            .StatusCode.Should().Be(HttpStatusCode.NoContent);

        (await _client.PatchAsync($"/api/pedidos/{pedidoCriado.Id}/separar", null))
            .StatusCode.Should().Be(HttpStatusCode.NoContent);

        (await _client.PatchAsync($"/api/pedidos/{pedidoCriado.Id}/em-rota", null))
            .StatusCode.Should().Be(HttpStatusCode.NoContent);

        (await _client.PatchAsync($"/api/pedidos/{pedidoCriado.Id}/entregar", null))
            .StatusCode.Should().Be(HttpStatusCode.NoContent);

        var consultaFinal = await _client.GetAsync($"/api/pedidos/{pedidoCriado.Id}");
        var pedidoFinal = await consultaFinal.Content.ReadFromJsonAsync<PedidoResponse>();

        pedidoFinal!.Status.Should().Be("Entregue");
    }

    [Fact]
    public async Task Nao_deve_criar_pedido_para_cliente_inexistente()
    {
        var produtoId = await CriarProdutoComEstoqueAsync();

        var request = new
        {
            clienteId = Guid.NewGuid(), // cliente que não existe
            itens = new[] { new { produtoId, quantidade = 1 } }
        };

        var resposta = await _client.PostAsJsonAsync("/api/pedidos", request);

        resposta.StatusCode.Should().Be(HttpStatusCode.UnprocessableEntity);
    }

    [Fact]
    public async Task Deve_sinalizar_estoque_insuficiente_sem_bloquear_criacao()
    {
        var clienteId = await CriarClienteAsync();
        var produtoId = await CriarProdutoComEstoqueAsync(quantidadeEmEstoque: 10);

        var request = new
        {
            clienteId,
            itens = new[] { new { produtoId, quantidade = 100 } } // acima do disponível
        };

        var resposta = await _client.PostAsJsonAsync("/api/pedidos", request);
        resposta.StatusCode.Should().Be(HttpStatusCode.Created);

        var criado = await resposta.Content.ReadFromJsonAsync<CriadoResponse>();
        var consulta = await _client.GetAsync($"/api/pedidos/{criado!.Id}");
        var pedido = await consulta.Content.ReadFromJsonAsync<PedidoResponse>();

        pedido!.EstoqueInsuficienteNaCriacao.Should().BeTrue();
    }

    private async Task<Guid> CriarClienteAsync()
    {
        var request = new
        {
            nomeFantasia = $"Floricultura {Guid.NewGuid()}",
            documento = GerarCpfUnico(),
            telefone = "47999999999",
            email = $"{Guid.NewGuid()}@teste.com",
            endereco = "Rua X, 1"
        };

        var resposta = await _client.PostAsJsonAsync("/api/clientes", request);
        var criado = await resposta.Content.ReadFromJsonAsync<CriadoResponse>();
        return criado!.Id;
    }

    private async Task<Guid> CriarProdutoComEstoqueAsync(int quantidadeEmEstoque = 50)
    {
        var produtoRequest = new
        {
            nome = "Rosa Vermelha",
            categoria = "Flor",
            unidadeMedida = "unidade",
            precoUnitario = 5.50m
        };

        var respostaProduto = await _client.PostAsJsonAsync("/api/produtos", produtoRequest);
        var produtoCriado = await respostaProduto.Content.ReadFromJsonAsync<CriadoResponse>();

        var loteRequest = new
        {
            quantidade = quantidadeEmEstoque,
            dataValidade = DateTime.UtcNow.AddDays(10)
        };

        await _client.PostAsJsonAsync($"/api/produtos/{produtoCriado!.Id}/lotes", loteRequest);

        return produtoCriado.Id;
    }

    // CPFs válidos conhecidos, usados ciclicamente para evitar duplicidade entre testes
    private static readonly string[] CpfsValidos = { "52998224725", "64825687008", "87417048093", "11144477735" };
    private static int _indiceCpf = 0;

    private static string GerarCpfUnico()
    {
        var cpf = CpfsValidos[_indiceCpf % CpfsValidos.Length];
        _indiceCpf++;
        return cpf;
    }

    private record CriadoResponse(Guid Id);
    private record PedidoResponse(Guid Id, string Status, bool EstoqueInsuficienteNaCriacao);
}