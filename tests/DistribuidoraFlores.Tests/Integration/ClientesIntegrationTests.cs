using System.Net;
using System.Net.Http.Json;
using FluentAssertions;
using Xunit;

namespace DistribuidoraFlores.Tests.Integration;

public class ClientesIntegrationTests : IClassFixture<CustomWebApplicationFactory>
{
    private readonly HttpClient _client;

    public ClientesIntegrationTests(CustomWebApplicationFactory factory)
    {
        _client = factory.CreateClient();
    }

    [Fact]
    public async Task Deve_cadastrar_cliente_e_consultar_por_id()
    {
        var request = new
        {
            nomeFantasia = "Floricultura Bela Flor",
            documento = "529.982.247-25",
            telefone = "47999999999",
            email = "contato@belaflor.com",
            endereco = "Rua das Flores, 123"
        };

        var respostaCriacao = await _client.PostAsJsonAsync("/api/clientes", request);
        respostaCriacao.StatusCode.Should().Be(HttpStatusCode.Created);

        var criado = await respostaCriacao.Content.ReadFromJsonAsync<CriadoResponse>();

        var respostaConsulta = await _client.GetAsync($"/api/clientes/{criado!.Id}");
        var cliente = await respostaConsulta.Content.ReadFromJsonAsync<ClienteResponse>();

        cliente!.TipoDocumento.Should().Be("CPF");
        cliente.Ativo.Should().BeTrue();
    }

    [Fact]
    public async Task Nao_deve_cadastrar_cliente_com_documento_duplicado()
    {
        var request = new
        {
            nomeFantasia = "Floricultura X",
            documento = "648.256.870-08", // CPF válido diferente do teste anterior
            telefone = "47988888888",
            email = "x@x.com",
            endereco = "Rua X, 1"
        };

        await _client.PostAsJsonAsync("/api/clientes", request);
        var segundaResposta = await _client.PostAsJsonAsync("/api/clientes", request);

        segundaResposta.StatusCode.Should().Be(HttpStatusCode.Conflict);
    }

    [Fact]
    public async Task Deve_desativar_e_ativar_cliente()
    {
        var criado = await CriarClienteAsync();
        criado.Id.Should().NotBe(Guid.Empty, "o cliente deveria ter sido criado com sucesso");

        var respostaDesativar = await _client.PatchAsync($"/api/clientes/{criado.Id}/desativar", null);
        respostaDesativar.StatusCode.Should().Be(HttpStatusCode.NoContent);

        var consultaAposDesativar = await _client.GetAsync($"/api/clientes/{criado.Id}");
        var clienteDesativado = await consultaAposDesativar.Content.ReadFromJsonAsync<ClienteResponse>();
        clienteDesativado!.Ativo.Should().BeFalse();

        var respostaAtivar = await _client.PatchAsync($"/api/clientes/{criado.Id}/ativar", null);
        respostaAtivar.StatusCode.Should().Be(HttpStatusCode.NoContent);
    }

    private async Task<CriadoResponse> CriarClienteAsync()
    {
        var request = new
        {
            nomeFantasia = "Floricultura Teste",
            documento = "111.444.777-35", // CPF válido
            telefone = "47977777777",
            email = "teste@teste.com",
            endereco = "Rua Teste, 1"
        };

        var resposta = await _client.PostAsJsonAsync("/api/clientes", request);
        return (await resposta.Content.ReadFromJsonAsync<CriadoResponse>())!;
    }

    private record CriadoResponse(Guid Id);
    private record ClienteResponse(Guid Id, string NomeFantasia, string Documento, string TipoDocumento, bool Ativo);
}