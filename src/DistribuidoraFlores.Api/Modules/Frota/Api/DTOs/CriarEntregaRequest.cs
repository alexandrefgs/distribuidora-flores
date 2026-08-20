namespace DistribuidoraFlores.Api.Modules.Frota.Api.DTOs;

public record CriarEntregaRequest(Guid PedidoId, Guid VeiculoId, Guid MotoristaId);