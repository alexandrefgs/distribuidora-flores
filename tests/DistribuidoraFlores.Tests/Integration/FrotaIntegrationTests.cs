using System.Net;
using System.Net.Http.Json;
using FluentAssertions;
using Xunit;

namespace DistribuidoraFlores.Tests.Integration;

public class FrotaIntegrationTests : IClassFixture<CustomWebApplicationFactory>
{
    private readonly HttpClient _client;

    public FrotaIntegrationTests(CustomWebApplicationFactory factory)
    {
        _client = factory.CreateClient();
    }

    [Fact]
    public async Task Deve_criar_entrega_para_pedido_separado_e_concluir_fluxo()
    {
        var pedidoId = await CriarPedidoSeparadoAsync();
        var veiculoId = await CriarVeiculoAsync();
        var motoristaId = await CriarMotoristaAsync();

        var entregaRequest = new { pedidoId, veiculoId, motoristaId };
        var respostaCriacao = await _client.PostAsJsonAsync("/api/entregas", entregaRequest);
        respostaCriacao.StatusCode.Should().Be(HttpStatusCode.Created);

        var entregaCriada = await respostaCriacao.Content.ReadFromJsonAsync<CriadoResponse>();

        (await _client.PatchAsync($"/api/entregas/{entregaCriada!.Id}/iniciar-rota", null))
            .StatusCode.Should().Be(HttpStatusCode.NoContent);

        (await _client.PatchAsync($"/api/entregas/{entregaCriada.Id}/concluir", null))
            .StatusCode.Should().Be(HttpStatusCode.NoContent);

        var consulta = await _client.GetAsync($"/api/entregas/{entregaCriada.Id}");
        var entregaFinal = await consulta.Content.ReadFromJsonAsync<EntregaResponse>();

        entregaFinal!.Status.Should().Be("Concluida");
    }

    [Fact]
    public async Task Nao_deve_criar_entrega_para_pedido_ainda_pendente()
    {
        var clienteId = await CriarClienteAsync();
        var produtoId = await CriarProdutoComEstoqueAsync();

        var pedidoRequest = new { clienteId, itens = new[] { new { produtoId, quantidade = 5 } } };
        var respostaPedido = await _client.PostAsJsonAsync("/api/pedidos", pedidoRequest);
        var pedidoCriado = await respostaPedido.Content.ReadFromJsonAsync<CriadoResponse>();
        // Não aprova nem separa — fica Pendente de propósito

        var veiculoId = await CriarVeiculoAsync();
        var motoristaId = await CriarMotoristaAsync();

        var entregaRequest = new { pedidoId = pedidoCriado!.Id, veiculoId, motoristaId };
        var resposta = await _client.PostAsJsonAsync("/api/entregas", entregaRequest);

        resposta.StatusCode.Should().Be(HttpStatusCode.UnprocessableEntity);
    }

    private async Task<Guid> CriarPedidoSeparadoAsync()
    {
        var clienteId = await CriarClienteAsync();
        var produtoId = await CriarProdutoComEstoqueAsync();

        var pedidoRequest = new { clienteId, itens = new[] { new { produtoId, quantidade = 5 } } };
        var respostaPedido = await _client.PostAsJsonAsync("/api/pedidos", pedidoRequest);
        var pedidoCriado = await respostaPedido.Content.ReadFromJsonAsync<CriadoResponse>();

        await _client.PatchAsync($"/api/pedidos/{pedidoCriado!.Id}/aprovar", null);
        await _client.PatchAsync($"/api/pedidos/{pedidoCriado.Id}/separar", null);

        return pedidoCriado.Id;
    }

    private async Task<Guid> CriarClienteAsync()
    {
        var request = new
        {
            nomeFantasia = $"Floricultura {Guid.NewGuid()}",
            documento = "52998224725",
            telefone = "47999999999",
            email = $"{Guid.NewGuid()}@teste.com",
            endereco = "Rua X, 1"
        };

        var resposta = await _client.PostAsJsonAsync("/api/clientes", request);
        var criado = await resposta.Content.ReadFromJsonAsync<CriadoResponse>();
        return criado!.Id;
    }

    private async Task<Guid> CriarProdutoComEstoqueAsync()
    {
        var produtoRequest = new { nome = "Rosa Vermelha", categoria = "Flor", unidadeMedida = "unidade", precoUnitario = 5.50m };
        var respostaProduto = await _client.PostAsJsonAsync("/api/produtos", produtoRequest);
        var produtoCriado = await respostaProduto.Content.ReadFromJsonAsync<CriadoResponse>();

        var loteRequest = new { quantidade = 50, dataValidade = DateTime.UtcNow.AddDays(10) };
        await _client.PostAsJsonAsync($"/api/produtos/{produtoCriado!.Id}/lotes", loteRequest);

        return produtoCriado.Id;
    }

    private async Task<Guid> CriarVeiculoAsync()
    {
        var request = new { placa = $"XYZ{Random.Shared.Next(1000, 9999)}", modelo = "Fiorino", capacidadeKg = 500 };
        var resposta = await _client.PostAsJsonAsync("/api/veiculos", request);
        var criado = await resposta.Content.ReadFromJsonAsync<CriadoResponse>();
        return criado!.Id;
    }

    private async Task<Guid> CriarMotoristaAsync()
    {
        var request = new { nome = "João Silva", telefone = "47988887777", cnh = GerarCnhAleatoria() };
        var resposta = await _client.PostAsJsonAsync("/api/motoristas", request);
        var criado = await resposta.Content.ReadFromJsonAsync<CriadoResponse>();
        return criado!.Id;
    }

    private static string GerarCnhAleatoria()
    {
        return string.Concat(Enumerable.Range(0, 11).Select(_ => Random.Shared.Next(0, 10).ToString()));
    }

    private record CriadoResponse(Guid Id);
    private record EntregaResponse(Guid Id, string Status);
}