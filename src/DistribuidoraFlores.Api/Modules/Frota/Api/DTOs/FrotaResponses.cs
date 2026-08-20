using DistribuidoraFlores.Api.Modules.Frota.Domain;

namespace DistribuidoraFlores.Api.Modules.Frota.Api.DTOs;

public record VeiculoResponse(Guid Id, string Placa, string Modelo, int CapacidadeKg, bool Ativo)
{
    public static VeiculoResponse FromDomain(Veiculo v) => new(v.Id, v.Placa, v.Modelo, v.CapacidadeKg, v.Ativo);
}

public record MotoristaResponse(Guid Id, string Nome, string Telefone, string Cnh, bool Ativo)
{
    public static MotoristaResponse FromDomain(Motorista m) => new(m.Id, m.Nome, m.Telefone, m.Cnh, m.Ativo);
}

public record EntregaResponse(Guid Id, Guid PedidoId, Guid VeiculoId, Guid MotoristaId, string Status, DateTime DataCriacao, DateTime? DataConclusao)
{
    public static EntregaResponse FromDomain(Entrega e) => new(e.Id, e.PedidoId, e.VeiculoId, e.MotoristaId, e.Status.ToString(), e.DataCriacao, e.DataConclusao);
}