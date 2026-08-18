namespace DistribuidoraFlores.Api.Modules.Pedidos.Api.DTOs;

public record CriarPedidoRequest(Guid ClienteId, List<ItemPedidoRequest> Itens);